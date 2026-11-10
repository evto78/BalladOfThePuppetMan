using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncountersData : ScriptableObject
{
    public int encounterID;
    public List<EnemyData> enemies = new List<EnemyData>();
    public List<float> offset = new List<float>();
    public Encounters.EncounterType encounterType;
}

public class Encounters
{
    public enum EncounterType
    {
        Regular,
        Scripted
    }
}