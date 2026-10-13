using System.Collections;
using System.Collections.Generic;
using TMPro; //can delete after
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public Transform[] spawns;
    public DataList dl;
    public GameObject[] enemies;

    List<GameObject> enemiesAlive = new List<GameObject>();


    public void SpawnEnemies()
    {
        RemoveAllEnemies();
        int encounterIndex = Random.Range(0, dl.allEncounters.Count);
        List<EnemyData> validEnemies = dl.allEncounters[encounterIndex].enemies;

        for (int i = 0; i < validEnemies.Count; i++)
        {
            enemiesAlive.Add(Instantiate(enemies[validEnemies[i].id], spawns[i].position, Quaternion.identity));
        }
    }

    public void RemoveAllEnemies()
    {
        if (enemiesAlive.Count > 0)
        {
            for (int i = 0; i < enemiesAlive.Count; i++)
            {
                Destroy(enemiesAlive[i]);
            }
            enemiesAlive.Clear();
        }
    }
}