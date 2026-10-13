using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteAnimator : MonoBehaviour
{
    public SpriteRenderer sr;
    PlayerActionManager player;
    TimelineManager timeline;

    public List<Sprite> spriteList;

    int activeSprite;
    int activeSpritePriority;

    public float parryOverrideTimer = 0f;
    public float hitOverrideTimer = 0f;

    void Start()
    {
        player = GetComponent<PlayerActionManager>();
        timeline = GameObject.Find("Timeline").GetComponent<TimelineManager>();
        activeSprite = 0;
        activeSpritePriority = 0;
        sr.sprite = spriteList[0]; // set the sprite to idle.
    }
    // Called from player action manager
    public void UpdateVisuals(bool parryActive, bool parryLanded, bool perfectActive, bool perfectLanded)
    {
        activeSprite = 0;
        activeSpritePriority = 0;

        if (!timeline.combatPause && timeline.combatActive) { CheckTimeline(parryActive, parryLanded, perfectActive, perfectLanded); }
        else { parryOverrideTimer = 0; }

        sr.sprite = spriteList[activeSprite];
    }
    // Am I about to attack, lets check the timeline!!!
    void CheckTimeline(bool parryActive, bool parryLanded, bool perfectActive, bool perfectLanded)
    {
        PlayerSkill nextSkill = null; int nextSkillIndex = 999999;
        PlayerSkill prevSkill = null; int prevSkillIndex = -1;
        foreach (TimelineManager.TimelineEvent tEvent in timeline.eventList)
        {
            if (tEvent.eventType == TimelineManager.EventType.PlayerAtk)
            {
                if ((tEvent.timelineIndex > timeline.eventIndex) && (tEvent.timelineIndex < nextSkillIndex)) { nextSkill = tEvent.playerSkill; nextSkillIndex = tEvent.timelineIndex; }
                if ((tEvent.timelineIndex < timeline.eventIndex) && (tEvent.timelineIndex > prevSkillIndex)) { prevSkill = tEvent.playerSkill; prevSkillIndex = tEvent.timelineIndex; }
            }
        }

        if (parryActive) { activeSprite = 2; }
        else if (player.curParryCooldown > 0) { activeSprite = 1; }

        //WIP!! ADD PRIORITIES! ADD GETTING HIT! AND PARRIES!!!
        if (nextSkill != null)
        {
            float nextSkillTime = timeline.TimeFromIndex(nextSkillIndex);
            if (nextSkillTime - timeline.timePassed < nextSkill.prepTime / 4f) { activeSprite = 5; } //Attack Wind Up 2
            if (nextSkillTime - timeline.timePassed < nextSkill.prepTime / 2f) { activeSprite = 4; } //Attack Wind Up 1
        }
        if (prevSkill != null)
        {
            float prevSkillTime = timeline.TimeFromIndex(prevSkillIndex);
            if (timeline.timePassed - prevSkillTime < 0.1f && perfectLanded) { activeSprite = 6; } // Perfect Attack Active
            else if (timeline.timePassed - prevSkillTime < 0.15f) { activeSprite = 7; } // Attack Active
            else if (timeline.timePassed - prevSkillTime < 0.2f) { activeSprite = 8; } // Attack Follow Through 1
            else if (timeline.timePassed - prevSkillTime < 0.25f) { activeSprite = 9; } // Attack Follow Through 2
        }

        if (hitOverrideTimer > 0) { activeSprite = 10; hitOverrideTimer -= Time.deltaTime; }

        if (parryLanded) { activeSprite = 3; parryOverrideTimer = 0.1f; }
        else if (parryOverrideTimer > 0) { activeSprite = 3; parryOverrideTimer -= Time.deltaTime; }

    }
}
