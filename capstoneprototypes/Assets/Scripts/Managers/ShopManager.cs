using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    GameDataManager gdm;
    List<ItemObject> items;
    List<ItemObject> rareItems;
    public List<GameObject> disableUI;
    public List<Image> shopSlotSprites;
    public List<ItemObject> shopSlots;
    public float rareItemChance;
    public Sprite standardSprite;
    public Sprite rareSprite;
    void Start()
    {
        if (gdm == null) { gdm = GameObject.Find("GameDataManager").GetComponent<GameDataManager>(); }
        items = new List<ItemObject>();
        rareItems = new List<ItemObject>();
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
    public void AttemptPurchaseItem(int slot)
    {

    }
    public void ExitShop()
    {
        gdm.ChangeState(GameDataManager.State.RoomSelection);
        foreach (GameObject g in disableUI) { g.SetActive(true); }
        gdm.timeline.camGlide.isUp = true;
    }
}
