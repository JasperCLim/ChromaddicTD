using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField] GameObject[] enemyList;
    [SerializeField] int round;
    [SerializeField] private float timeBetweenWaves; // implement later

    private bool enemiesAlive;

    private void SpawnEnemies()
    {
        //UnityEngine.Debug.Log("spawning");
        StartCoroutine("ISpawnEnemies");
    }

    IEnumerator ISpawnEnemies()
    {
        for (int i = 0; i < round; i++)
        {
            int enemyIndex = UnityEngine.Random.Range(0,8); // sequential spawn, make random later

            GameObject newEnemy = Instantiate(enemyList[enemyIndex],this.transform.position,Quaternion.identity);
            yield return new WaitForSeconds(1f);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
            //round over, start a new one
            round++;
            SpawnEnemies();
        }
    }
}
