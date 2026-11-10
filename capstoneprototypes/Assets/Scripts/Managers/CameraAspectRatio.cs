using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAspectRatio : MonoBehaviour
{
    void Start()
    {
        UpdateCameraScale();
    }

    void Update()
    {
        UpdateCameraScale();
    }

    void UpdateCameraScale()
    {
        float targetRatio = 16f / 9f;
        float windowRatio = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowRatio / targetRatio;

        Camera cam = GetComponent<Camera>();

        if (scaleHeight < 1.0f)
        {
            Rect newRect = cam.rect;

            newRect.width = 1f;
            newRect.height = scaleHeight;
            newRect.x = 0;
            newRect.y = (1f - scaleHeight) / 2f;

            cam.rect = newRect;
        }
        else
        {
            float scaleWidth = 1f / scaleHeight;

            Rect newRect = cam.rect;

            newRect.width = scaleHeight;
            newRect.height = 1f;
            newRect.x = (1f - scaleWidth) / 2f;
            newRect.y = 0;

            cam.rect = newRect;
        }
    }
}
