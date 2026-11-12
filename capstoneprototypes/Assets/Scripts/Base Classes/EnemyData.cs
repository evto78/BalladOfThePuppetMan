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
    public List<Enemy.AttackPattern> attackPatterns = new List<Enemy.AttackPattern>();
    public List<int> patternByRound = new List<int>();

    [Header("Flavor")]
    public string enemyName;
    public string enemyDescription;
    public Sprite enemySprite;
}
