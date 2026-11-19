using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPulse : MonoBehaviour
{
    Light l; public AnimationCurve curve; public float speed; float intesity; float timer;
    void Start()
    {
        timer = Random.Range(0f, 1f);
        l = GetComponent<Light>(); intesity = l.intensity;
    }
    void Update()
    {
        timer += Time.deltaTime * speed; if (timer > 1) { timer = 0; }
        l.intensity = curve.Evaluate(timer) * intesity;
    }
}
