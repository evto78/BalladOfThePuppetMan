using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverAnim : MonoBehaviour
{
    Image img;
    public List<Sprite> animSheet;
    public List<GameObject> enableOnFinish;
    float timer;
    public float animSpeed = 5f;
    int frame = 0;
    bool playAnim = false;
    void Start()
    { img = GetComponent<Image>(); ResetAnim(); }
    public void PlayAnim() { img.enabled = true; ; playAnim = true; timer = 1; frame = 0; img.sprite = animSheet[frame]; }
    void Update()
    {
        if (playAnim)
        {
            if (timer <= 0) 
            { 
                timer = 1; 
                frame++; 
                if (frame == animSheet.Count) { playAnim = false; DonePlaying(); }
                else { img.sprite = animSheet[frame]; }
            }
            timer -= Time.deltaTime * animSpeed;
        }
    }
    void DonePlaying()
    {
        foreach(GameObject obj in enableOnFinish)
        {
            obj.SetActive(true);
        }
    }
    public void ResetAnim()
    {
        foreach (GameObject obj in enableOnFinish)
        {
            obj.SetActive(false);
        }
        img.enabled = false; ; playAnim = false; timer = 1; frame = 0; img.sprite = animSheet[frame];
    }
}
