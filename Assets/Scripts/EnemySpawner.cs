using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField] GameObject[] enemyList;
    [SerializeField] int round;
    [SerializeField] private float timeBetweenWaves; // implement later
    [SerializeField] private GameManager gameManager;

    private List<GameObject> currentWave = new List<GameObject>();
    private List<GameObject> nextWave = new List<GameObject>();

    private GameObject AddEnemyToWave()
    {
        GameObject newEnemy;
        int enemyIndex = UnityEngine.Random.Range(0,enemyList.Length-1);
        newEnemy = enemyList[enemyIndex];
        return newEnemy;
    }

    private void SpawnEnemies()
    {
        //UnityEngine.Debug.Log("spawning");
        currentWave = nextWave;
        List<GameObject> newNextWave = new List<GameObject>();
        for (int i=0; i<round+1;i++)
        {
            newNextWave.Add(AddEnemyToWave());
        }
        nextWave = newNextWave;
        string currentWaveString = string.Join(", ", currentWave);
        string nextWaveString = string.Join(", ", nextWave);
        UnityEngine.Debug.Log("Round:" + round);
        UnityEngine.Debug.Log("Current Wave:" + currentWaveString);
        UnityEngine.Debug.Log("Next Wave:" + nextWaveString);
        StartCoroutine("ISpawnEnemies");
    }

    IEnumerator ISpawnEnemies()
    {
        
        for (int i = 0; i < currentWave.Count; i++)
        {

            GameObject newEnemy = Instantiate(currentWave[i],this.transform.position,Quaternion.identity);
            UnityEngine.Debug.Log("Spawned enemy: " + newEnemy);
            yield return new WaitForSeconds(1f);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextWave.Add(AddEnemyToWave());
        string nextWaveString = string.Join(", ", nextWave);
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] enemyLeft = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemyLeft.Length > 0)
        {
            // round in progress
            return;
        }
        else
        {
            //round over, start a new one if the game hasn't ended
            if (!gameManager.gameEnded)
            {
                round++;
                SpawnEnemies();
            }
        }
    }
}
