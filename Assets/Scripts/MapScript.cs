using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System.Linq;

public class MapScript : MonoBehaviour
{

    [SerializeField] // Lets us set the path within the Unity editor
    public List<GameObject> pathTiles = new List<GameObject>();

    public GameObject endTile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        endTile = pathTiles.Last(); // Set the endTile to the last tile in the path. Called dynamically in case the path changes (future proof?)
    }
}
