using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class LayeredEnemyScript : MonoBehaviour
{

    [SerializeField] private EnemyScript[] enemyList;

    public void DecreaseEachPriority()
    {
        int enemyCounter = 0; // see how many enemies are left alive


        foreach (EnemyScript es in enemyList)
        {

            enemyCounter += es.DecreasePriority();

        }

        if (enemyCounter == 0) // no enemies left, destroy self to free up memory
        {
            Die();
        }

    }

    public void Die()
    {
        Object.Destroy(this.gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
