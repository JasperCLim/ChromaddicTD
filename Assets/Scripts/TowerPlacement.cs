using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private Vector2 GetMouseLocation()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FindTileToBuild()
    {
        Vector2 mousePosition = GetMouseLocation();

        RaycastHit2D tile = Physics2D.Raycast(mousePosition,new Vector2(0,0), 0.1f, tileMask,-100,100);

        if (tile.collider != null)
        {

            // Check if it's a map tile but not a path tile
            if (tile.collider.tag == "Background" && tile.collider.tag != "Path")
                {
                    hoverTile = tile.collider.gameObject; // Set hoverTile value to GameObject

                }
           
        }
    }

    public bool TowerExistsAlready()
    {
        bool towerExists = false;

        Vector2 mousePosition = GetMouseLocation();

        RaycastHit2D tile = Physics2D.Raycast(mousePosition,new Vector2(0,0), 0.1f, towerMask,-100,100);

        if (tile.collider != null)
        {
            towerExists = true;
        }

        return towerExists;
    }

    private void PlaceTower()
    {
        if (hoverTile != null && TowerExistsAlready() == false)
        {
            GameObject newTower = Instantiate(towerToPlace);
            newTower.transform.position = hoverTile.transform.position;
            newTower.layer = LayerMask.NameToLayer("TowerLayer");

            isBuilding = false;

            if (previewTower != null)
            {
                Destroy(previewTower);
            }
        }
    }

    private void PreviewBuilding()
    {
        isBuilding = true;

        previewTower = Instantiate(towerToPlace);

        if (previewTower.GetComponent<TowerScript>() != null)
        {
            Destroy(previewTower.GetComponent<TowerScript>()); // make sure TowerScript doesn't run during preview placement
        }


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
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
                if (Input.GetButtonDown("Fire1"))
                {
                    Debug.Log("Place Tower");
                    PlaceTower();
                }
            }
        } 
        else // not current building, look for input to start building
        {
            if (Input.GetKeyDown(KeyCode.R)) // place red tower
            {
                towerToPlace = availableTowers[0];
                PreviewBuilding();
            } 
            else if (Input.GetKeyDown(KeyCode.G)) // place green tower
            {
                towerToPlace = availableTowers[1];
                PreviewBuilding();            
            }
            else if (Input.GetKeyDown(KeyCode.B)) // place blue tower
            {            
                towerToPlace = availableTowers[2];
                PreviewBuilding();
            }
        }
        
    }
}
