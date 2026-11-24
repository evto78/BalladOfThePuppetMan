using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public int curMemories = 0;

    float timeSpent;

    public float parryWindow;
    public float parryCooldown;
    public float curParryCooldown = 0;
    public float curParryCooldownVisual = 0;
    bool parryActive;
    bool parryLanded;

    public float perfectWindow;
    public float perfectCooldown;
    public float curPerfectCooldown = 0;
    public float curPerfectCooldownVisual = 0;
    bool perfectActive;
    bool perfectLanded;

    public List<StatusEffectInstance> statEffects;
    public List<StatusEffect> statData;
    public List<PlayerSkill> equippedSkills;
    public List<PlayerSkill> skillData;
    public List<PlayerSkill> modifiedSkills;

    public List<ItemObject> equippedItems;
    bool damageTakenDuringCombat = false;
    public bool firstRoundofCombat = true;
    public int memoryGain = 0;

    public Enemy targetedEnemy;
    public Transform targetUI;
    public GameObject largeTarget;
    public GameObject smallTarget;

    [Header("Visuals")]
    public List<ParticleSystem> parryParticles;
    public List<ParticleSystem> perfectParticles;
    public List<ParticleSystem> damageParticles;
    public Image energyFill;
    float visualEnergyLeft;
    public SpriteRenderer healthBar;
    public List<Sprite> healthBarStates;
    public Color positiveMemoriesColor;
    public Color negativeMemoriesColor;

    [Header("References")]
    PlayerSpriteAnimator psAnim;
    public TimelineManager timeline;
    AudioManager audioMan;
    GeneralAnimator gAnim;
    public SpriteRenderer sr;
    public Image fillBar;
    public TextMeshProUGUI healthTxt;
    public TextMeshProUGUI maxHealthTxt;
    public GameOverAnim gameOverAnim;
    public GameDataManager gdm;
    public TextMeshProUGUI memoryCount;
    public GameObject damageIndicatorPrefab;

    private void Awake()
    {
        gdm = GameObject.Find("GameDataManager").GetComponent<GameDataManager>();
        timeline = GameObject.Find("Timeline").GetComponent<TimelineManager>();
        audioMan = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        gAnim = GetComponent<GeneralAnimator>();
        psAnim = GetComponent<PlayerSpriteAnimator>();

        statData.AddRange(Resources.LoadAll<StatusEffect>("StatusEffects"));
        SortStatusEffectData();

        skillData.AddRange(Resources.LoadAll<PlayerSkill>("Skills"));
        SortSkillData();

        equippedItems = new List<ItemObject>();
    }

    void Start()
    {
        hp = mhp;
        energy = maxEnergy;

        memoryCount.transform.parent.gameObject.SetActive(false);

        curParryCooldown = 0f;
        curParryCooldownVisual = 0f;
        parryActive = false;
        parryLanded = false;

        curPerfectCooldown = 0f;
        curPerfectCooldownVisual = 0f;
        perfectActive = false;
        perfectLanded = false;

        statEffects = new List<StatusEffectInstance>();
    }

    public void SkillSelectStart()
    {
        energy = Mathf.Clamp(energy += 2, 0, maxEnergy);
        memoryGain = 0;

        List<StatusEffectInstance> deletedEffects = new List<StatusEffectInstance>();
        foreach (StatusEffectInstance se in statEffects)
        {
            switch (se.data.id)
            {
                case 0:
                    se.stacks--;
                    if (se.stacks <= 0) { deletedEffects.Add(se); }
                    break;
            }
        }
        RemoveEffectsFromList(deletedEffects);

        foreach (ItemObject i in equippedItems)
        {
            switch(i.id)
            {
                case 0:
                    if (!damageTakenDuringCombat)
                    {
                        hp = Mathf.Clamp(hp += 2, 0, mhp);
                    }
                        break;
                case 4:
                    if (firstRoundofCombat)
                    {
                        energy += 2;
                    }
                    break;
                case 5:
                    if (!firstRoundofCombat)
                    {
                        energy = Mathf.Clamp(energy += 1, 0, maxEnergy);
                    }
                    break;
            }
        }
        damageTakenDuringCombat = false;

        if (firstRoundofCombat) { firstRoundofCombat = false; }
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
        if (targetedEnemy != null) 
        { 
            targetUI.gameObject.SetActive(true); 
            targetUI.position = targetedEnemy.transform.position; 
            if (targetedEnemy.myData.id == 1) 
            { largeTarget.SetActive(true); smallTarget.SetActive(false); }
            else { largeTarget.SetActive(false); smallTarget.SetActive(true); }
        } 
        else { targetUI.gameObject.SetActive(false); }

        //Manage Timers
        curParryCooldown -= Time.deltaTime; if (curParryCooldown < 0) { curParryCooldown = 0; }
        curPerfectCooldown -= Time.deltaTime; if (curPerfectCooldown < 0) { curPerfectCooldown = 0; }
        curParryCooldownVisual -= Time.deltaTime; if (curParryCooldownVisual < 0) { curParryCooldownVisual = 0; }
        curPerfectCooldownVisual -= Time.deltaTime; if (curPerfectCooldownVisual < 0) { curPerfectCooldownVisual = 0; }

        //Check if parry / perfect window is over
        parryActive = parryCooldown - curParryCooldown < parryWindow / 2f; if (!parryActive) { parryLanded = false; }
        perfectActive = perfectCooldown - curPerfectCooldown < perfectWindow / 2f; if (!perfectActive) { perfectLanded = false; }

        GetInputs();

        //MONEY!
        if (curMemories > 9999) { memoryCount.text = "ALOT"; }
        else { memoryCount.text = curMemories.ToString(); }
        if (curMemories >= 0) { memoryCount.color = positiveMemoriesColor; } else { memoryCount.color = negativeMemoriesColor; }
        if (gdm.gameState == GameDataManager.State.Shop) { memoryCount.transform.parent.gameObject.SetActive(true); }
        else if (!memoryAnimActive) { memoryCount.transform.parent.gameObject.SetActive(false); }
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
        curParryCooldownVisual = parryCooldown;
        parryActive = true;
    }

    void AttemptPerfect()
    {
        if (curPerfectCooldown > 0 && !perfectLanded) { return; }

        curPerfectCooldown = perfectCooldown;
        curPerfectCooldownVisual = perfectCooldown;
        perfectActive = true;
    }

    void UpdateVisuals()
    {
        visualEnergyLeft = Mathf.Lerp(visualEnergyLeft, energy, Time.deltaTime * 10f);

        fillBar.fillAmount = (float)hp / (float)mhp;
        energyFill.fillAmount = (Mathf.CeilToInt((visualEnergyLeft / (float)maxEnergy) * 16f)) / 16f;
        healthBar.sprite = healthBarStates[Mathf.FloorToInt(((float)hp/(float)mhp)*(healthBarStates.Count-1))];

        healthTxt.text = hp.ToString();
        maxHealthTxt.text = mhp.ToString();
    }

    void LateUpdate()
    {
        UpdateVisuals();
        psAnim.UpdateVisuals(parryActive, parryLanded, perfectActive, perfectLanded);
    }

    public IEnumerator ReactableEvent(TimelineManager.TimelineEvent onComingEvent)
    {
        //return;
        //timeline.timePassed;
        //timeline.TimeFromIndex(onComingEvent.timelineIndex);

        //If player is already parrying, then resolve this now.
        if (onComingEvent.eventType == TimelineManager.EventType.EnemyAtk)
        {
            if (parryActive) { MonsterAttacked(onComingEvent, curParryCooldown - parryCooldown); }
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
            if (perfectActive) { PlayerActivateSkill(onComingEvent, curPerfectCooldown - perfectCooldown); }
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

        else if (onComingEvent.eventType == TimelineManager.EventType.Heal)
        {
            if (parryActive) { GurbyHeal(onComingEvent, curParryCooldown - parryCooldown); }
            else
            {
                //Otherwise, Wait for up to half of the parry window, and check again.
                bool attackProcessed = false;
                float tempTimer = parryWindow / 2f;
                while (tempTimer > 0 && attackProcessed == false)
                {
                    if (parryActive) { attackProcessed = true; GurbyHeal(onComingEvent, (parryWindow / 2f) - tempTimer); }
                    tempTimer -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                //If the attack still wasn't parried, send it now.
                if (!attackProcessed) { GurbyHeal(onComingEvent, (parryWindow / 2f) - tempTimer); }
            }
        }

        yield return null;
    }

    //An enemy is about to hit the player
    public void MonsterAttacked(TimelineManager.TimelineEvent attackEvent, float delay)
    {
        if (parryActive)
        {
            attackEvent.removed = true;
            ParryLanded();
            if (delay >= -0.025f && delay <= 0.025f)
            {
                //Add particle spawn for PERFECT timing going RIGHT
            }
            else
            {
                //Add particle spawn for GOOD timing going RIGHT
            }
            Debug.Log(delay);
        }
        else
        {
            TakeDamage(attackEvent.enemyAction.damage);
            //Add arrow color change to red for missed timing
        }
    }

    //Gurby is about to heal the player
    public void GurbyHeal(TimelineManager.TimelineEvent healEvent, float delay)
    {
        if (parryActive)
        {
            healEvent.removed = true;
            TakeDamage(-healEvent.enemyAction.damage);
            if (hp > mhp) { hp = mhp; }
            ParryLanded();
            if (delay >= -0.025f && delay <= 0.025f)
            {
                //Add particle spawn for PERFECT timing going LEFT
            }
            else
            {
                //Add particle spawn for GOOD timing going LEFT
            }
            Debug.Log(delay);
        }
        else
        {
            //Add arrow color change to red for missed timing
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
            attackEvent.removed = true;
            perfectMult = skill.perfectMultiplier;
            //Add star particle for hitting a timing (spawn from "liveTimelineFeedbackPoint.position) *MAKE THESE PARTICLES GO LEFT
        }
        else
        {
            TakeDamage(attackEvent.enemyAction.damage);
            //Add "!" particle or "x" particle for missing a timing (spawn from "liveTimelineFeedbackPoint.position) *MAKE THESE PARTICLES GO LEFT
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
                case 11: StandardAttack(attackEvent, skill, perfectMult); break; //Devistating Blow
                case 12: StandardAttack(attackEvent, skill, perfectMult); break; //KILL
            }
    }

    void StandardAttack(TimelineManager.TimelineEvent attackEvent, PlayerSkill skill, float perfectMult)
    {
        float bonusMult = 1f;
        float elementalMult = 1f;
        List<StatusEffectInstance> deletedEffects = new List<StatusEffectInstance>();
        foreach (StatusEffectInstance se in statEffects)
        {
            switch (se.data.id)
            {
                case 0:
                    bonusMult += 0.5f * se.stacks;
                    deletedEffects.Add(se);
                    break;
            }
        }
        RemoveEffectsFromList(deletedEffects);
        foreach (ItemObject i in equippedItems)
        {
            switch (i.id)
            {
                case 6:
                    if (skill.elementalType == ElementalTypes.Power)
                    {
                        bonusMult += 0.5f;
                    }
                    break;
                case 7:
                    if (skill.elementalType == ElementalTypes.Money)
                    {
                        bonusMult += 0.5f;
                    }
                    break;
                case 8:
                    if (skill.elementalType == ElementalTypes.Fame)
                    {
                        bonusMult += 0.5f;
                    }
                    break;
                case 13:
                    if (skill.elementalType == ElementalTypes.Corruption)
                    {
                        bonusMult += 0.5f;
                        TakeDamage(1);
                    }
                    break;
                case 16:
                    float coinMult = 0;
                    if (curMemories > 0)
                    {
                        coinMult = Mathf.Clamp(coinMult += 0.01f * curMemories, 0f, 1f);
                        bonusMult += coinMult;
                    }
                    else if (curMemories < 0)
                    {
                        coinMult = Mathf.Clamp(coinMult += 0.005f * curMemories, -0.5f, 0f);
                        bonusMult += coinMult;
                    }
                    break;
            }
        }

        if (skill.target == PlayerSkill.TargetType.SingleEnemy)
        {
            elementalMult = ElementalMods.Modifier(skill.elementalType, attackEvent.enemyTargeted.elementalType);
            foreach (ItemObject i in equippedItems)
            {
                switch (i.id)
                {
                    case 1:
                        if (elementalMult > 1f)
                        {
                            elementalMult = ((elementalMult - 1) * 1.5f) + 1;
                        }
                        break;
                }
            }

            //If enemy previously targeted is gone, aim for another enemy.
            if (attackEvent.enemyTargeted == null && timeline.activeEnemies.Count > 0)
            {
                if (targetedEnemy == null) { attackEvent.enemyTargeted = timeline.activeEnemies[0]; }
                else
                {
                    attackEvent.enemyTargeted = targetedEnemy;
                }
            }
            //If no enemies are left alive, cancel the attack.
            else if (timeline.activeEnemies.Count == 0) { return; }
            //Do the attack.
            attackEvent.enemyTargeted.TakeDamage(Mathf.CeilToInt(skill.intensity * bonusMult * perfectMult * elementalMult));
        }
        else if (skill.target == PlayerSkill.TargetType.AllEnemies)
        {
            foreach (Enemy e in timeline.activeEnemies) {
                elementalMult = ElementalMods.Modifier(skill.elementalType, e.elementalType);
                foreach (ItemObject i in equippedItems)
                {
                    switch (i.id)
                    {
                        case 1:
                            if (elementalMult > 1f)
                            {
                                elementalMult = ((elementalMult - 1) * 1.5f) + 1;
                            }
                            break;
                    }
                }

                e.TakeDamage(Mathf.CeilToInt(skill.intensity * bonusMult * perfectMult * elementalMult));
            }
            
        }
    }

    //Called by the skill selection buttons when clicked
    public void AttemptUseSkill(int skillSlotIndex)
    {
        if (timeline.combatActive && timeline.combatPause)
        {
            if (!isSkillUseable(skillSlotIndex)) { return; }

            PlayerSkill skill = equippedSkills[skillSlotIndex];

            int timeIndex = timeline.IndexFromTime(timeSpent + skill.prepTime);
            int timePassedIndex = timeline.IndexFromTime(timeSpent);

            Enemy enemyTargeted;

            if (targetedEnemy == null) { enemyTargeted = timeline.activeEnemies[0]; }
            else
            {
                enemyTargeted = targetedEnemy;
            }

            timeline.AddMarker(timeIndex, TimelineManager.EventType.PlayerAtk, null, null, skill, enemyTargeted);
            timeline.AddSkillBlock(timeIndex, timePassedIndex);
            timeSpent += skill.prepTime;
            energy -= skill.cost;
        }
    }

    public bool isSkillUseable(int skillSlotIndex)
    {
        PlayerSkill skill = equippedSkills[skillSlotIndex];

        if (timeline.combatTime - timeSpent < skill.prepTime) { Debug.LogWarning("Not enough time to use skill."); return false; }
        if (energy < skill.cost) { Debug.LogWarning("Not enough energy to use skill."); return false; }
        if (timeline.activeEnemies.Count < 0) { Debug.LogWarning("No enemies!"); return false; }

        return true;
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
        GameObject damageIndicatorObj = Instantiate(damageIndicatorPrefab, new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z), Quaternion.identity);
        DamageIndicator damageIndicator = damageIndicatorObj.GetComponent<DamageIndicator>();
        damageIndicator.untilRoundEnd = false;
        damageIndicator.SetDamage(dmg);

        if (dmg < 0)
        {
            //Heal Effects
        }
        else
        {
            //Damage Effects
            List<StatusEffectInstance> deletedEffects = new List<StatusEffectInstance>();
            foreach (StatusEffectInstance se in statEffects)
            {
                switch (se.data.id)
                {
                    case 0:
                        deletedEffects.Add(se);
                        break;
                }
            }
            RemoveEffectsFromList(deletedEffects);
            foreach (ItemObject i in equippedItems)
            {
                switch (i.id)
                {
                    case 0:
                        if (!damageTakenDuringCombat)
                        {
                            damageTakenDuringCombat = true;
                        }
                        break;
                }
            }

            switch (dmg)
            {
                case < 3: gAnim.StartCoroutine(gAnim.Shake(Vector3.right * 0.1f, 8f, 3f, -Vector3.one)); break;
                case < 5: gAnim.StartCoroutine(gAnim.Shake(Vector3.right * 0.2f, 8f, 3f, -Vector3.one)); break;
                case < 10: gAnim.StartCoroutine(gAnim.Shake(Vector3.right * 0.3f, 8f, 3f, -Vector3.one)); break;
                case >= 10: gAnim.StartCoroutine(gAnim.Shake(Vector3.right * 0.6f, 8f, 3f, -Vector3.one)); break;
            }

            foreach (ParticleSystem ps in damageParticles) { ps.Play(); }

            psAnim.hitOverrideTimer = 0.25f;
            if (hp <= 0) { Die(); }
        }
    }

    void Die()
    {
        Debug.Log("DEAD!");
        gameObject.SetActive(false);
        gdm.ChangeState(GameDataManager.State.GameOver);
    }

    void ParryLanded()
    {
        parryLanded = true;
        curParryCooldown = 0;
        foreach (ParticleSystem ps in parryParticles) { ps.Stop(); ps.Play(); }
        timeline.HitParticles();
        audioMan.PlaySound(1, AudioManager.SFXType.Effect, 1f, 1f);

        foreach (ItemObject i in equippedItems)
        {
            switch (i.id)
            {
                case 3:
                    hp = Mathf.Clamp(hp+=1, 0, mhp);
                    break;
                case 9:
                    memoryGain++;
                    break;
            }
        }
    }

    void PerfectLanded()
    {
        perfectLanded = true;
        curPerfectCooldown = 0;
        foreach (ParticleSystem ps in perfectParticles) { ps.Stop(); ps.Play(); }
        timeline.HitParticles();

        foreach (ItemObject i in equippedItems)
        {
            switch (i.id)
            {
                case 3:
                    hp = Mathf.Clamp(hp+=1, 0, mhp);
                    break;
                case 9:
                    memoryGain++;
                    break;
            }
        }
    }

    //Ran when a healing room is selected
    public void HealRoom()
    {
        hp = mhp;
    }

    public void GainLoseMemory(int amount)
    {
        StartCoroutine(MemoryGainLoseAnim(amount));
    }
    bool memoryAnimActive = false;
    IEnumerator MemoryGainLoseAnim(int amount)
    {
        memoryAnimActive = true;

        //Fade In
        memoryCount.transform.parent.gameObject.SetActive(true);
        float fadeProgress = 0;
        while (fadeProgress < 1)
        {
            fadeProgress += Time.deltaTime * 6f; if (fadeProgress > 1) { fadeProgress = 1; }
            for (int i = 0; i < memoryCount.transform.parent.childCount; i++)
            {
                if (memoryCount.transform.parent.GetChild(i).TryGetComponent<Image>(out Image img)) { img.color = new Color(img.color.r, img.color.g, img.color.b, fadeProgress); }
                if (memoryCount.transform.parent.GetChild(i).TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI txt)) { txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, fadeProgress); }
            }
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.25f);

        int amountLeft = amount;
        float waitTime;

        while (amountLeft > 0) { curMemories++; amountLeft--;
            waitTime = 0.1f / ((float)Mathf.Abs(amountLeft));
            if (waitTime > 0.1f) { waitTime = 0.1f; }
            yield return new WaitForSeconds(waitTime);
        }
        while (amountLeft < 0) { curMemories--; amountLeft++;
            waitTime = 0.1f / ((float)Mathf.Abs(amountLeft));
            if (waitTime > 0.1f) { waitTime = 0.1f; }
            yield return new WaitForSeconds(waitTime); 
        }

        yield return new WaitForSeconds(0.5f);

        //Fade Out
        while (fadeProgress > 0)
        {
            fadeProgress -= Time.deltaTime * 6f; if (fadeProgress < 0) { fadeProgress = 0; }
            for (int i = 0; i < memoryCount.transform.parent.childCount; i++)
            {
                if (memoryCount.transform.parent.GetChild(i).TryGetComponent<Image>(out Image img)) { img.color = new Color(img.color.r, img.color.g, img.color.b, fadeProgress); }
                if (memoryCount.transform.parent.GetChild(i).TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI txt)) { txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, fadeProgress); }
            }
            yield return new WaitForEndOfFrame();
        }
        memoryCount.transform.parent.gameObject.SetActive(false);

        for (int i = 0; i < memoryCount.transform.parent.childCount; i++)
        {
            if (memoryCount.transform.parent.GetChild(i).TryGetComponent<Image>(out Image img)) { img.color = new Color(img.color.r, img.color.g, img.color.b, 1); }
            if (memoryCount.transform.parent.GetChild(i).TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI txt)) { txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 1); }
        }

        memoryAnimActive = false;
        yield return null;
    }

    void RemoveEffectsFromList(List<StatusEffectInstance> deleted)
    {
        foreach (StatusEffectInstance se in deleted)
        {
            statEffects.Remove(se);
        }
    }

    public void UpdateSkillsByItem()
    {
        for (int si = 0; si < equippedSkills.Count; si++)
        {
            bool itemAltered = false;
            PlayerSkill modifiedSkill = Instantiate(equippedSkills[si]);

            foreach (ItemObject i in equippedItems)
            {
                switch (i.id)
                {
                    case 10:
                        if (equippedSkills[si].elementalType == ElementalTypes.Money)
                        {
                            modifiedSkill.elementalType = ElementalTypes.Corruption;
                            itemAltered = true;
                        }
                        break;
                    case 11:
                        if (equippedSkills[si].elementalType == ElementalTypes.Power)
                        {
                            modifiedSkill.elementalType = ElementalTypes.Corruption;
                            itemAltered = true;
                        }
                        break;
                    case 12:
                        if (equippedSkills[si].elementalType == ElementalTypes.Fame)
                        {
                            modifiedSkill.elementalType = ElementalTypes.Corruption;
                            itemAltered = true;
                        }
                        break;
                    case 14:
                        if (si == 0)
                        {
                            modifiedSkill.cost = skillData[modifiedSkill.id].cost - 1;
                            itemAltered = true;
                        }
                        else if (si == 2)
                        {
                            modifiedSkill.cost = skillData[modifiedSkill.id].cost + 1;
                            itemAltered = true;
                        }
                        break;
                }
            }

            if (itemAltered) {
                equippedSkills[si] = modifiedSkill;
                modifiedSkills.Add(modifiedSkill);
            }
        }
    }
}
