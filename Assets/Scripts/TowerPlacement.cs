using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private GameObject[] availableTowers;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask tileMask;
    [SerializeField] private LayerMask towerMask;

    private GameObject towerToPlace;
    private GameObject previewTower;
    private GameObject hoverTile;
    private bool isBuilding;
    private int selectedTowerIndex = -1;
    
    // Reference to economy system
    private EconomySystem economySystem;

    private Vector2 GetMouseLocation()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FindTileToBuild()
    {
        Vector2 mousePosition = GetMouseLocation();
        RaycastHit2D tile = Physics2D.Raycast(mousePosition, new Vector2(0, 0), 0.1f, tileMask, -100, 100);

        if (tile.collider != null)
        {
            // Check if it's a map tile but not a path tile
            if (tile.collider.tag == "Background" && tile.collider.tag != "Path")
            {
                hoverTile = tile.collider.gameObject;
            }
        }
    }

    public bool TowerExistsAlready()
    {
        bool towerExists = false;
        Vector2 mousePosition = GetMouseLocation();
        RaycastHit2D tile = Physics2D.Raycast(mousePosition, new Vector2(0, 0), 0.1f, towerMask, -100, 100);

        if (tile.collider != null)
        {
            towerExists = true;
        }
        return towerExists;
    }

    private void PlaceTower()
    {
        // Check if we can place a tower and afford it
        if (hoverTile != null && !TowerExistsAlready() && economySystem != null)
        {
            // Attempt to purchase the tower
            if (economySystem.PurchaseTower(selectedTowerIndex))
            {
                // Create the tower
                GameObject newTower = Instantiate(towerToPlace);
                newTower.transform.position = hoverTile.transform.position;
                newTower.layer = LayerMask.NameToLayer("TowerLayer");

                isBuilding = false;
                if (previewTower != null)
                {
                    Destroy(previewTower);
                }
            }
            else
            {
                // Not enough money
                Debug.Log("Not enough gold to place this tower!");
            }
        }
    }

    private void PreviewBuilding(int towerIndex)
    {
        selectedTowerIndex = towerIndex;
        
        // Check if we can afford this tower
        if (economySystem != null && !economySystem.CanAffordTower(towerIndex))
        {
            Debug.Log("Cannot afford tower! Cost: 50 gold");
            return;
        }
        
        isBuilding = true;
        towerToPlace = availableTowers[towerIndex];
        previewTower = Instantiate(towerToPlace);

        // Remove the TowerScript from the preview
        if (previewTower.GetComponent<TowerScript>() != null)
        {
            Destroy(previewTower.GetComponent<TowerScript>());
        }
        
        // Make preview semi-transparent
        SpriteRenderer renderer = previewTower.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color color = renderer.color;
            color.a = 0.5f; // 50% transparency
            renderer.color = color;
        }
    }

    void Start()
    {
        // Find the economy system
        economySystem = FindFirstObjectByType<EconomySystem>();
        
        if (economySystem == null)
        {
            Debug.LogError("Economy System not found! Please add EconomySystem to the scene.");
        }
    }

    void Update()
    {
        if (isBuilding) // already in build mode, preview tower on mouse location
        {
            if (previewTower != null)
            {
                FindTileToBuild();

                if (hoverTile != null)
                {
                    previewTower.transform.position = hoverTile.transform.position;
                }
                
                // Left click to place tower
                if (Input.GetButtonDown("Fire1"))
                {
                    PlaceTower();
                }
                
                // Right click or Escape to cancel
                if (Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.Escape))
                {
                    isBuilding = false;
                    Destroy(previewTower);
                }
            }
        } 
        else // not current building, look for input to start building
        {
            if (Input.GetKeyDown(KeyCode.R)) // place red tower
            {
                PreviewBuilding(0);
            } 
            else if (Input.GetKeyDown(KeyCode.G)) // place green tower
            {
                PreviewBuilding(1);          
            }
            else if (Input.GetKeyDown(KeyCode.B)) // place blue tower
            {           
                PreviewBuilding(2);
            }
        }
    }
}
