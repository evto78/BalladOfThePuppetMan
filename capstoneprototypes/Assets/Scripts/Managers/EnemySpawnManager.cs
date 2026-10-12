using System.Collections;
using System.Collections.Generic;
using TMPro; //can delete after
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public GameObject[] spawnPoints; //Used for final spawning logic
    public DataList dl;
    public GameObject enemyPrefab; //Used for final spawning logic

    //Only used for testing features delete them
    public TextMeshProUGUI[] enemyName;
    public GameObject[] enemy;

    public void SpawnEnemies()
    {
        int encounterIndex = Random.Range(0, dl.allEncounters.Count);
        List<EnemyData> validEnemies = dl.allEncounters[encounterIndex].enemies;

        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].SetActive(false);
        }

        for (int i = 0; i < validEnemies.Count; i++)
        {
            //Tests if encounter spawning logic works
            enemy[i].SetActive(true);
            enemyName[i].text = validEnemies[i].enemyName;
        }
    }
}