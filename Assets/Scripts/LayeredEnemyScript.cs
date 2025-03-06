using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class LayeredEnemyScript : MonoBehaviour
{

    [SerializeField] private List<GameObject> enemyList = new List<GameObject>();

    public void RemoveEnemy(GameObject enemy)
    {
        enemyList.Remove(enemy);
    }

    public void DecreaseEachPriority()
    {
        int enemyCounter = 0; // see how many enemies are left alive


        foreach (GameObject es in enemyList)
        {

            enemyCounter += es.GetComponent<EnemyScript>().DecreasePriority();

        }

        if (enemyCounter == 0) // no enemies left, destroy self to free up memory
        {
            Die();
        }

    }

    public List<GameObject> getLayers() {
        return enemyList;
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
        //Debug.Log(enemyList.Count);
        if (enemyList.Count == 0)
        {
            Object.Destroy(this.gameObject);
        }
    }
}
