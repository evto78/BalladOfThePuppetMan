using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpriteAnimator : MonoBehaviour
{
    [Header("Animations")]
    public List<Sprite> idleAnim;
    public List<Sprite> preAttackAnim;
    public List<Sprite> postAttackAnim;
    public List<Sprite> hurtAnim;

    [Header("References")]
    public SpriteRenderer sr;
    GameDataManager gdm;

    float hurtTimer;
    float overrideIdleTimer;
    float idleCounter;
    List<Sprite> activeAnim;
    int activeAnimIndex;

    private void Start()
    {
        gdm = GameObject.Find("GameDataManager").GetComponent<GameDataManager>();
        sr.sprite = idleAnim[0];
        overrideIdleTimer = 0f;
        hurtTimer = 0f;
        activeAnimIndex = 0;
        activeAnim = idleAnim;
    }
    //Called from the enemy script in Update Visuals.
    public void AnimUpdate()
    {
        //overrideIdleTimer -= Time.deltaTime * gdm.timeline.combatSpeed;
        if (overrideIdleTimer <= 0) 
        { 
            overrideIdleTimer = 0f; 
            activeAnim = idleAnim;
            activeAnimIndex = Mathf.RoundToInt(idleCounter);

            if (hurtTimer > 0)
            {
                activeAnim = hurtAnim;
                activeAnimIndex = Mathf.RoundToInt((hurtTimer * 2f) * (hurtAnim.Count - 1));
            }
        }
        else
        {
            if (activeAnim == postAttackAnim)
            {
                if (hurtTimer > 0)
                {
                    activeAnim = hurtAnim;
                    activeAnimIndex = Mathf.RoundToInt((hurtTimer * 2f) * (hurtAnim.Count - 1));
                }
            }
        }

        //Set sprite to proper anim frame
        sr.sprite = activeAnim[activeAnimIndex];
    }
    private void LateUpdate()
    {
        idleCounter += Time.deltaTime * gdm.timeline.combatSpeed;
        if (idleCounter > idleAnim.Count - 1) { idleCounter -= idleAnim.Count - 1; }

        if (overrideIdleTimer <= 0)
        {
            hurtTimer -= Time.deltaTime * gdm.timeline.combatSpeed;
        }
        else if (activeAnim == postAttackAnim)
        {
            hurtTimer -= Time.deltaTime * gdm.timeline.combatSpeed * 2f; //If hurt anim is overriding the post attack anim, it finishes twice as fast.
        }
        if (hurtTimer <= 0) { hurtTimer = 0; }
    }
    public void UpdateAnimationByTime(float time, Enemy.Action action)
    {
        overrideIdleTimer = 0f;
        float animProgress = 0f;
        float animStart = 0f;
        float animEnd = 0f;

        //Find fitting animation:
        if (time < action.delayBeforeAction && time > action.delayBeforeAction - action.animWindupStart)
        {
            activeAnim = preAttackAnim;

            animStart = action.delayBeforeAction - action.animWindupStart;
            animEnd = action.delayBeforeAction;
        }
        else if (time < action.delayBeforeAction + action.animWinddownEnd)
        {
            activeAnim = postAttackAnim;

            animStart = action.delayBeforeAction;
            animEnd = action.delayBeforeAction + action.animWinddownEnd;
        }
        else { return; } //No proper animation found?
        overrideIdleTimer = animEnd - time;

        //Find the anim progress
        animProgress = time / animEnd;
        activeAnimIndex = Mathf.RoundToInt(animProgress * (activeAnim.Count - 1));
    }
    public void HurtAnim()
    {
        hurtTimer = 0.5f;
    }
}
