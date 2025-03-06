using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField] GameObject[] enemyList;
    [SerializeField] int round;
    [SerializeField] private float timeBetweenWaves;

    [SerializeField] private float healthScaling;
    [SerializeField] private float moveScaling;
    List<Tuple<int, string[]>> spawnQueue = new List<Tuple<int, string[]>>();
    
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
        //     GameObject newEnemy = Instantiate(enemyList[enemyIndex],this.transform.position,Quaternion.identity);
        //         if (enemyIndex == 0) {
        //             newEnemy.GetComponent<EnemyScript>().Spawn(colors[UnityEngine.Random.Range(0,7)], healthScaling*(round-1), moveScaling*(round-1));
        //         }
        //         else {
        //             List<GameObject> layerind = newEnemy.GetComponent<LayeredEnemyScript>().getLayers();
        //             for (int k = 0; k < enemyIndex+1; k++) {
        //                 layerind[k].GetComponent<EnemyScript>().Spawn(colors[UnityEngine.Random.Range(0,7)], healthScaling*(round-1), moveScaling*(round-1));
        //             }
        //         }    
            //yield return new WaitForSeconds(1f);
        }
        StartCoroutine(ISpawnEnemies());
    }
    IEnumerator restTime()
    {
        resting = true;
        yield return new WaitForSeconds(timeBetweenWaves);
        resting = false;
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

    public List<Tuple<int, string[]>> getQueue()
    {
        return spawnQueue;
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
