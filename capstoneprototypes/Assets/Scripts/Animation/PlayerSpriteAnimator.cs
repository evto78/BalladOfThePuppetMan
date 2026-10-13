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

        CheckTimeline(perfectActive, perfectLanded);

        sr.sprite = spriteList[activeSprite];
    }
    // Am I about to attack, lets check the timeline!!!
    void CheckTimeline(bool perfectActive, bool perfectLanded)
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

        //WIP!! ADD PRIORITIES AND ALSO ATTACK WIND UPS!
        //parries also need to be added.
        if (prevSkill != null)
        {
            float prevSkillTime = timeline.TimeFromIndex(prevSkillIndex);
            if (timeline.timePassed - prevSkillTime < 0.05f && perfectLanded) { activeSprite = 6; } // Perfect Attack Active
            else if (timeline.timePassed - prevSkillTime < 0.1f) { activeSprite = 7; } // Attack Active
            else if (timeline.timePassed - prevSkillTime < 0.15f) { activeSprite = 8; } // Attack Follow Through 1
            else if (timeline.timePassed - prevSkillTime < 0.2f) { activeSprite = 9; } // Attack Follow Through 2
        }
        if (nextSkill != null)
        {

        }
    }
}
