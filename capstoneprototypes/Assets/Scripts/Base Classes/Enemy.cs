using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for all enemies
public class Enemy : MonoBehaviour
{
    [System.Serializable]
    public class Action
    {
        public TimelineManager.EventType actionType;
        public bool oneOff; //If true, the enemy will only do this action in the first loop of the action sequence.

        public int damage; //If the action is an attack, how much damage should it do? If the action is speed up / down, how much should the combat time change by?

        //Try to make the delay in 160ths. As a tip, 0.05 is 8/160, and so 0.025 is 4/160. Not strictly needed, but helps make timing make more sense!
        public float delayBeforeAction; //How long should this enemy wait before doing this action?
        public float delayAfterAction; //How long should this enemy wait after doing this action?

        public bool playAnim; //Should this action change the enemies sprite at all.
        //Make sure that the wind up and wind down fits within the delay before after the action. 
        //Set both to 0 if you don't want an animaiton change \/
        public float animWindupStart; //The amount of time before the end of the "delayBeforeAction" where the enemy changes their sprite to prepare for an attack.
        public float animWinddownEnd; //The amount of time after the end of the "delayBeforeAction" where the enemy changes their sprite to attack.
    }

    [System.Serializable]
    public class AttackPattern
    {
        public int timelineChangePriority; //This pattern prioritizes its changes to the timeline above all other changes with less priority. Try to never match priority, unless it is the same changes.
        public float timelineCombatTimePref; //What combat time does this pattern prefer?
        public float timelineCombatSpeedPref; //What combat speed does this pattern prefer?
        public List<Action> actionSequence = new List<Action>();
    }

    [Header("Visuals")]
    EnemySpriteLibrary spriteLib; // the database of all sprites
    EnemySpriteAnimator anim;

    [Header("Stats")]
    public int mhp;
    public int hp;
    public ElementalTypes elementalType;

    [Header("Functional")]
    TimelineManager timeline;
    float curTimePassed; //The amount of time passed according to the timeline. Accurate to all timeline distortions.
    float curTimelineIndex; //The amount of time passed according to the current index of the timeline relative to its resolution. Accurate to all timeline distortions.
    PlayerActionManager player;
    AudioManager audioMan;
    public EnemyData myData;
    public List<AttackPattern> attackPatterns;
    int curRound;
    int curLoop; //Number of times that the action sequence has looped this round.
    public List<int> patternByRound;

    private void Awake()
    {
        spriteLib = GameObject.Find("EnemySpriteLibrary").GetComponent<EnemySpriteLibrary>();

        if (myData != null) { SetEnemyData(-1, myData); } //If data is set in editor, use that data

        hp = mhp;

        anim = GetComponent<EnemySpriteAnimator>();
        timeline = GameObject.Find("Timeline").GetComponent<TimelineManager>();
        player = GameObject.Find("Player").GetComponent<PlayerActionManager>();
        audioMan = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        timeline.activeEnemies.Add(this);
    }

    void Start()
    {
        curTimePassed = 0;
    }

    //Data is optional, but if it is given, it is prefered.
    public void SetEnemyData(int id, EnemyData data)
    {
        if (data == null && id >= 0) { myData = Resources.Load <EnemyData> ("Enemies/" + id.ToString()); }
        else { myData = data; }

        if (myData == null) { Debug.LogError("ENEMY DATA NOT FOUND!"); }

        mhp = myData.maxHP;
        hp = mhp;
        elementalType = myData.type;

        attackPatterns = new List<AttackPattern>();
        AttackPattern newPattern = new AttackPattern();
        foreach (AttackPattern aPattern in myData.attackPatterns)
        {
            newPattern = new AttackPattern();

            newPattern.timelineChangePriority = aPattern.timelineChangePriority;
            newPattern.timelineCombatSpeedPref = aPattern.timelineCombatSpeedPref;
            newPattern.timelineCombatTimePref = aPattern.timelineCombatTimePref;

            foreach (Action a in aPattern.actionSequence)
            {
                newPattern.actionSequence.Add(a);
            }

            attackPatterns.Add(newPattern);
        }

        patternByRound = myData.patternByRound;


        if (spriteLib == null) { spriteLib = GameObject.Find("EnemySpriteLibrary").GetComponent<EnemySpriteLibrary>(); }

        //Set up anim sheets
        anim = GetComponent<EnemySpriteAnimator>();
        anim.idleAnim = spriteLib.globalSpriteList[myData.id].anims[0].animFrames;
        anim.preAttackAnim = spriteLib.globalSpriteList[myData.id].anims[1].animFrames;
        anim.postAttackAnim = spriteLib.globalSpriteList[myData.id].anims[2].animFrames;
        anim.hurtAnim = spriteLib.globalSpriteList[myData.id].anims[3].animFrames;
    }

