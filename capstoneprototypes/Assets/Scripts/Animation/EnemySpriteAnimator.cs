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
    public float idleCounter;
    List<Sprite> activeAnim;
    public int activeAnimIndex;

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
            activeAnimIndex = Mathf.FloorToInt(idleCounter);

            if (hurtTimer > 0)
            {
                activeAnim = hurtAnim;
                activeAnimIndex = Mathf.FloorToInt((hurtTimer * 2f) * (hurtAnim.Count - 1));
            }
        }
        else
        {
            if (activeAnim == postAttackAnim)
            {
                if (hurtTimer > 0)
                {
                    activeAnim = hurtAnim;
                    activeAnimIndex = Mathf.FloorToInt((hurtTimer * 2f) * (hurtAnim.Count - 1));
                }
            }
        }

        //Set sprite to proper anim frame
        activeAnimIndex = Mathf.Clamp(activeAnimIndex, 0, activeAnim.Count-1);
        sr.sprite = activeAnim[activeAnimIndex];
    }
    private void LateUpdate()
    {
        idleCounter += Time.deltaTime * gdm.timeline.combatSpeed;
        if (idleCounter > idleAnim.Count - 1) 
        { 
            idleCounter -= Mathf.Clamp(idleAnim.Count - 1, 1, idleAnim.Count); 
        }

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
            animStart = action.delayBeforeAction - action.animWindupStart;
            animEnd = action.delayBeforeAction;

            if (time >= animStart && time <= animEnd) { activeAnim = preAttackAnim; }
            else { return; }
        }
        else if (time < action.delayBeforeAction + action.animWinddownEnd)
        {
            animStart = action.delayBeforeAction;
            animEnd = action.delayBeforeAction + action.animWinddownEnd;

            if (time >= animStart && time <= animEnd) { activeAnim = postAttackAnim; }
            else { return; }
        }
        else { return; } //No proper animation found?
        overrideIdleTimer = animEnd - time;

        //Find the anim progress
        animProgress = (time - animStart) / (animEnd - animStart);
        activeAnimIndex = Mathf.FloorToInt(animProgress * (activeAnim.Count - 1));

        Debug.Log("INFO:");
        if (activeAnim == idleAnim) { Debug.Log("Idle Anim. Count: " + idleAnim.Count); }
        if (activeAnim == preAttackAnim) { Debug.Log("PreAtk Anim. Count: " + preAttackAnim.Count); }
        if (activeAnim == postAttackAnim) { Debug.Log("PostAtk Anim. Count: " + postAttackAnim.Count); }
        if (activeAnim == hurtAnim) { Debug.Log("Hurt Anim. Count: " + hurtAnim.Count); }
        Debug.Log("Time: " + time);
        Debug.Log(animStart + " / " + animEnd);
        Debug.Log("Index: " + activeAnimIndex);
    }
    public void HurtAnim()
    {
        hurtTimer = 0.5f;
    }
}
