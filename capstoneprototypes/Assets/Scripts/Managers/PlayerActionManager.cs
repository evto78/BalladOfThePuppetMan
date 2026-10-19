using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionManager : MonoBehaviour
{
    [System.Serializable]
    public class StatusEffectInstance
    {
        public StatusEffect data;
        public int stacks;
    }

    [Header("Functional")]
    public int hp;
    public int mhp;

    public int maxEnergy;
    public int energy;

    float timeSpent;

    public float parryWindow;
    public float parryCooldown;
    public float curParryCooldown = 0;
    bool parryActive;
    bool parryLanded;

    public float perfectWindow;
    public float perfectCooldown;
    public float curPerfectCooldown = 0;
    bool perfectActive;
    bool perfectLanded;

    public List<StatusEffectInstance> statEffects;
    public List<StatusEffect> statData;
    public List<PlayerSkill> equippedSkills;
    public List<PlayerSkill> skillData;

    [Header("Visuals")]
    public List<ParticleSystem> parryParticles;
    public List<ParticleSystem> perfectParticles;
    public Image energyFill;
    float visualEnergyLeft;
    public SpriteRenderer healthBar;
    public List<Sprite> healthBarStates;

    [Header("References")]
    PlayerSpriteAnimator psAnim;
    public TimelineManager timeline;
    AudioManager audioMan;
    GeneralAnimator gAnim;
    public SpriteRenderer sr;
    public Image fillBar;
    public GameOverAnim gameOverAnim;

    private void Awake()
    {
        timeline = GameObject.Find("Timeline").GetComponent<TimelineManager>();
        audioMan = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        gAnim = GetComponent<GeneralAnimator>();
        psAnim = GetComponent<PlayerSpriteAnimator>();

        statData.AddRange(Resources.LoadAll<StatusEffect>("StatusEffects"));
        SortStatusEffectData();

        skillData.AddRange(Resources.LoadAll<PlayerSkill>("Skills"));
        SortSkillData();
    }

    void Start()
    {
        hp = mhp;
        energy = maxEnergy;

        curParryCooldown = 0f;
        parryActive = false;
        parryLanded = false;

        curPerfectCooldown = 0f;
        perfectActive = false;
        perfectLanded = false;

        statEffects = new List<StatusEffectInstance>();
    }

    public void SkillSelectStart()
    {
        energy += 2;
        if (energy > maxEnergy) { energy = maxEnergy; }
        timeSpent = 0f;
    }

    void SortStatusEffectData()
    {
        List<int> comparisonList = new List<int>();
        List<StatusEffect> sortedItemData = new List<StatusEffect>();
        for (int i = 0; i < statData.Count; i++) { comparisonList.Add(i); sortedItemData.Add(null); }
        for (int i = 0; i < statData.Count; i++)
        {
            sortedItemData[comparisonList.IndexOf(statData[i].id)] = statData[i];
        }
        statData = sortedItemData;
    }

    void SortSkillData()
    {
        List<int> comparisonList = new List<int>();
        List<PlayerSkill> sortedItemData = new List<PlayerSkill>();
        for (int i = 0; i < skillData.Count; i++) { comparisonList.Add(i); sortedItemData.Add(null); }
        for (int i = 0; i < skillData.Count; i++)
        {
            sortedItemData[comparisonList.IndexOf(skillData[i].id)] = skillData[i];
        }
        skillData = sortedItemData;
    }

    void Update()
    {
        //Manage Timers
        curParryCooldown -= Time.deltaTime; if (curParryCooldown < 0) { curParryCooldown = 0; }
        curPerfectCooldown -= Time.deltaTime; if (curPerfectCooldown < 0) { curPerfectCooldown = 0; }

        //Check if parry / perfect window is over
        parryActive = parryCooldown - curParryCooldown < parryWindow; if (!parryActive) { parryLanded = false; }
        perfectActive = perfectCooldown - curPerfectCooldown < perfectWindow; if (!perfectActive) { perfectLanded = false; }

        GetInputs();
    }

    void GetInputs()
    {
        if (!timeline.combatPause && timeline.combatActive)
        {
            if (Input.GetMouseButtonDown(1)) { AttemptParry(); }
            if (Input.GetMouseButtonDown(0)) { AttemptPerfect(); }
        }
    }

    void AttemptParry()
    {
        if (curParryCooldown > 0 && !parryLanded) { return; }

        curParryCooldown = parryCooldown;
        parryActive = true;
    }

    void AttemptPerfect()
    {
        if (curPerfectCooldown > 0 && !perfectLanded) { return; }

        curPerfectCooldown = perfectCooldown;
        perfectActive = true;
    }

    void UpdateVisuals()
    {
        visualEnergyLeft = Mathf.Lerp(visualEnergyLeft, energy, Time.deltaTime * 10f);

        fillBar.fillAmount = (float)hp / (float)mhp;
        energyFill.fillAmount = (Mathf.CeilToInt((visualEnergyLeft / (float)maxEnergy) * 16f)) / 16f;
        healthBar.sprite = healthBarStates[Mathf.FloorToInt(((float)hp/(float)mhp)*(healthBarStates.Count-1))];
    }

    void LateUpdate()
    {
        UpdateVisuals();
        psAnim.UpdateVisuals(parryActive, parryLanded, perfectActive, perfectLanded);
    }

    public IEnumerator ReactableEvent(TimelineManager.TimelineEvent onComingEvent)
    {
        //If player is already parrying, then resolve this now.
        if (onComingEvent.eventType == TimelineManager.EventType.EnemyAtk)
        {
            if (parryActive) { MonsterAttacked(onComingEvent, 0f); }
            else
            {
                //Otherwise, Wait for up to half of the parry window, and check again.
                bool attackProcessed = false;
                float tempTimer = parryWindow / 2f;
                while (tempTimer > 0 && attackProcessed == false)
                {
                    if (parryActive) { attackProcessed = true; MonsterAttacked(onComingEvent, (parryWindow / 2f) - tempTimer); }
                    tempTimer -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                //If the attack still wasn't parried, send it now.
                if (!attackProcessed) { MonsterAttacked(onComingEvent, (parryWindow / 2f) - tempTimer); }
            }
        }

        //If player is already perfecting, then resolve this now.
        else if (onComingEvent.eventType == TimelineManager.EventType.PlayerAtk)
        {
            if (perfectActive) { PlayerActivateSkill(onComingEvent, 0f); }
            else
            {
                //Otherwise, Wait for up to half of the perfect window, and check again.
                bool attackProcessed = false;
                float tempTimer = perfectWindow / 2f;
                while (tempTimer > 0 && attackProcessed == false)
                {
                    if (perfectActive) { attackProcessed = true; PlayerActivateSkill(onComingEvent, (perfectWindow / 2f) - tempTimer); }
                    tempTimer -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                //If the attack still wasn't parried, send it now.
                if (!attackProcessed) { PlayerActivateSkill(onComingEvent, (perfectWindow / 2f) - tempTimer); }
            }
        }

        yield return null;
    }

    //An enemy is about to hit the player
    public void MonsterAttacked(TimelineManager.TimelineEvent attackEvent, float delay)
    {
        if (parryActive)
        {
            ParryLanded();
        }
        else
        {
            TakeDamage(attackEvent.enemyAction.damage);
        }
    }

    //One of the players skills are about to activate
    public void PlayerActivateSkill(TimelineManager.TimelineEvent attackEvent, float delay)
    {
        PlayerSkill skill = attackEvent.playerSkill;
        float perfectMult = 1;

        if (skill.perfectable && perfectActive)
        {
            PerfectLanded();
            perfectMult = skill.perfectMultiplier;
        }

        switch (skill.id)
        {
            case 0: StandardAttack(attackEvent, skill, perfectMult); break; //Strike
            case 1: StandardAttack(attackEvent, skill, perfectMult); break; //Bash
            case 2: StandardAttack(attackEvent, skill, perfectMult); break; //Heavy Bash
            case 4: AddStatusEffect(0, Mathf.CeilToInt(skill.intensity * perfectMult)); break; //Focus
            case 5: StandardAttack(attackEvent, skill, perfectMult); break; //Barrage
            case 6: StandardAttack(attackEvent, skill, perfectMult); break; //Headbutt
            case 7: StandardAttack(attackEvent, skill, perfectMult); break; //Calm Punch
            case 8: StandardAttack(attackEvent, skill, perfectMult); AddStatusEffect(0, Mathf.CeilToInt(skill.intensity * perfectMult)); break; //Focused Strike
            case 9: StandardAttack(attackEvent, skill, perfectMult); break; //Strong Swing
            case 10: StandardAttack(attackEvent, skill, perfectMult); break; //Poke
        }
    }

    void StandardAttack(TimelineManager.TimelineEvent attackEvent, PlayerSkill skill, float perfectMult)
    {
        //If enemy previously targeted is gone, aim for another enemy.
        if (attackEvent.enemyTargeted == null && timeline.activeEnemies.Count > 0) { attackEvent.enemyTargeted = timeline.activeEnemies[0]; }
        //If no enemies are left alive, cancel the attack.
        else if (timeline.activeEnemies.Count == 0) { return; }
        //Do the attack.
        attackEvent.enemyTargeted.TakeDamage(Mathf.CeilToInt(skill.intensity * perfectMult * ElementalMods.Modifier(skill.elementalType, attackEvent.enemyTargeted.elementalType)));
    }

    //Called by the skill selection buttons when clicked
    public void AttemptUseSkill(int skillSlotIndex)
    {
        if (timeline.combatActive && timeline.combatPause)
        {
            PlayerSkill skill = equippedSkills[skillSlotIndex];

            if (timeline.combatTime - timeSpent < skill.prepTime) { Debug.LogError("Not enough time to use skill."); return; }
            if (energy < skill.cost) { Debug.LogError("Not enough energy to use skill."); return; }

            int timeIndex = timeline.IndexFromTime(timeSpent + skill.prepTime);
            int timePassedIndex = timeline.IndexFromTime(timeSpent);

            timeline.AddMarker(timeIndex, TimelineManager.EventType.PlayerAtk, null, null, skill, timeline.activeEnemies[0]);
            timeline.AddSkillBlock(timeIndex, timePassedIndex);
            timeSpent += skill.prepTime;
            energy -= skill.cost;
        }
    }

    //Called by the skill selection buttons when hovered over
    public void PreviewSkill(int skillSlotIndex)
    {
        if (timeline.combatActive && timeline.combatPause)
        {
            PlayerSkill skill = equippedSkills[skillSlotIndex];

            bool skillIsPlaceable = timeline.combatTime - timeSpent >= skill.prepTime;

            int timeIndex = timeline.IndexFromTime(timeSpent + skill.prepTime);
            int timePassedIndex = timeline.IndexFromTime(timeSpent);

            timeline.PreviewSkill(timeIndex, timePassedIndex, equippedSkills[skillSlotIndex], skillIsPlaceable);
        }
    }

    //Called by the skill blocks when clicked
    public void AttemptRemoveSkill(TimelineManager.TimelineEvent tEvent)
    {
        if (tEvent.skillBlock != null)
        {
            timeline.RemoveEventAndMarker(tEvent, true);
        }
    }

    //Called by the timeline when sucsessfully removed
    public void SkillRemoved(TimelineManager.TimelineEvent tEvent)
    {
        //Recover resources spent on that skill
        timeSpent -= tEvent.playerSkill.prepTime;
        energy += tEvent.playerSkill.cost;
    }

    void AddStatusEffect(int id, int stacks)
    {
        bool statusApplied = false;
        foreach(StatusEffectInstance effectInstance in statEffects)
        {
            if (effectInstance.data.id == id) { effectInstance.stacks += stacks; statusApplied = true; }
        }
        if (!statusApplied) 
        {
            StatusEffectInstance newEffectInstance = new StatusEffectInstance();
            newEffectInstance.stacks = stacks;
            newEffectInstance.data = statData[id];
            statEffects.Add(newEffectInstance); 
        }
    }

    void TakeDamage(int dmg)
    {
        hp -= dmg;
        psAnim.hitOverrideTimer = 0.25f;
        if (hp <= 0) { Die(); }
    }

    void Die()
    {
        Debug.Log("DEAD!");
        gameObject.SetActive(false);
        gameOverAnim.PlayAnim();
    }

    void ParryLanded()
    {
        parryLanded = true;
        foreach (ParticleSystem ps in parryParticles) { ps.Stop(); ps.Play(); }
    }

    void PerfectLanded()
    {
        perfectLanded = true;
        foreach (ParticleSystem ps in perfectParticles) { ps.Stop(); ps.Play(); }
    }
}
