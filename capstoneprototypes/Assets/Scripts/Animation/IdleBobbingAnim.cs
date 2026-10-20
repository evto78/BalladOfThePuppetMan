using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBobbingAnim : MonoBehaviour
{
    Vector3 localAnchorPos;
    public Vector3 bobDir;
    public float bobSpeed;
    public float bobIntensity;
    public Vector2 speedMultRandRange;
    public AnimationCurve bobCurve;
    float counter = 0f;

    private void Start()
    {
        localAnchorPos = transform.localPosition;
        if (speedMultRandRange == Vector2.zero) { speedMultRandRange = Vector2.one; }
        bobSpeed *= Random.Range(speedMultRandRange.x, speedMultRandRange.y);
    }

    void Update()
    {
        transform.localPosition = localAnchorPos + (bobCurve.Evaluate(counter) * (bobDir * bobIntensity));

        counter += Time.deltaTime * bobSpeed;
        if (counter >= 1) { counter = 0; }
    }
}
