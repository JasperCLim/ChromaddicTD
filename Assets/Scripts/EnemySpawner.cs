using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] enemyList;
    [SerializeField] private int round = 1; // Start at round 1
    [SerializeField] private float timeBetweenWaves;

    private bool roundCompleted = false; // Start with round not completed
    private EconomySystem economySystem;

    // Add public getter for round
    public int GetRound()
    {
        return round;
    }

    private void SpawnEnemies()
    {
        roundCompleted = false;
        StartCoroutine("ISpawnEnemies");
    }

    IEnumerator ISpawnEnemies()
    {
        for (int i = 0; i < round; i++)
        {
            int enemyIndex = UnityEngine.Random.Range(0, Mathf.Min(8, enemyList.Length));
            GameObject newEnemy = Instantiate(enemyList[enemyIndex], transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1f);
        }
    }

    void Start()
    {
        // Find the economy system
        economySystem = FindFirstObjectByType<EconomySystem>();
        
        // Start spawning first round of enemies immediately
        SpawnEnemies();
    }

    void Update()
    {
        GameObject[] enemyLeft = GameObject.FindGameObjectsWithTag("Enemy");
        
        if (enemyLeft.Length > 0)
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
                Debug.Log($"Round {round} completed! Awarded {reward} gold.");
            }
            
            // Wait for the next round
            StartCoroutine(StartNextRound());
        }
    }
    
    IEnumerator StartNextRound()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        round++;
        SpawnEnemies();
    }
}
