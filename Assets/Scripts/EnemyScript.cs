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

    private Color my_color;
    private GameObject targetTile; // Current target for the enemy
    private MapScript ms; // Variable to hold the MapScript.cs reference

    private List<GameObject> towersAttackingMe = new();

    public Color attackTowerColorMix;

    private void MoveEnemy()
    // This script moves the enemy towards the target tile
 
    {
        transform.position = Vector3.MoveTowards(transform.position, targetTile.transform.position, moveSpeed * Time.deltaTime);
    }

    private void CheckTarget()

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
                    LowerPriorityOfAllEnemies(); // handling for layered enemies
                    
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
        if (PlayerStats.Lives !=0)
        {
            PlayerStats.Lives--;
        }
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

    private void LowerPriorityOfAllEnemies()
    // Checks to see if the enemy is part of a layered enemy. If so, remove from parent object list and decrease priority of other layers
    {

        LayeredEnemyScript les = GetComponentInParent<LayeredEnemyScript>();
        if(les)
        {
            les.RemoveEnemy(this.gameObject); // remove this enemy from the parent object list of children
            les.DecreaseEachPriority(); // lower priority of other layers
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
            // add the attacking tower to the colors attacking this enemy
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
            // add the new tower to the mix of attacking towers
            towersAttackingMe.Add(attackingTower);
        }

        Debug.Log(this.name + " " + my_color + " attacked by: " + attackTowerColorMix + " priority " + priority);

        if (attackTowerColorMix == my_color)
        {
            health -= dmg; // apply tower damage to enemy
        }

        healthBar.UpdateHealthBar(health, maxHealth); // update the health bar
        Debug.Log(health);

        // kill enemy
        if (health <= 0)
        {
            LowerPriorityOfAllEnemies(); // see if this enemy is part of a layered enemy
            Object.Destroy(this.gameObject);
        }

        //StartCoroutine(ResetColor(attackingTower)); // after 1 second, reset colors and towers attacking this enemy
        
    }

/*
    IEnumerator ResetColor(GameObject attackingTower)
    // need to change this code to avoid bugs. Call this in Update(), not as an IEnumerator. 
    // foreach list of attacking towers 
    //      check if the distance from enemy to tower is larger than tower range. 
    //      If yes, reset color and remove attacking tower from list 
    {
        // after 2x attacking tower fire rate, reset colors and towers attacking this enemy
        TowerScript ts = attackingTower.GetComponent<TowerScript>();
        float falloffTime = ts.returnFireRate() * 2;
        yield return new WaitForSeconds(falloffTime);
        attackTowerColorMix = new Color(0,0,0,0);
        towersAttackingMe.Remove(attackingTower);
    }
*/

    private void ResetColor()
    {
        //Debug.Log("towers " + towersAttackingMe.Count);
        if (towersAttackingMe.Count > 0)
        {
            foreach (GameObject i in towersAttackingMe.ToArray())
            {
                TowerScript ts = i.GetComponent<TowerScript>();
                //Debug.Log(Vector2.Distance(i.transform.position,this.transform.position) + " " + ts.getTowerRange());
                if (Vector2.Distance(i.transform.position,this.transform.position) > ts.getTowerRange())
                {
                    attackTowerColorMix = new Color(0,0,0,0);
                    towersAttackingMe.Clear();
                }
            }
        }

    }

    public void Spawn(string col, float healthScale, float moveScale)
    {
        enemyColor = col;
        SetMyColor();
        health += healthScale;
        maxHealth += healthScale;
        moveSpeed += moveScale;
    }
    void Awake()

    // When the enemy spawns, find the Map object and store a reference to the MapScript. This is necessary to find the path tiles

    {
        my_color = new Color(0,0,0,0);
        attackTowerColorMix = new Color(0,0,0,1);
        GameObject map = GameObject.FindWithTag("Map");
        ms = map.GetComponent<MapScript>();
        healthBar = GetComponentInChildren<FloatingHealthBar>(); // get the health bar component
    }

    private void SetMyColor()
    {
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthBar.UpdateHealthBar(health, maxHealth); // enemy starts with full health
        targetTile = ms.pathTiles[0]; // set the initial target to the first tile in the path
        SetMyColor();
    }

    // Update is called once per frame
    void Update()
    {
        CheckTarget();
        MoveEnemy();
        ResetColor();
    }
}
