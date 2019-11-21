using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerupControl : MonoBehaviour
{
    [Header("Movement Utilities")]
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private GameObject shieldPrefab;

    private GameObject player, playerController, persistentController;
    private bool hasPath;

    /// <summary>
    /// Awake is called on the first frame of instantiation.
    /// </summary>
    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        playerController = GameObject.Find("PlayerController");
        persistentController = GameObject.FindWithTag("Persistent");
        hasPath = false;
        GetPath();
    }

    /// <summary>
    /// FixedUpdate is called every fixed framerate frame.
    /// </summary>
    private void FixedUpdate()
    {
        if (player == null && !hasPath)
        {
            player = GameObject.FindWithTag("Player");
            GetPath();
        }

        if (CheckIfOffScreen())
        {
            Destroy(this.gameObject);
        }
    }

    private bool CheckIfOffScreen()
    {
        if (transform.position.x < -10 || transform.position.x > 110
            || transform.position.y > 110 || transform.position.y < -10)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Sets the enemy velocity according to where the player is. The enemy will move in a straight line 
    /// towards where the player was located when the enemy was first instantiated.
    /// </summary>
    private void GetPath()
    {
        if (player == null)
            return;
        float xHeight = transform.position.x - player.transform.position.x;
        float yHeight = transform.position.y - player.transform.position.y;

        float angleInRad = Mathf.Atan(yHeight / xHeight);
        float angleInDeg = Mathf.Rad2Deg * angleInRad;
        float xSpeed = moveSpeed * Mathf.Cos(angleInRad);
        float ySpeed = moveSpeed * Mathf.Sin(angleInRad);
        if (player.transform.position.x < transform.position.x)
        {
            xSpeed *= -1;
            ySpeed *= -1;
            angleInDeg += 180;
        }
        Vector2 movePath = new Vector2(xSpeed, ySpeed);
        GetComponent<Rigidbody2D>().velocity = movePath;
        hasPath = true;
    }

    /// <summary>
    /// Called when the collision trigger is collided with. If it collides with the player,
    /// the player takes damage.
    /// </summary>
    /// <param name="collision">The collision component that was collided with.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collidedObject = collision.gameObject;
        if (collidedObject.tag == "Player")
        {
            GameObject.Instantiate(shieldPrefab);
            Destroy(this.gameObject);
        }
    }
}
