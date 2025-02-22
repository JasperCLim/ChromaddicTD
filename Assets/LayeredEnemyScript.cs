using UnityEngine;

public class LayeredEnemyScript : MonoBehaviour
{

    [SerializeField] private EnemyScript[] enemyList;

    public void DecreaseEachPriority()
    {
        foreach (EnemyScript es in enemyList)
        {
            es.DecreasePriority();
        }
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
