using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkerObject : MonoBehaviour
{
    public SpriteRenderer sr;
    public List<Sprite> markerSprites;
    public int baseSortingOrder = 40;
    public SpriteRenderer bgSr;

    public ReadoutOnHover myReadout;

    float standardYPos;
    public AnimationCurve bobbingCurve;

    public void SetType(TimelineManager.EventType newType)
    {
        standardYPos = transform.localPosition.y;

        bgSr.enabled = false;

        int typeIndex = newType.GetHashCode();

        sr.sprite = markerSprites[typeIndex];
        sr.sortingOrder = baseSortingOrder + typeIndex;

        myReadout.gameObject.SetActive(true);

        switch (typeIndex)
        {
            case 0: myReadout.readoutText = "An enemy will attack at this point in the timeline. Press RMB to parry at the right time!"; bgSr.enabled = true; break;
            case 1: myReadout.readoutText = "You will use a skill at this point in the timeline. Press LMB to perfect at the right time!"; bgSr.enabled = true; break;
            case 2: myReadout.readoutText = "At this point in the timeline, the timeline will speed up!"; break;
            case 3: myReadout.readoutText = "At this point in the timeline, the timeline will slow down!"; break;
            case 4: myReadout.gameObject.SetActive(false); break;
        }
    }

    public void UpdateBobbing(float bobProgress)
    {
        if (bobProgress < 0) { bobProgress = (16f * ((bobProgress * -1f))); }
        float modifiedProgress = bobbingCurve.Evaluate(bobProgress);
        float newYPos = Mathf.Lerp(standardYPos + 1, standardYPos, modifiedProgress);
        transform.localPosition = new Vector3(transform.localPosition.x, newYPos, transform.localPosition.z);
    }
}
