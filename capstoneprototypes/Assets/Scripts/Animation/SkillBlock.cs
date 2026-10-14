using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBlock : MonoBehaviour
{
    public Transform leftBorder;
    public Transform rightBorder;
    public Transform middleSegment;

    List<SpriteRenderer> srs;

    bool isHoveredOver;
    float hoverTime;
    public Color idleColor;
    public Color hoverColor;

    private void Awake()
    {
        srs = new List<SpriteRenderer>();
        srs.AddRange(GetComponentsInChildren<SpriteRenderer>());
        hoverTime = 0f;
    }

    public void SetUpBlock(Vector3 initialPos, Vector3 endPos)
    {
        transform.localPosition = initialPos;
        float relEnd = endPos.x - initialPos.x;
        rightBorder.transform.localPosition = new Vector3(relEnd, rightBorder.transform.localPosition.y, 0);
        middleSegment.transform.localPosition = new Vector3(relEnd / 2f, middleSegment.transform.localPosition.y, 0);
        middleSegment.transform.localScale = new Vector3((relEnd * 10f)-40f, middleSegment.transform.localScale.y, middleSegment.transform.localScale.z);
    }

    private void Update()
    {
        isHoveredOver = IsTouchingMouse(gameObject);

        if (isHoveredOver) { hoverTime += Time.deltaTime * 10f; }
        else { hoverTime -= Time.deltaTime * 10f; }
        hoverTime = Mathf.Clamp(hoverTime, 0f, 1f);

        foreach (SpriteRenderer sr in srs)
        {
            sr.color = Color.Lerp(idleColor, hoverColor, hoverTime);
        }
    }

    public bool IsTouchingMouse(GameObject g)
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return g.GetComponentInChildren<Collider2D>().OverlapPoint(point);
    }
}
