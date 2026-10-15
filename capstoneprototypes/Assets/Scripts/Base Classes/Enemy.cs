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
    List<Sprite> spriteList; //0: idle, 1: windup, 2: attack, 3: hurt, 4+ is for extra sprites / attacks.
    public SpriteRenderer sr;
    int activeSprite;
    float hitTimer = 0f; //When more than 1, count down and force display hurt animation.

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
    List<AttackPattern> attackPatterns;
    int curRound;
    public List<int> patternByRound;

    private void Awake()
    {
        spriteLib = GameObject.Find("EnemySpriteLibrary").GetComponent<EnemySpriteLibrary>();

        if (myData != null) { SetEnemyData(-1, myData); } //If data is set in editor, use that data

        hp = mhp;

        timeline = GameObject.Find("Timeline").GetComponent<TimelineManager>();
        player = GameObject.Find("Player").GetComponent<PlayerActionManager>();
        audioMan = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        timeline.activeEnemies.Add(this);
    }

    void Start()
    {
        curTimePassed = 0;
        activeSprite = 0;
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
        attackPatterns = myData.attackPatterns;
        patternByRound = myData.patternByRound;


        if (spriteLib == null) { spriteLib = GameObject.Find("EnemySpriteLibrary").GetComponent<EnemySpriteLibrary>(); }

        spriteList = spriteLib.globalSpriteList[myData.id].spriteList;
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
        AttackPattern curPattern = GetPatternFromRound(curRound);

        float accumulatedDelay = 0;
        while (accumulatedDelay <= timeline.combatTime)
        {
            foreach (Action action in curPattern.actionSequence)
            {
                accumulatedDelay += action.delayBeforeAction;
                if (accumulatedDelay > timeline.combatTime) { break; }
                int indexOfDelay = Mathf.FloorToInt((accumulatedDelay / timeline.combatTime) * timeline.resolution);
                timeline.AddMarker(indexOfDelay, action.actionType, this, action, null, null);
                accumulatedDelay += action.delayAfterAction;
            }
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

    void FindCurState()
    {
        AttackPattern curPattern = GetPatternFromRound(curRound);

        float accumulatedDelay = 0;
        bool actionFound = false;
        int loopLimit = 1000;
        while (!actionFound)
        {
            foreach (Action action in curPattern.actionSequence)
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
            loopLimit--;
            if (loopLimit < 1) { Debug.LogError("ENEMY: " + gameObject.name + ", NO ACTION COULD BE FOUND!"); break; }
        }
    }

    void RunCurAction(Action action, float relitiveTime, float prevRelitiveTime)
    {
        if (relitiveTime < action.delayBeforeAction)
        {
            //before delay
            if ((relitiveTime > action.delayBeforeAction - action.animWindupStart) && action.playAnim)
            {
                activeSprite = 1; //prepare for attack
            }
            if (action.playAnim)
            {
            }
        }
        else if (prevRelitiveTime < action.delayBeforeAction)
        {
            //action!
            if (action.playAnim) 
            { 
                activeSprite = 2; 
            }

            //ActivateAction(action);
        }
        else
        {
            //after delay
            if (relitiveTime < action.delayBeforeAction + action.animWinddownEnd && action.playAnim)
            {
                activeSprite = 2; //attack anim
            }
            else if (action.playAnim)
            {
                activeSprite = 0; //idle anim
            }
        }
    }

    void UpdateVisuals()
    {
        if (hitTimer > 0) { sr.sprite = spriteList[3]; hitTimer -= Time.deltaTime; } // Force set hit animation
        else
        {
            sr.sprite = spriteList[activeSprite];
        }
    }

    public void Attack(TimelineManager.TimelineEvent attackEvent)
    {
        if (player == null || !player.isActiveAndEnabled) { return; }
        player.StartCoroutine(player.ReactableEvent(attackEvent));
    }

    public void PrepareForNextRound()
    {
        activeSprite = 0;
        UpdateVisuals();
    }

    public void TakeDamage(int damage)
    {
        hitTimer = 0.25f;

        hp -= damage;

        if (hp <= 0) { Die(); }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
