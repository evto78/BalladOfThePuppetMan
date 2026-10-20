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
            GameObject spawnedEnemy = Instantiate(enemies[validEnemies[i].id], spawns[i].position, Quaternion.identity);
            spawnedEnemy.GetComponentInChildren<SpriteRenderer>().sortingOrder = Mathf.Abs(Mathf.RoundToInt(spawnedEnemy.transform.localPosition.y));
        }
    }

    public void SpawnRandomEncounter()
    {
        int encounter = Random.Range(0, dl.allEncounters.Count);

        // !!! - TEMP FIX - !!!
        if (encounter == 6) { encounter = 0; }//IF Encounter would be the boss encounter (encounter 6) Set it to encounter 0 instead.

        SpawnEncounter(encounter);
    }
}