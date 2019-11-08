using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private float lerpAmount;

    private GameObject player;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    /// <summary>
    /// FixedUpdate is called every fixed framerate frame.
    /// </summary>
    void Update()
    {
        GetComponent<Rigidbody2D>().position = Vector3.Lerp(GetComponent<Rigidbody2D>().position, player.GetComponent<Rigidbody2D>().position, lerpAmount);
    }
}
