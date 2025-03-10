using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] enemyList;
    [SerializeField] private int round = 1;
    [SerializeField] private float timeBetweenWaves;
    [SerializeField] private float healthScaling; //linear * round. if base health is 5, on round 3 it will be 5 + 3 * healthScaling
    [SerializeField] private float moveScaling; // ditto

    private EconomySystem economySystem; // reference the Economy object
    public GameManager gm; // reference the Game Manager object
    private bool firstRound = true;
    List<Tuple<int, string[]>> spawnQueue = new List<Tuple<int, string[]>>(); //< # of enemies, {layercolour1, layercolour2, etc} >
                                                                                // -1 enemies to skip wave and generate randomly
    
    private bool enemiesAlive;
    private bool resting;

    public int GetRound()
    {
        return round;
    }


    List<List<Tuple<int, string[]>>> uniqueWaves = new()
    {
        new List<Tuple<int, string[]>> // Wave 1: RGY, RGY, RGY, RGY, B, B
        {
            Tuple.Create(1, new[] { "red", "green", "yellow" }),   //< # of enemies, {layercolour1, layercolour2, etc} >
            Tuple.Create(1, new[] { "blue" })
        }
    };

/*


    List<List<Tuple<int, string[]>>> uniqueWaves = new List<List<Tuple<int, string[]>>>
    {
        new List<Tuple<int, string[]>> // Wave 1: RGY, RGY, RGY, RGY, B, B
        {
            Tuple.Create(4, new[] { "red", "green", "yellow" }),   //< # of enemies, {layercolour1, layercolour2, etc} >
            Tuple.Create(2, new[] { "blue" })
        },
        new List<Tuple<int, string[]>> // Wave 2: call the randon enemy spawner
        {
            Tuple.Create(-1, new[] { "white" }),
        },
        new List<Tuple<int, string[]>> // Wave 3: C, C, C, MYB, MYB
        {
            Tuple.Create(3, new[] { "cyan" }),
            Tuple.Create(2, new[] { "magenta", "yellow", "blue" })
        }
    };
*/

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
            
            for (int j = 0; j < uniqueWaves[round-1][i].Item1; j++) { // For all enemies in type
                GameObject newEnemy = Instantiate(enemyList[layersp-1],this.transform.position,Quaternion.identity); //create enemy
                if (layersp == 1) {
                    newEnemy.GetComponent<EnemyScript>().Spawn(uniqueWaves[round-1][i].Item2[0], healthScaling*(round-1), moveScaling*(round-1)); //set move/health scaling
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

                if (layers == 1) // single layer enemy
                {
                    newEnemy.GetComponent<EnemyScript>().Spawn(spawnQueue[i].Item2[0], healthScaling*(round-1), moveScaling*(round-1));
                }
                else // multi layer enemy
                {
                    List<GameObject> layerind = newEnemy.GetComponent<LayeredEnemyScript>().getLayers();
                    for (int k = 0; k < layers; k++) { // For each layer
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
            // determine how many layers each enemy will have (1-3)
            int enemyIndex = UnityEngine.Random.Range(0,3); //Random.Range is upper bounds exclusive
            string[] colorList = new string[enemyIndex+1]; // string array to hold list of colored layers
            for (int j = 0; j <= enemyIndex; j++) {
                colorList[j] = colors[UnityEngine.Random.Range(0,7)]; // pick a random color for each layer
            }
            spawnQueue.Add(Tuple.Create(1, colorList)); // add the tuple representing the enemy to the spawn queue
        }
        StartCoroutine(ISpawnEnemies());
    }

    IEnumerator restTime()
    {
        resting = true;
        yield return new WaitForSeconds(timeBetweenWaves); 
        resting = false;
        if (round <= uniqueWaves.Count) { //if there are still preset waves not done
            if (uniqueWaves[round-1][0].Item1==-1) { //if preset is skipped
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
        // Find the economy system
        economySystem = FindFirstObjectByType<EconomySystem>();
    }

    void Update()
    {
        if (gm.gameEnded)
        {
            return;
        }
        GameObject[] enemyLeft = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemyLeft.Length > 0 | resting is true)
        {
            // round or rest in progress
            return;
        }
        else
        {
            if (firstRound)
            {
                firstRound = false;
            }
            else
            {
                // Award currency for completing the round
                if (economySystem != null)
                {
                    int reward = economySystem.GetRoundReward();
                    economySystem.AddCurrency(reward);
                    Debug.Log($"Round {round} completed! Awarded {reward} gold.");
                }
                
                //round over, start a new one
                round++;
            }
            StartCoroutine("restTime");
        }
    }
}
