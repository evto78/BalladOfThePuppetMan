using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToCursor : MonoBehaviour
{
    public float speed;
    public bool pixelPerfect;
    public bool instant;
    void Update()
    {
        Vector3 flatPos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
        transform.position = Vector3.Lerp(transform.position, flatPos, Time.deltaTime * speed);
        if (instant) { transform.position = flatPos; }

        if (pixelPerfect)
        {
            Vector3 pixelPerfectPos = new Vector3(Mathf.RoundToInt(transform.position.x*10), Mathf.RoundToInt(transform.position.y*10), Mathf.RoundToInt(transform.position.z*10));
            pixelPerfectPos /= 10f;
            transform.position = pixelPerfectPos;
        }
    }
}
