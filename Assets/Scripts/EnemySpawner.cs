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
    private bool resting;
//    GameObject[] enemies = AssetDatabase.FindAssets("t:prefab", new string[] {"Assets/Prefabs/Enemies"});
    

    int[][] uniqueWaves = {
        new int[] {-1,2,3},
        new int[] {-1,4},
        new int[] {3,4,4,5}
    };


    private void SpawnEnemiesPreset()
    {
        //UnityEngine.Debug.Log("spawning");
        StartCoroutine("ISpawnEnemiesPreset");
    }

    private void SpawnEnemiesRand()
    {
    //UnityEngine.Debug.Log("spawning");
    StartCoroutine("ISpawnEnemiesRand");
    }

    public int roundNum()
    {
        return round;
    }

    IEnumerator ISpawnEnemiesPreset()
    {
        for (int i = 0; i < uniqueWaves[round].Length; i++)
        {
            for (int j = 0; j < uniqueWaves[round][i]; j++) {
                GameObject newEnemy = Instantiate(enemyList[i],this.transform.position,Quaternion.identity);
                yield return new WaitForSeconds(1f);
        
            }}
    }
    IEnumerator ISpawnEnemiesRand()
    {
        for (int i = 0; i < round; i++)
        {
            int enemyIndex = UnityEngine.Random.Range(0,41);

            GameObject newEnemy = Instantiate(enemyList[enemyIndex],this.transform.position,Quaternion.identity);
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator restTime()
    {
        resting = true;
        yield return new WaitForSeconds(timeBetweenWaves);
        resting = false;
        if (round <= uniqueWaves.Length) {
            if (uniqueWaves[round][0]==-1) {
                SpawnEnemiesRand();
            }
            else {
                SpawnEnemiesPreset();
            }
        }
        else {
            SpawnEnemiesRand();
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
        if (enemyLeft.Length > 0 | resting is true)
        {
            // round or rest in progress
            return;
        }
        else
        {
            //round over, start a new one
            round++;
            StartCoroutine("restTime");
        }
    }
}
