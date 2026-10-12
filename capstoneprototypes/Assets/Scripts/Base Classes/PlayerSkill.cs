using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Skill", menuName = "Skill/Create New Player Skill")]
public class PlayerSkill : ScriptableObject
{
    [Header("Functional")]
    public int id;
    public int cost;
    public float prepTime;
    public enum TargetType { Self, SingleEnemy, AllEnemies }
    public TargetType target;
    public int intensity; //If the skill does damage, how much damage? If it heals, how much does it heal?
    public float perfectMultiplier = 1; //How much does a perfect hit affect this skill?
    public ElementalTypes elementalType;

    [Header("Flavor")]
    public string skillName;
    public string skillDescription;

    //Can the skill be perfected with a timed input?
    public bool perfectable;
}
