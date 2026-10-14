using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncountersData : ScriptableObject
{
    public int encounterID;
    public List<EnemyData> enemies = new List<EnemyData>();
    public List<float> offset = new List<float>();
}
