using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ShopManager : MonoBehaviour
{
    GameDataManager gdm;
    List<ItemObject> items;
    List<ItemObject> rareItems;
    public List<GameObject> disableUI;
    public List<Image> shopSlotSprites;
    public List<ItemObject> shopSlots;
    public List<ItemObject> cart;
    public float rareItemChance;
    public Sprite standardSprite;
    public Sprite rareSprite;
    int totalCost;
    public TextMeshProUGUI totalCostText;
    public Color costColor;
    public Color overcostColor;
    public void StartShop()
    {
        totalCost = 0;

        if (gdm == null) { gdm = GameObject.Find("GameDataManager").GetComponent<GameDataManager>(); }
        items = new List<ItemObject>();
        rareItems = new List<ItemObject>();
        cart = new List<ItemObject>();
        gdm.timeline.camGlide.isUp = false;

        foreach (ItemObject i in gdm.itemData)
        {
            if (i.isRare) { rareItems.Add(i); } else { items.Add(i); }
        }

        foreach (GameObject g in disableUI) { g.SetActive(false); }

        GenerateShop();
    }
    void GenerateShop()
    {
        shopSlots = new List<ItemObject>();

        for (int i = 0; i < shopSlotSprites.Count; i++)
        {
            ItemObject itemPicked;

            if (Random.Range(0,100) < rareItemChance) 
            { itemPicked = rareItems[Random.Range(0, rareItems.Count)]; shopSlotSprites[i].sprite = rareSprite; } 
            else { itemPicked = items[Random.Range(0, items.Count)]; shopSlotSprites[i].sprite = standardSprite; }

            shopSlots.Add(itemPicked);
            shopSlotSprites[i].transform.parent.GetChild(1).GetComponent<Image>().sprite = itemPicked.sprite;
            shopSlotSprites[i].transform.parent.GetComponent<ReadoutOnHover>().readoutText = "Cost: " + itemPicked.cost + " | " + itemPicked.itemName + " | " + itemPicked.itemDescription;
        }
    }
    private void Update()
    {
        totalCostText.text = totalCost.ToString();
        if (totalCost > 9999) { totalCostText.text = "ALOT"; }

        if (totalCost > gdm.player.curMemories) { totalCostText.color = overcostColor; }
        else { totalCostText.color = costColor; }
    }
    public void AttemptPurchaseItem(int slot)
    {
        ItemObject item = shopSlots[slot];
        if (gdm.player.curMemories >= item.cost)
        {
            gdm.player.GainLoseMemory(item.cost);
            gdm.player.equippedItems.Add(item);
        }
    }
    public void PurchaseItems()
    {
        if (cart.Count == 0) { return; }
        gdm.player.GainLoseMemory(-totalCost);
        foreach(ItemObject i in cart)
        {
            gdm.player.equippedItems.Add(i);
        }
        cart = new List<ItemObject>();
    }
    public void ExitShop()
    {
        PurchaseItems();

        gdm.ChangeState(GameDataManager.State.RoomSelection);
        foreach (GameObject g in disableUI) { g.SetActive(true); }
        gdm.timeline.camGlide.isUp = true;
    }
    public void AddRemoveToCart(int slot)
    {
        ItemObject item = shopSlots[slot];
        if (cart.Contains(item)) { cart.Remove(item); totalCost -= item.cost; }
        else { cart.Add(item); totalCost += item.cost; }
    }
}
