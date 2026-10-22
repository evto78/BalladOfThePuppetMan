using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TargetingArrow : MonoBehaviour
{
    List<Transform> orbs = new List<Transform>();
    Transform pointer;
    Vector3 startPos; Vector3 endPos;

    public AnimationCurve leanCurve;
    private void Start()
    {
        pointer = transform.GetChild(0);
        orbs.Add(transform.GetChild(1));
        orbs.Add(transform.GetChild(2));

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
            if (i > orbs.Count - 4) { orbs[i].gameObject.SetActive(false); }
            orbs[i].transform.position = new Vector3 (
                    Mathf.LerpUnclamped(startPos.x, modEndPos.x, 1f - leanCurve.Evaluate(1f - progress)),
                    Mathf.LerpUnclamped(startPos.y, modEndPos.y, leanCurve.Evaluate(progress)),
                    0 );
        }
        pointer.transform.position = orbs[orbs.Count-3].position;
        pointer.transform.LookAt(modEndPos, -Vector3.forward);
    }
}
