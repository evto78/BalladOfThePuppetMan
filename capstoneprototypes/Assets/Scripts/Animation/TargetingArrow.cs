using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingArrow : MonoBehaviour
{
    List<Transform> orbs = new List<Transform>();
    Vector3 startPos; Vector3 endPos;

    public AnimationCurve leanCurve;

    public AnimationCurve yCurve;
    public float yOffset;
    private void Start()
    {
        orbs.Add(transform.GetChild(0));
        orbs.Add(transform.GetChild(1));

        for (int i = 0; i < 30; i++) { NewOrb(); }
    }
    void NewOrb()
    {
        orbs.Add(Instantiate(orbs[orbs.Count - 2], orbs[orbs.Count-2].parent));
    }
    void Update()
    {
        startPos = transform.position;
        endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        endPos = new Vector3(endPos.x, endPos.y, 0);

        Vector3 modEndPos = endPos;
        float progress = 0f;
        for (int i = 0; i < orbs.Count; i++)
        {
            progress = (float)i / (float)(orbs.Count - 1);
            //modEndPos = new Vector3(endPos.x, endPos.y + (yOffset * yCurve.Evaluate(progress)), 0);
            //orbs[i].transform.position = Vector3.Lerp(startPos, modEndPos, progress);
            orbs[i].transform.position = new Vector3 (
                    Mathf.LerpUnclamped(startPos.x, modEndPos.x, 1f - leanCurve.Evaluate(1f - progress)),
                    Mathf.LerpUnclamped(startPos.y, modEndPos.y, leanCurve.Evaluate(progress)),
                    0 );
        }
    }
}
