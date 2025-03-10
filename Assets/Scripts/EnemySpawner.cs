using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] enemyList;
    [SerializeField] private int round = 1; // Start at round 1
    [SerializeField] private float timeBetweenWaves;
    [SerializeField] private float healthScaling;
    [SerializeField] private float moveScaling;
    private List<Tuple<int, string[]>> spawnQueue = new List<Tuple<int, string[]>>(); //< # of enemies, {layercolour1, layercolour2, etc} >

    private bool roundCompleted = false; // Start with round not completed
    private EconomySystem economySystem;
    
    private bool enemiesAlive;
    private bool resting;

    List<List<Tuple<int, string[]>>> uniqueWaves = new List<List<Tuple<int, string[]>>>
    {
        new List<Tuple<int, string[]>> // Wave 1
        {
            Tuple.Create(4, new[] { "red", "green", "yellow" }),   //< # of enemies, {layercolour1, layercolour2, etc} >
            Tuple.Create(2, new[] { "blue" })
        },
        new List<Tuple<int, string[]>> // Wave 2
        {
            Tuple.Create(-1, new[] { "white" }),
        },
        new List<Tuple<int, string[]>> // Wave 3
        {
            Tuple.Create(3, new[] { "cyan" }),
            Tuple.Create(2, new[] { "magenta", "yellow", "blue" })
        }
    };

    // Add public getter for round
    public int GetRound()
    {
        return round;
    }

    public List<Tuple<int, string[]>> getQueue()
    {
        return spawnQueue;
    }
    private void SpawnEnemiesPreset()
    {
        //UnityEngine.Debug.Log("spawningp");
        StartCoroutine("ISpawnEnemiesPreset");
    }


    IEnumerator ISpawnEnemiesPreset()
    {
        for (int i = 0; i < uniqueWaves[round-1].Count; i++) // For all in wave
        {
            int layersp = uniqueWaves[round-1][i].Item2.Length;
            
            for (int j = 0; j < uniqueWaves[round-1][i].Item1; j++) { // For all enemies in wave
                GameObject newEnemy = Instantiate(enemyList[layersp-1],this.transform.position,Quaternion.identity);
                if (layersp == 1) {
                    newEnemy.GetComponent<EnemyScript>().Spawn(uniqueWaves[round-1][i].Item2[0], healthScaling*(round-1), moveScaling*(round-1));
                }
                else {
                    List<GameObject> layerind = newEnemy.GetComponent<LayeredEnemyScript>().getLayers();
                    for (int k = 0; k < layersp; k++) { //Layer
                        layerind[k].GetComponent<EnemyScript>().Spawn(uniqueWaves[round-1][i].Item2[k], healthScaling*(round-1), moveScaling*(round-1));
                    }
                }
                yield return new WaitForSeconds(1f);
            }
        }
    }

    IEnumerator ISpawnEnemies()
    {

        for (int i = 0; i < spawnQueue.Count; i++)
        {
            int layers = spawnQueue[i].Item2.Length;

            for (int j = 0; j < spawnQueue[i].Item1; j++) { // For all enemies in wave
                GameObject newEnemy = Instantiate(enemyList[layers-1],this.transform.position,Quaternion.identity);
                if (layers == 1) {
                    newEnemy.GetComponent<EnemyScript>().Spawn(spawnQueue[i].Item2[0], healthScaling*(round-1), moveScaling*(round-1));
                }
                else {
                    List<GameObject> layerind = newEnemy.GetComponent<LayeredEnemyScript>().getLayers();
                    for (int k = 0; k < layers; k++) { //Layer
                        layerind[k].GetComponent<EnemyScript>().Spawn(spawnQueue[i].Item2[k], healthScaling*(round-1), moveScaling*(round-1));
                    }
                }
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private void SpawnEnemiesRand()
    {
        string[] colors = {"red", "green", "blue", "cyan", "yellow", "magenta", "white"};
        spawnQueue.Clear();
        for (int i = 0; i < round+1; i++) //spawn # enemies = to round+1
        {
            int enemyIndex = UnityEngine.Random.Range(0,3); //this is upper bounds exclusive?!?!?!
            string[] colorList = new string[enemyIndex+1];
            for (int j = 0; j <= enemyIndex; j++) {
                colorList[j] = colors[UnityEngine.Random.Range(0,7)];
            }
            spawnQueue.Add(Tuple.Create(1, colorList));
        }
        StartCoroutine(ISpawnEnemies());
    }

    IEnumerator restTime()
    {
        resting = true;
        yield return new WaitForSeconds(timeBetweenWaves);
        resting = false;
        roundCompleted = false;
        if (round <= uniqueWaves.Count) {
            if (uniqueWaves[round-1][0].Item1==-1) {
                SpawnEnemiesRand();
            }
            else {
                spawnQueue = new List<Tuple<int, string[]>>(uniqueWaves[round-1]);
                StartCoroutine("ISpawnEnemies");
            }
        }
        else {
            SpawnEnemiesRand();
        }
        
    }
    /// <summary>
    /// //
    // /// </summary>
    // private void SpawnEnemies()
    // {
    //     roundCompleted = false;
    //     StartCoroutine("ISpawnEnemies");
    // }

    // IEnumerator ISpawnEnemies()
    // {
    //     for (int i = 0; i < round; i++)
    //     {
    //         int enemyIndex = UnityEngine.Random.Range(0, Mathf.Min(8, enemyList.Length));
    //         GameObject newEnemy = Instantiate(enemyList[enemyIndex], transform.position, Quaternion.identity);
    //         yield return new WaitForSeconds(1f);
    //     }
    // }

    void Start()
    {
        // Find the economy system
        economySystem = FindFirstObjectByType<EconomySystem>();
        
        // Start spawning first round of enemies immediately
        spawnQueue = new List<Tuple<int, string[]>>(uniqueWaves[round-1]);
        StartCoroutine("ISpawnEnemies");
    }

    void Update()
    {
        GameObject[] enemyLeft = GameObject.FindGameObjectsWithTag("Enemy");
        
        if (enemyLeft.Length > 0 | resting is true)
        {
            // Round in progress
            return;
        }
        else if (!roundCompleted)
        {
            // Round just completed
            roundCompleted = true;
            
            // Award currency for completing the round
            if (economySystem != null)
            {
                int reward = economySystem.GetRoundReward();
                economySystem.AddCurrency(reward);
                UnityEngine.Debug.Log($"Round {round} completed! Awarded {reward} gold.");
            }
            
            // Wait for the next round
            round++;
            StartCoroutine("restTime");
        }
    }
}

