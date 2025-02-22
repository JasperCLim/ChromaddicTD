using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Threading;

public class TowerScript : MonoBehaviour
{

    [SerializeField] private float targetingRange;
    [SerializeField] private LayerMask tileMask;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private string towerColor;
    [SerializeField] private float fireRate; // tower fire rate
    [SerializeField] private float damage; // tower damage

    private float timer = 0; // timer for calculating damage over time

    private void FindNearbyTiles()
    {
        RaycastHit2D[] nearbyTiles = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, tileMask);
    
        if (nearbyTiles.Length > 0)
        {
            //Debug.Log("Found nearby tiles");
            int numHits = nearbyTiles.Length;
            foreach(RaycastHit2D i in nearbyTiles)
            {
                PathTileScript tileScript = i.collider.GetComponent<PathTileScript>();
                if (Vector2.Distance(i.transform.position, transform.position) < targetingRange)
                {
                    switch(towerColor)
                    {
                        case "blue":
                        tileScript.lightUp(0,0,1);
                        break;
                        case "green":
                        tileScript.lightUp(0,1,0);
                        break;
                        case "red":
                        tileScript.lightUp(1,0,0);
                        break;
                    }
                    
                }
                else tileScript.resetColor();

                
            }
        }
    
    }

    private void FindNearbyEnemies()
    {
        RaycastHit2D[] nearbyEnemies = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask);
        EnemyScript targetEnemy = null;

        // Find if the enemy with priority 1 was hit
        foreach (RaycastHit2D hit in nearbyEnemies)
        {
            EnemyScript enemy = hit.collider.GetComponent<EnemyScript>();

            if (enemy != null && enemy.GetPriority() == 1)
            {
                targetEnemy = enemy;
                break;
            }
        }

        // Damage the enemy with priority 1 if it was hit by a tower of the same colour
        if (targetEnemy != null && targetEnemy.GetColor() == towerColor)
        {
            Debug.Log($"Tower ({towerColor}) is attacking the {targetEnemy.GetColor()} enemy");
            targetEnemy.die(damage);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpriteRenderer my_sprite = GetComponent<SpriteRenderer>();
        Color my_color = new Color(0,0,0);
        switch(towerColor)
            {
                case "blue":
                my_color = new Color(0,0,1);
                break;
                case "green":
                my_color = new Color(0,1,0);
                break;
                case "red":
                my_color = new Color(1,0,0);
                break;
            }
        my_sprite.color = my_color;
    }

    // Update is called once per frame
    void Update()
    {
        FindNearbyTiles();

        // damage over time instead of every frame
        if (timer > fireRate)
        {
            FindNearbyEnemies();
            timer = 0;
        }

        timer += Time.deltaTime;
        
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    }

}
