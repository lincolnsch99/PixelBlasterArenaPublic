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
    

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera");
        map = new BoundaryMap(MAP_SIZE, MAP_SIZE);
        currentPrefabSelection = GameObject.Instantiate(verticalStraightPrefab);
        currentTypeSelection = SingleSpaceType.VERTICAL_STRAIGHT;
        editable = (SceneManager.GetActiveScene().name != "GameScene");
    }

    // Update is called once per frame
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

    private Vector2Int ConvertToGridIndex(Vector2 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x / SPRITE_UNIT_WIDTH),
            Mathf.RoundToInt(worldPosition.y / SPRITE_UNIT_WIDTH));
    }

    private Vector3 ConvertToWorldPosition(Vector2Int gridIndex)
    {
        return new Vector3(gridIndex.x * SPRITE_UNIT_WIDTH, gridIndex.y * SPRITE_UNIT_WIDTH, 0);
    }

    private void DisplaySpriteAtPosition(Vector2 displayAt)
    {
        Vector2Int gridPos = ConvertToGridIndex(displayAt);
        currentPrefabSelection.transform.position = ConvertToWorldPosition(gridPos);
        currentPrefabSelection.transform.GetChild(0).GetComponent<SpriteRenderer>()
            .color = new Color(1, 1, 1, 0.5f);
    }

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

    private void ClearMap()
    {
        GameObject[] objectsInScene = FindObjectsOfType<GameObject>();
        foreach(GameObject gObject in objectsInScene)
        {
            if (gObject.tag == "Boundary" && !gObject.Equals(currentPrefabSelection))
                Destroy(gObject);
        }
    }

    public void SetMap(BoundaryMap map)
    {
        this.map = new BoundaryMap(map);
        DisplayMap();
    }

    public BoundaryMap GetMap()
    {
        return map;
    }
}
