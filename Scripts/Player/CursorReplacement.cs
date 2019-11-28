/// File Name: CursorReplacement.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: Replaces the cursor with a custom targeter sprite.
/// 
/// Date Last Updated: November 27, 2019

using UnityEngine;

public class CursorReplacement : MonoBehaviour
{
    private Camera mainCamera;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    /// <summary>
    /// Update is called every frame.
    /// </summary>
    void Update()
    {
        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(worldPoint.x, worldPoint.y, 0);
        transform.Rotate(0, 0, 2.5f);
    }
}
