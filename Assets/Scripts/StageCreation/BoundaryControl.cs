/// File Name: BoundaryControl.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: Simple script that is attached to all boundaries. If the player collides with
/// this object, the player is instantly killed.
/// 
/// Date Last Updated: November 8, 2019

using UnityEngine;

public class BoundaryControl : MonoBehaviour
{ 
    private GameObject playerController;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        playerController = GameObject.Find("PlayerController");
    }

    /// <summary>
    /// For every collision, the boundary checks if it was the player that collided with it. If so, kill the player.
    /// Otherwise ignore the object.
    /// </summary>
    /// <param name="collision">The collider that collided with this boundary trigger.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collidedObject = collision.gameObject;
        if(collidedObject.tag == "Player")
        {
            playerController.BroadcastMessage("Kill", SendMessageOptions.DontRequireReceiver);
        }
    }
}
