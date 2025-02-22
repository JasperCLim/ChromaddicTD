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

    private GameObject targetTile; // Current target for the enemy
    private MapScript ms; // Variable to hold the MapScript.cs reference

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
                    Destroy(transform.gameObject);
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

    // Decrese the priority of an enemy to make it closer to being 1 (killable)
    public void DecreasePriority()
    {
        if (priority > 1)
        {
            priority--;
        }
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

    public string GetColor()
    {
        return enemyColor;
    }

    // enemy loses health and dies if health goes below 0
    public void die(float dmg)
    {
        health -= dmg; // apply tower damage to enemy
        healthBar.UpdateHealthBar(health, maxHealth); // update the health bar
        Debug.Log(health);

        // kill enemy
        if (health <= 0)
        {
            Object.Destroy(this.gameObject);
            if (priority == 1)
            {
                LowerPriorityOfAllEnemies();
            }
        }
        
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
        healthBar.UpdateHealthBar(health, maxHealth); // enemy sarts with full health
        targetTile = ms.pathTiles[0]; // set the initial target to the first tile in the path
        SpriteRenderer my_sprite = GetComponent<SpriteRenderer>();
        Color my_color = new Color(0,0,0);
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
