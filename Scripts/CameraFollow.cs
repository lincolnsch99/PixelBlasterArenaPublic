/// File Name: CameraFollow.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: Simple script just for positioning the camera to follow the player.
/// 
/// Date Last Updated: November 8, 2019

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private float lerpAmount;

    private GameObject player;
    private PersistentControl persistentController;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        persistentController = GameObject.FindWithTag("Persistent").GetComponent<PersistentControl>();
        GetComponent<Camera>().orthographicSize = 60;
    }

    /// <summary>
    /// FixedUpdate is called every fixed framerate frame.
    /// </summary>
    void FixedUpdate()
    {
        if(!persistentController.IsPaused())
        {
            if (Input.mouseScrollDelta.y > 0)
                GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize - 2.5f;
            if (Input.mouseScrollDelta.y < 0)
                GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize + 2.5f;
        }

        if (GetComponent<Camera>().orthographicSize > 75)
            GetComponent<Camera>().orthographicSize = 75;
        else if (GetComponent<Camera>().orthographicSize < 25)
            GetComponent<Camera>().orthographicSize = 25;
    }
}
