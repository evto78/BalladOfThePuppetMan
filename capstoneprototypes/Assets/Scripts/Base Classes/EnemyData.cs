using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "0 - New Enemy", menuName = "Enemy/Create New Enemy")]
public class EnemyData : ScriptableObject
{
    [Header("Functional")]
    public int id;
    public int maxHP;
    public ElementalTypes type;

    [Header("Flavor")]
    public string enemyName;
    public string enemyDescription;
}
