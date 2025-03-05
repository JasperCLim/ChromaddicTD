using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed; // How fast the enemy moves
    [SerializeField] private float health; // Enemy health
    [SerializeField] private float maxHealth; // Enemy max health
    [SerializeField] FloatingHealthBar healthBar; // Reference to the health bar
    [SerializeField] private string enemyColor; // Enemy Colour
    [SerializeField] private int priority; // Enemy hit priority

    private Color my_color = new Color(0,0,0);
    private GameObject targetTile; // Current target for the enemy
    private MapScript ms; // Variable to hold the MapScript.cs reference

    private List<GameObject> towersAttackingMe = new List<GameObject>();

    public Color attackTowerColorMix;

    private void moveEnemy()
    // This script moves the enemy towards the target tile
 
    {
        transform.position = Vector3.MoveTowards(transform.position, targetTile.transform.position, moveSpeed * Time.deltaTime);
    }

    private void checkTarget()

    // This script calculates the distance between the enemy and target
    // If it is at the target set the new target to the next tile along the path

    {

        if (targetTile != null)
        {
            float distance = (transform.position-targetTile.transform.position).magnitude;

            if (distance < 0.001f)
            {
                if (targetTile == ms.endTile) // end of path, destroy enemy
                {
                    LayeredEnemyScript les = GetComponentInParent<LayeredEnemyScript>();
                    if (les)
                    {
                        les.RemoveEnemy(this.gameObject); // remove the enemy from the layered enemy list of contents
                    }
                    
                    //Destroy(transform.gameObject);
                    EndPath();
                    Debug.Log("Enemy reached end of path. Lose a life");
                }
                else // find next target along path
                {
                    int currentIndex = ms.pathTiles.IndexOf(targetTile);

                    targetTile = ms.pathTiles[currentIndex +1];
                }

            }

        }
    }

    // decrement the number of lives and destroy the enemy once enemy reaches end of path
    void EndPath() {
        PlayerStats.Lives--;
        Destroy(transform.gameObject);
    }

    // Decrese the priority of an enemy to make it closer to being 1 (killable)
    public int DecreasePriority()
    {
        if (priority > 1)
        {
            priority--;
            return 1;
        }
        return 0;
    }

    // When an enemy of priority 1 dies, move all the other priorities down by 1
    private void LowerPriorityOfAllEnemies()
    {
        EnemyScript[] allEnemies = FindObjectsByType<EnemyScript>(FindObjectsSortMode.None);

        foreach (EnemyScript enemy in allEnemies)
        {
            if (enemy.GetPriority() > 1)
            {
                enemy.DecreasePriority();
            }
        }
    }



    public int GetPriority()
    {
        return priority;
    }

    public Color GetColor()
    {
        return my_color;
    }

    // enemy loses health and dies if health goes below 0
    public void Die(float dmg, Color attackingColor, GameObject attackingTower)
    {

        if(towersAttackingMe.Contains(attackingTower))
        {
            // this tower is already attacking
        } 
        else
        {
            // add the new tower to the mix of attacking towers
        
        if (attackingColor.r == 1)
        {
            attackTowerColorMix.r = 1;
        }
        if (attackingColor.g == 1)
        {
            attackTowerColorMix.g = 1;
        }
        if (attackingColor.b == 1)
        {
            attackTowerColorMix.b = 1;
        }
        if (attackingColor.a == 1)
        {
            attackTowerColorMix.a = 1;
        }
        towersAttackingMe.Add(attackingTower);
        }

        Debug.Log(my_color + " attacked by: " + attackTowerColorMix);

        if (attackTowerColorMix == my_color)
        {
            health -= dmg; // apply tower damage to enemy
        }

        healthBar.UpdateHealthBar(health, maxHealth); // update the health bar
        Debug.Log(health);

        // kill enemy
        if (health <= 0)
        {
            LayeredEnemyScript les = GetComponentInParent<LayeredEnemyScript>();
            if (les)
            {
                les.RemoveEnemy(this.gameObject);
            }


            if (priority == 1)
            {
                
                if (les)
                {
                    les.DecreaseEachPriority();
                }
                
            }
            Object.Destroy(this.gameObject);
        }

        StartCoroutine(ResetColor(attackingTower));
        
    }


    IEnumerator ResetColor(GameObject attackingTower)
    {
        //yield on a new YieldInstruction that waits for 1 seconds.
        yield return new WaitForSeconds(1);
        attackTowerColorMix = new Color(0,0,0,0);
        towersAttackingMe.Remove(attackingTower);
    }

    void Awake()

    // When the enemy spawns, find the Map object and store a reference to the MapScript. This is necessary to find the path tiles

    {
        GameObject map = GameObject.FindWithTag("Map");
        ms = map.GetComponent<MapScript>();
        healthBar = GetComponentInChildren<FloatingHealthBar>(); // get the health bar component
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthBar.UpdateHealthBar(health, maxHealth); // enemy starts with full health
        targetTile = ms.pathTiles[0]; // set the initial target to the first tile in the path
        SpriteRenderer my_sprite = GetComponent<SpriteRenderer>();

        switch(enemyColor)
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
                case "white":
                my_color = new Color(1,1,1);
                break;
                case "yellow":
                my_color = new Color(1,1,0);
                break;
                case "cyan":
                my_color = new Color(0,1,1);
                break;
                case "magenta":
                my_color = new Color(1,0,1);
                break;
            }
        my_sprite.color = my_color;
    }

    // Update is called once per frame
    void Update()
    {
        checkTarget();
        moveEnemy();
    }
}
