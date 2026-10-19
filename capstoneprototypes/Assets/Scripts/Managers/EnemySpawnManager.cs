using System.Collections;
using System.Collections.Generic;
using TMPro; //can delete after
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public Transform[] spawns;
    public DataList dl;
    public GameObject[] enemies;

    void Awake()
    {
        //SpawnRandomEncounter();
    }

    public void SpawnEncounter(int encounterIndex)
    {
        List<EnemyData> validEnemies = dl.allEncounters[encounterIndex].enemies;

        for (int i = 0; i < validEnemies.Count; i++)
        {
            Instantiate(enemies[validEnemies[i].id], spawns[i].position, Quaternion.identity);
        }
    }

    public void SpawnRandomEncounter()
    {
        int encounter = Random.Range(0, dl.allEncounters.Count);
        SpawnEncounter(encounter);
    }
}