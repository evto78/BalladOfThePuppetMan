using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "0_New Item", menuName = "Item/Create New Item")]
public class ItemObject : ScriptableObject
{
    [Header("Functional")]
    public int id;
    public int cost;
    public bool isRare;
    public Sprite sprite;

    [Header("Flavor")]
    public string itemName;
    public string itemDescription;
}
