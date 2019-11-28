/// File Name: BoundaryDrawer.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: Handles displaying UI to the user and taking inputs from the user
/// when creating custom stages. 
/// 
/// Date Last Updated: November 27, 2019

using UnityEngine;
using UnityEngine.SceneManagement;

public class BoundaryDrawer : MonoBehaviour
{
    public static int MAP_SIZE = 20;
    public static int SPRITE_UNIT_WIDTH = 5;

    [SerializeField]
    private GameObject verticalStraightPrefab;
    [SerializeField]
    private GameObject horizontalStraightPrefab;
    [SerializeField]
    private GameObject corner1QPrefab;
    [SerializeField]
    private GameObject corner2QPrefab;
    [SerializeField]
    private GameObject corner3QPrefab;
    [SerializeField]
    private GameObject corner4QPrefab;
    [SerializeField]
    private GameObject playerSpawnPrefab;
    [SerializeField]
    private AudioClip placeClick;
    [SerializeField]
    private AudioClip removeClick;

    private bool editable;
    public bool Editable { get { return editable; } set { editable = value; } }

    private Vector2 mouseWorldPos;
    private Vector2Int lastPlacedPos, lastRemovedPos;
    private BoundaryMap map;
    private GameObject currentPrefabSelection, mainCamera;
    private SingleSpaceType currentTypeSelection;
    

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera");
        map = new BoundaryMap(MAP_SIZE, MAP_SIZE);
        currentPrefabSelection = GameObject.Instantiate(verticalStraightPrefab);
        currentTypeSelection = SingleSpaceType.VERTICAL_STRAIGHT;
        editable = (SceneManager.GetActiveScene().name != "GameScene");
    }

    /// <summary>
    /// Update is called every frame.
    /// </summary>
    void Update()
    {
        if (editable)
        {
            mouseWorldPos = mainCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPos = ConvertToGridIndex(mouseWorldPos);
            if (gridPos.x >= 0 && gridPos.x < map.GetWidth()
                && gridPos.y >= 0 && gridPos.y < map.GetHeight())
            {
                currentPrefabSelection.SetActive(true);
                DisplaySpriteAtPosition(mouseWorldPos);
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    if (currentTypeSelection != SingleSpaceType.VERTICAL_STRAIGHT)
                    {
                        Destroy(currentPrefabSelection);
                        currentPrefabSelection = GameObject.Instantiate(verticalStraightPrefab);
                        currentTypeSelection = SingleSpaceType.VERTICAL_STRAIGHT;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    if (currentTypeSelection != SingleSpaceType.CORNER_1Q)
                    {
                        Destroy(currentPrefabSelection);
                        currentPrefabSelection = GameObject.Instantiate(corner1QPrefab);
                        currentTypeSelection = SingleSpaceType.CORNER_1Q;
                    }
                }
                else if(Input.GetKeyDown(KeyCode.Alpha3))
                {
                    if (currentTypeSelection != SingleSpaceType.PLAYER_SPAWN)
                    {
                        Destroy(currentPrefabSelection);
                        currentPrefabSelection = GameObject.Instantiate(playerSpawnPrefab);
                        currentTypeSelection = SingleSpaceType.PLAYER_SPAWN;
                    }
                }
                if (Input.mouseScrollDelta.y < 0)
                {
                    if (currentTypeSelection == SingleSpaceType.CORNER_1Q || currentTypeSelection == SingleSpaceType.CORNER_2Q
                        || currentTypeSelection == SingleSpaceType.CORNER_3Q || currentTypeSelection == SingleSpaceType.CORNER_4Q)
                    {
                        Destroy(currentPrefabSelection);
                        currentPrefabSelection = NextCornerTypePrefab();
                        currentTypeSelection = NextCornerType();
                    }
                    else if(currentTypeSelection == SingleSpaceType.VERTICAL_STRAIGHT 
                        || currentTypeSelection == SingleSpaceType.HORIZONTAL_STRAIGHT)
                    {
                        Destroy(currentPrefabSelection);
                        currentPrefabSelection = OtherRegularTypePrefab();
                        currentTypeSelection = OtherRegularType();
                    }
                }

                if (Input.mouseScrollDelta.y > 0)
                {
                    if (currentTypeSelection == SingleSpaceType.CORNER_1Q || currentTypeSelection == SingleSpaceType.CORNER_2Q
                        || currentTypeSelection == SingleSpaceType.CORNER_3Q || currentTypeSelection == SingleSpaceType.CORNER_4Q)
                    {
                        Destroy(currentPrefabSelection);
                        currentPrefabSelection = PrevCornerTypePrefab();
                        currentTypeSelection = PrevCornerType();
                    }
                    else if(currentTypeSelection == SingleSpaceType.VERTICAL_STRAIGHT
                        || currentTypeSelection == SingleSpaceType.HORIZONTAL_STRAIGHT)
                    {
                        Destroy(currentPrefabSelection);
                        currentPrefabSelection = OtherRegularTypePrefab();
                        currentTypeSelection = OtherRegularType();
                    }
                }

                if (Input.GetMouseButton(0))
                {
                    SetSpriteAtPosition(mouseWorldPos);
                    DisplayMap();
                }
                else if (Input.GetMouseButton(1))
                {
                    RemoveSpriteAtPosition(mouseWorldPos);
                    DisplayMap();
                }
            }
            else
            {
                currentPrefabSelection.SetActive(false);
            }
        }
        else
        {
            currentPrefabSelection.SetActive(false);
        }
    }

    /// <summary>
    /// Converts the world position to a coordinate representing the position in the double array used
    /// for storing stage boundaries.
    /// </summary>
    /// <param name="worldPosition">Position to be converted to coordinates.</param>
    /// <returns>A Vector2Int representing coordinates.</returns>
    private Vector2Int ConvertToGridIndex(Vector2 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x / SPRITE_UNIT_WIDTH),
            Mathf.RoundToInt(worldPosition.y / SPRITE_UNIT_WIDTH));
    }

    /// <summary>
    /// Converts the coordinates to a position in the game world.
    /// </summary>
    /// <param name="gridIndex">The coordinates to be converted to world position.</param>
    /// <returns>A Vector3 representing the converted position.</returns>
    private Vector3 ConvertToWorldPosition(Vector2Int gridIndex)
    {
        return new Vector3(gridIndex.x * SPRITE_UNIT_WIDTH, gridIndex.y * SPRITE_UNIT_WIDTH, 0);
    }

    /// <summary>
    /// Shows the currently selected sprite at the desired position (with half opacity).
    /// </summary>
    /// <param name="displayAt">The position the sprite will be displayed at.</param>
    private void DisplaySpriteAtPosition(Vector2 displayAt)
    {
        Vector2Int gridPos = ConvertToGridIndex(displayAt);
        currentPrefabSelection.transform.position = ConvertToWorldPosition(gridPos);
        currentPrefabSelection.transform.GetChild(0).GetComponent<SpriteRenderer>()
            .color = new Color(1, 1, 1, 0.5f);
    }

    /// <summary>
    /// Updates the boundary map to now include the selected boundary at the given position.
    /// Once set, the boundary map will display the boundary at full opacity.
    /// </summary>
    /// <param name="setAt">The position that the sprite will be displayed at.</param>
    private void SetSpriteAtPosition(Vector2 setAt)
    {
        Vector2Int gridPos = ConvertToGridIndex(setAt);
        if (gridPos.x >= 0 && gridPos.x < map.GetWidth()
            && gridPos.y >= 0 && gridPos.y < map.GetHeight()
            && gridPos != lastPlacedPos)
        {
            lastPlacedPos = gridPos;
            map.UpdateSpace(gridPos.x, gridPos.y, currentTypeSelection);
            GetComponent<AudioSource>().PlayOneShot(placeClick);
        }
    }

    /// <summary>
    /// Updates the boundary map to remove boundaries from the given location.
    /// </summary>
    /// <param name="removeAt">The position that the boundaries will be cleared.</param>
    private void RemoveSpriteAtPosition(Vector2 removeAt)
    {
        Vector2Int gridPos = ConvertToGridIndex(removeAt);
        if (gridPos.x >= 0 && gridPos.x < map.GetWidth()
            && gridPos.y >= 0 && gridPos.y < map.GetHeight()
            && gridPos != lastRemovedPos)
        {
            lastRemovedPos = gridPos;
            if(map.ResetSpace(gridPos.x, gridPos.y))
                GetComponent<AudioSource>().PlayOneShot(removeClick);
        }
    }

    /// <summary>
    /// Cycles the corner sprite to the next in line.
    /// </summary>
    /// <returns>A GameObject of the correct sprite that has been instantiated.</returns>
    private GameObject NextCornerTypePrefab()
    {
        switch(currentTypeSelection)
        {
            case SingleSpaceType.CORNER_1Q:
                return GameObject.Instantiate(corner2QPrefab, 
                    ConvertToWorldPosition(ConvertToGridIndex(mouseWorldPos)), new Quaternion());
            case SingleSpaceType.CORNER_2Q:
                return GameObject.Instantiate(corner3QPrefab, 
                    ConvertToWorldPosition(ConvertToGridIndex(mouseWorldPos)), new Quaternion());
            case SingleSpaceType.CORNER_3Q:
                return GameObject.Instantiate(corner4QPrefab, 
                    ConvertToWorldPosition(ConvertToGridIndex(mouseWorldPos)), new Quaternion());
            case SingleSpaceType.CORNER_4Q:
                return GameObject.Instantiate(corner1QPrefab, 
                    ConvertToWorldPosition(ConvertToGridIndex(mouseWorldPos)), new Quaternion());
            default:
                return GameObject.Instantiate(corner1QPrefab, 
                    ConvertToWorldPosition(ConvertToGridIndex(mouseWorldPos)), new Quaternion());
        }
    }

    /// <summary>
    /// Cycles the corner type to the next in line.
    /// </summary>
    /// <returns>Type identifying what space type is next to be selected.</returns>
    private SingleSpaceType NextCornerType()
    {
        switch (currentTypeSelection)
        {
            case SingleSpaceType.CORNER_1Q:
                return SingleSpaceType.CORNER_2Q;
            case SingleSpaceType.CORNER_2Q:
                return SingleSpaceType.CORNER_3Q;
            case SingleSpaceType.CORNER_3Q:
                return SingleSpaceType.CORNER_4Q;
            case SingleSpaceType.CORNER_4Q:
                return SingleSpaceType.CORNER_1Q;
            default:
                return SingleSpaceType.CORNER_1Q;
        }
    }

    /// <summary>
    /// Cycles the regular sprite to the next in line.
    /// </summary>
    /// <returns>A GameObject of the correct sprite that has been instantiated.</returns>
    private GameObject OtherRegularTypePrefab()
    {
        switch(currentTypeSelection)
        {
            case SingleSpaceType.VERTICAL_STRAIGHT:
                return GameObject.Instantiate(horizontalStraightPrefab, 
                    ConvertToWorldPosition(ConvertToGridIndex(mouseWorldPos)), new Quaternion());
            case SingleSpaceType.HORIZONTAL_STRAIGHT:
                return GameObject.Instantiate(verticalStraightPrefab, 
                    ConvertToWorldPosition(ConvertToGridIndex(mouseWorldPos)), new Quaternion());
            default:
                return GameObject.Instantiate(verticalStraightPrefab, 
                    ConvertToWorldPosition(ConvertToGridIndex(mouseWorldPos)), new Quaternion());
        }
    }

    /// <summary>
    /// Cycles the regular type to the next in line.
    /// </summary>
    /// <returns>Type identifying what space type is next to be selected.</returns>
    private SingleSpaceType OtherRegularType()
    {
        switch (currentTypeSelection)
        {
            case SingleSpaceType.VERTICAL_STRAIGHT:
                return SingleSpaceType.HORIZONTAL_STRAIGHT;
            case SingleSpaceType.HORIZONTAL_STRAIGHT:
                return SingleSpaceType.VERTICAL_STRAIGHT;
            default:
                return SingleSpaceType.VERTICAL_STRAIGHT;
        }
    }

    /// <summary>
    /// Cycles the corner sprite to the previous in line.
    /// </summary>
    /// <returns>A GameObject of the correct sprite that has been instantiated.</returns>
    private GameObject PrevCornerTypePrefab()
    {
        switch (currentTypeSelection)
        {
            case SingleSpaceType.CORNER_1Q:
                return GameObject.Instantiate(corner4QPrefab, 
                    ConvertToWorldPosition(ConvertToGridIndex(mouseWorldPos)), new Quaternion());
            case SingleSpaceType.CORNER_2Q:
                return GameObject.Instantiate(corner1QPrefab, 
                    ConvertToWorldPosition(ConvertToGridIndex(mouseWorldPos)), new Quaternion());
            case SingleSpaceType.CORNER_3Q:
                return GameObject.Instantiate(corner2QPrefab, 
                    ConvertToWorldPosition(ConvertToGridIndex(mouseWorldPos)), new Quaternion());
            case SingleSpaceType.CORNER_4Q:
                return GameObject.Instantiate(corner3QPrefab, 
                    ConvertToWorldPosition(ConvertToGridIndex(mouseWorldPos)), new Quaternion());
            default:
                return GameObject.Instantiate(corner1QPrefab, 
                    ConvertToWorldPosition(ConvertToGridIndex(mouseWorldPos)), new Quaternion());
        }
    }

    /// <summary>
    /// Cycles the corner type to the previous in line.
    /// </summary>
    /// <returns>Type identifying what space type is next to be selected.</returns>
    private SingleSpaceType PrevCornerType()
    {
        switch (currentTypeSelection)
        {
            case SingleSpaceType.CORNER_1Q:
                return SingleSpaceType.CORNER_4Q;
            case SingleSpaceType.CORNER_2Q:
                return SingleSpaceType.CORNER_1Q;
            case SingleSpaceType.CORNER_3Q:
                return SingleSpaceType.CORNER_2Q;
            case SingleSpaceType.CORNER_4Q:
                return SingleSpaceType.CORNER_3Q;
            default:
                return SingleSpaceType.CORNER_1Q;
        }
    }

    /// <summary>
    /// Updates the world to represent the boundary that is being made by the player.
    /// </summary>
    public void DisplayMap()
    {
        ClearMap();
        for(int i = 0; i < map.GetWidth(); i++)
        {
            for(int j = 0; j < map.GetHeight(); j++)
            {
                switch(map.GetTypeAtPosition(i, j))
                {
                    case SingleSpaceType.VERTICAL_STRAIGHT:
                        GameObject.Instantiate(verticalStraightPrefab, 
                            ConvertToWorldPosition(new Vector2Int(i, j)), new Quaternion());
                        break;
                    case SingleSpaceType.HORIZONTAL_STRAIGHT:
                        GameObject.Instantiate(horizontalStraightPrefab,
                            ConvertToWorldPosition(new Vector2Int(i, j)), new Quaternion());
                        break;
                    case SingleSpaceType.CORNER_1Q:
                        GameObject.Instantiate(corner1QPrefab,
                            ConvertToWorldPosition(new Vector2Int(i, j)), new Quaternion());
                        break;
                    case SingleSpaceType.CORNER_2Q:
                        GameObject.Instantiate(corner2QPrefab,
                            ConvertToWorldPosition(new Vector2Int(i, j)), new Quaternion());
                        break;
                    case SingleSpaceType.CORNER_3Q:
                        GameObject.Instantiate(corner3QPrefab,
                            ConvertToWorldPosition(new Vector2Int(i, j)), new Quaternion());
                        break;
                    case SingleSpaceType.CORNER_4Q:
                        GameObject.Instantiate(corner4QPrefab,
                            ConvertToWorldPosition(new Vector2Int(i, j)), new Quaternion());
                        break;
                    case SingleSpaceType.PLAYER_SPAWN:
                        if (editable)
                        {
                            GameObject.Instantiate(playerSpawnPrefab,
                                ConvertToWorldPosition(new Vector2Int(i, j)), new Quaternion());
                        }
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Clears the world of all existing boundary objects.
    /// </summary>
    private void ClearMap()
    {
        GameObject[] objectsInScene = FindObjectsOfType<GameObject>();
        foreach(GameObject gObject in objectsInScene)
        {
            if (gObject.tag == "Boundary" && !gObject.Equals(currentPrefabSelection))
                Destroy(gObject);
        }
    }

    /// <summary>
    /// Setter for the boundary map.
    /// </summary>
    /// <param name="map">New BoundaryMap.</param>
    public void SetMap(BoundaryMap map)
    {
        this.map = new BoundaryMap(map);
        DisplayMap();
    }

    /// <summary>
    /// Getter for the boundary map.
    /// </summary>
    /// <returns>The boundary map.</returns>
    public BoundaryMap GetMap()
    {
        return map;
    }
}
