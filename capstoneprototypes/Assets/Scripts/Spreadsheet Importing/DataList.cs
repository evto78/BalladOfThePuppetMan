using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DataList : ScriptableObject
{
    public List<EnemyData> allEnemies = new List<EnemyData>();
    public List<PlayerSkill> allPlayerSkills = new List<PlayerSkill>();
    public List<EncountersData> allEncounters = new List<EncountersData>();
}