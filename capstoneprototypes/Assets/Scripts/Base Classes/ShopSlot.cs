using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    ShopManager shopMan;
    float hoverTime = 0f;
    public int slot;

    public Color idleColor;
    public Color hoverColor;

    GameObject selectedSprite;

    private void Awake()
    {
        shopMan = GetComponentInParent<ShopManager>();
        selectedSprite = transform.GetChild(2).gameObject;
    }

    private void Update()
    {
        bool isHoveredOver = IsTouchingMouse(gameObject);

        if (isHoveredOver) { hoverTime += Time.deltaTime * 10f; }
        else { hoverTime -= Time.deltaTime * 10f; }
        hoverTime = Mathf.Clamp(hoverTime, 0f, 1f);

        transform.GetChild(0).GetComponent<Image>().color = Color.Lerp(idleColor, hoverColor, hoverTime);

        if (isHoveredOver && Input.GetMouseButtonUp(0)) { shopMan.AddRemoveToCart(slot); }

        selectedSprite.SetActive(shopMan.cart.Contains(shopMan.shopSlots[slot]));
    }

    public bool IsTouchingMouse(GameObject g)
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return g.GetComponentInChildren<Collider2D>().OverlapPoint(point);
    }
}