    private void OnDisable()
    {
        if (timeline != null && timeline.activeEnemies.Contains(this))
        {
            timeline.EnemyDied(this); 
            timeline.activeEnemies.Remove(this);
        }
    }

    private void OnDestroy()
    {
        if (timeline != null && timeline.activeEnemies.Contains(this))
        {
            timeline.EnemyDied(this);
            timeline.activeEnemies.Remove(this);
        }
    }

    public void SetupMarkers()
    {
        curTimePassed = timeline.timePassed;
        curTimelineIndex = timeline.eventIndex;
        curRound = timeline.roundNumber;

        AttackPattern curPattern = GetPatternFromRound(curRound);

        float accumulatedDelay = 0;
        curLoop = 0;
        while (accumulatedDelay <= timeline.combatTime)
        {
            foreach (Action action in curPattern.actionSequence)
            {
                if (curLoop == 0 || (curLoop > 0 && !action.oneOff)) // Only counts one offs in the first loop.
                {
                    accumulatedDelay += action.delayBeforeAction;
                    if (accumulatedDelay > timeline.combatTime) { break; }
                    int indexOfDelay = Mathf.FloorToInt((accumulatedDelay / timeline.combatTime) * timeline.resolution);
                    timeline.AddMarker(indexOfDelay, action.actionType, this, action, null, null);
                    accumulatedDelay += action.delayAfterAction;
                }
            }
            curLoop++;
        }
    }
    public AttackPattern GetPatternFromRound(int round)
    {
        int relitiveRound = round % patternByRound.Count;

        AttackPattern curPattern = attackPatterns[patternByRound[relitiveRound]];

        return curPattern;
    }
    //Update as normal, but sent from the timeline. This is to make sure that timing is accurate.
    public void TimelineUpdate()
    {
        curTimePassed = timeline.timePassed;
        curTimelineIndex = timeline.eventIndex;
        curRound = timeline.roundNumber;

        FindCurState();

        UpdateVisuals();
    }

    private void LateUpdate()
    {
        UpdateVisuals();
    }

    void FindCurState()
    {
        AttackPattern curPattern = GetPatternFromRound(curRound);

        float accumulatedDelay = 0;
        bool actionFound = false;
        int loopLimit = 1000;
        curLoop = 0;
        while (!actionFound)
        {
            foreach (Action action in curPattern.actionSequence)
            {
                if (curLoop == 0 || (curLoop > 0 && !action.oneOff)) // Only counts one offs in the first loop.
                {
                    if (curTimePassed >= accumulatedDelay && curTimePassed < accumulatedDelay + action.delayBeforeAction + action.delayAfterAction)
                    {
                        actionFound = true;
                        RunCurAction(action, curTimePassed - accumulatedDelay, timeline.prevTimePassed - accumulatedDelay); break;
                    }
                    else
                    {
                        accumulatedDelay += action.delayBeforeAction + action.delayAfterAction;
                    }
                }
            }
            loopLimit--; curLoop++;
            if (loopLimit < 1) { Debug.LogError("ENEMY: " + gameObject.name + ", NO ACTION COULD BE FOUND!"); break; }
        }
    }

    void RunCurAction(Action action, float relitiveTime, float prevRelativeTime)
    {
        if (action.playAnim) { anim.UpdateAnimationByTime(relitiveTime, action); UpdateVisuals(); }
    }

    public void AddInitialOffset(float time)
    {
        AttackPattern firstPattern = GetPatternFromRound(0);
        Action offsetAction = new Action();

        offsetAction.actionType = TimelineManager.EventType.None;
        offsetAction.oneOff = true;
        offsetAction.damage = 0;
        offsetAction.delayBeforeAction = time;
        offsetAction.delayAfterAction = 0;
        offsetAction.playAnim = false;
        offsetAction.animWindupStart = 0;
        offsetAction.animWinddownEnd = 0;

        firstPattern.actionSequence.Insert(0, offsetAction);
    }

    void UpdateVisuals()
    {
        anim.AnimUpdate();
    }

    public void Attack(TimelineManager.TimelineEvent attackEvent)
    {
        if (player == null || !player.isActiveAndEnabled) { return; }
        player.StartCoroutine(player.ReactableEvent(attackEvent));
    }

    public void PrepareForNextRound()
    {
        UpdateVisuals();
    }

    public void TakeDamage(int damage)
    {
        anim.HurtAnim();

        hp -= damage;

        if (hp <= 0) { Die(); }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
