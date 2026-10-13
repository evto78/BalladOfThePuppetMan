using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteAnimator : MonoBehaviour
{
    public SpriteRenderer sr;

    public List<Sprite> spriteList;

    int activeSprite;
    int activeSpritePriority;

    void Start()
    {
        sr.sprite = spriteList[0]; // set the sprite to idle.
    }
    // Called from player action manager
    public void UpdateVisuals()
    {

    }
}
