/// File Name: CameraFollow.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: Simple script just for positioning the camera to follow the player.
/// 
/// Date Last Updated: November 8, 2019

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private enum CameraType
    {
        STATIC,
        FOLLOW
    }

    [SerializeField]
    private float lerpAmount;

    private GameObject player;
    private PersistentControl persistentController;
    private Vector2 staticPlacement;
    private CameraType type;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        persistentController = GameObject.FindWithTag("Persistent").GetComponent<PersistentControl>();
        GetComponent<Camera>().orthographicSize = 60;
        staticPlacement = transform.position;
        type = CameraType.STATIC;
    }

    /// <summary>
    /// Update is called every frame.
    /// </summary>
    void Update()
    {
        if(!persistentController.IsPaused())
        {
            if (Input.mouseScrollDelta.y > 0)
                GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize - 2.5f;
            if (Input.mouseScrollDelta.y < 0)
                GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize + 2.5f;

            if(Input.GetKeyDown(KeyCode.F))
            {
                if (type == CameraType.STATIC)
                    type = CameraType.FOLLOW;
                else
                {
                    type = CameraType.STATIC;
                    transform.position = staticPlacement;
                }
            }

            if(type == CameraType.FOLLOW)
            {
                if(player.transform.position.x > -10 && player.transform.position.x < 110
                    && player.transform.position.y > -10 && player.transform.position.y < 110)
                    transform.position = Vector2.Lerp(transform.position, player.transform.position, 0.125f);
            }

            if(type == CameraType.STATIC)
            {
                Vector2 curPos = transform.position;
                if (Input.GetKeyDown(KeyCode.UpArrow))
                    curPos.y += 5;
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                    curPos.y -= 5;
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                    curPos.x += 5;
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    curPos.x -= 5;

                staticPlacement = curPos;
                transform.position = curPos;
            }
        }

        if (GetComponent<Camera>().orthographicSize > 75)
            GetComponent<Camera>().orthographicSize = 75;
        else if (GetComponent<Camera>().orthographicSize < 25)
            GetComponent<Camera>().orthographicSize = 25;

        if (transform.position.z > -10)
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
}
