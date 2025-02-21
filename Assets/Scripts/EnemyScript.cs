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

    }

    // Update is called once per frame
    void Update()
    {
        checkTarget();
        moveEnemy();
    }
}
