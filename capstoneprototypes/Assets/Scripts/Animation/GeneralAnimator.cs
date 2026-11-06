using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralAnimator : MonoBehaviour
{
    public bool isLocal;

    Vector3 startPos;
    Quaternion startRot;
    Vector3 startScale;

    private void Awake()
    {
        startPos = GetPos();
        startRot = GetRot();
        startScale = GetScale();
    }

    public void UpdateStartPos()
    {
        if (isLocal)
        {
            startPos = transform.localPosition;
            startRot = transform.localRotation;
            startScale = transform.localScale;
        }
        else
        {
            startPos = transform.position;
            startRot = transform.rotation;
            startScale = transform.localScale;
        }
    }

    Vector3 GetPos()
    { if (isLocal) { return transform.localPosition; } else { return transform.position; } }
    Quaternion GetRot()
    { if (isLocal) { return transform.localRotation; } else { return transform.rotation; } }
    Vector3 GetScale()
    { return transform.localScale; }

    public void ResetAll()
    {
        StopAllCoroutines();
         
        if (isLocal)
        {
            transform.localPosition = startPos;
            transform.localRotation = startRot;
            transform.localScale = startScale;
        }
        else
        {
            transform.position = startPos;
            transform.rotation = startRot;
            transform.localScale = startScale;
        }
    }

    public IEnumerator GlideToPos(Vector3 newPos, float seconds, bool relativePos)
    {
        Vector3 curPos = GetPos();
        Vector3 tarPos = newPos;
        if (relativePos) { tarPos += curPos; }
        float timer = seconds;
        Vector3 lerpPos;

        while (timer > 0)
        {
            lerpPos = Vector3.Lerp(tarPos, curPos, timer/seconds);
            if (isLocal) { transform.localPosition = lerpPos; } else { transform.position = lerpPos; }
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
    public IEnumerator TurnToRot(Quaternion newRot, float seconds, bool relativeRot)
    {
        Quaternion curRot = GetRot();
        Quaternion tarRot = newRot;
        if (relativeRot) { tarRot *= curRot; }
        float timer = seconds;
        Quaternion lerpRot;

        while (timer > 0)
        {
            lerpRot = Quaternion.Lerp(tarRot, curRot, timer / seconds);
            if (isLocal) { transform.localRotation = lerpRot; } else { transform.rotation = lerpRot; }
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
    public IEnumerator LerpScale(Vector3 newScale, float seconds, bool relativeScale)
    {
        Vector3 curScale = GetScale();
        Vector3 tarScale = newScale;
        if (relativeScale) { tarScale += curScale; }
        float timer = seconds;
        Vector3 lerpScale;

        while (timer > 0)
        {
            lerpScale = Vector3.Lerp(tarScale, curScale, timer / seconds);
            transform.localScale = lerpScale;
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
    //Recursively move in this direction every frame.
    public IEnumerator ContinuousGlide(Vector3 dir)
    {
        if (isLocal) { transform.localPosition += dir * Time.deltaTime; } 
        else { transform.position += dir * Time.deltaTime; }
        yield return StartCoroutine(ContinuousGlide(dir));
    }
    //Recursively turn in this direction every frame.
    public IEnumerator ContinuousTurn(Vector3 dir)
    {
        if (isLocal) { transform.localEulerAngles += dir * Time.deltaTime; }
        else { transform.eulerAngles += dir * Time.deltaTime; }
        yield return StartCoroutine(ContinuousTurn(dir));
    }
    //Recursively move in this direction every frame.
    public IEnumerator ContinuousScale(Vector3 scale)
    {
        transform.localScale += scale * Time.deltaTime;
        yield return StartCoroutine(ContinuousScale(scale));
    }
    //Recursively bob according to dir every frame.
    public IEnumerator ContinuousBobbing(Vector3 dir, float speed, Vector3 prevStartPos)
    {
        // -Vector3.one is a standin for no value given.
        if (prevStartPos == -Vector3.one) { prevStartPos = GetPos(); }
        float progress = 0f;
        float modProgress = 0f;

        while (progress < 2f) 
        {
            progress += Time.deltaTime * speed;
            modProgress = progress * (Mathf.PI * 2);
            if (isLocal) { transform.localPosition = prevStartPos + (dir * (Mathf.Sin(modProgress))); }
            else { transform.position = prevStartPos + (dir * (Mathf.Sin(modProgress))); }
            yield return new WaitForEndOfFrame();
        }

        yield return StartCoroutine(ContinuousBobbing(dir, speed, prevStartPos));
    }
    //Recursively shake according to dir every frame, decays over time until done.
    public IEnumerator Shake(Vector3 dir, float speed, float decayRate, Vector3 prevStartPos)
    {
        // -Vector3.one is a standin for no value given.
        if (prevStartPos == -Vector3.one) { prevStartPos = GetPos(); }
        float progress = 0f;
        float modProgress = 0f;

        while (progress < 2f)
        {
            progress += Time.deltaTime * speed;
            modProgress = progress * (Mathf.PI * 2);
            if (isLocal) { transform.localPosition = prevStartPos + (dir * (Mathf.Sin(modProgress))); }
            else { transform.position = prevStartPos + (dir * (Mathf.Sin(modProgress))); }
            yield return new WaitForEndOfFrame();
        }

        dir /= decayRate;
        
        if (dir.magnitude > 0.001f) { yield return StartCoroutine(Shake(dir, speed, decayRate, prevStartPos)); }
        else { yield return null; }
    }
}
