using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorReplacement : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(worldPoint.x, worldPoint.y, 0);
        transform.Rotate(0, 0, 5);
    }
}
