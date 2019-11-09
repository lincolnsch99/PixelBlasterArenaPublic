/// File Name: EnemyControl.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: Every GameObject that is representing an enemy has this script attached. This script is used
/// for monitoring and changing enemy behavior. It also stores necessary data for the enemy, such as health and
/// damage.
/// 
/// Date Last Updated: November 8, 2019

using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    [SerializeField]
    private EnemyType enemyType;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private int damage;
    [SerializeField]
    private int maxLives;
    [SerializeField]
    private int collisionForce;

    private int curLives;
    private GameObject player, playerController;
    
    /// <summary>
    /// Awake is called on the first frame of instantiation.
    /// </summary>
    private void Awake()
    {
        curLives = maxLives;
        player = GameObject.FindWithTag("Player");
        playerController = GameObject.Find("PlayerController");
        GetPath();
    }

    /// <summary>
    /// FixedUpdate is called every fixed framerate frame.
    /// </summary>
    private void FixedUpdate()
    {
        if(enemyType == EnemyType.SHREDDER)
        {
            transform.GetChild(transform.childCount - 1).Rotate(0, 0, 5);
        }
        else if(enemyType == EnemyType.PURSUER)
        {
            GetPath();
        }
    }

    /// <summary>
    /// Sets the enemy velocity according to where the player is. The enemy will move in a straight line 
    /// towards where the player was located when the enemy was first instantiated.
    /// </summary>
    private void GetPath()
    {
        float xHeight = transform.position.x - player.transform.position.x;
        float yHeight = transform.position.y - player.transform.position.y;

        float angleInRad = Mathf.Atan(yHeight / xHeight);
        float angleInDeg = Mathf.Rad2Deg * angleInRad;
        float xSpeed = moveSpeed * Mathf.Cos(angleInRad);
        float ySpeed = moveSpeed * Mathf.Sin(angleInRad);
        if(player.transform.position.x < transform.position.x)
        {
            xSpeed *= -1;
            ySpeed *= -1;
            angleInDeg += 180;
        }
        Vector2 movePath = new Vector2(xSpeed, ySpeed);
        GetComponent<Rigidbody2D>().velocity = movePath;
        transform.localEulerAngles = new Vector3(0, 0, angleInDeg);
    }

    /// <summary>
    /// Called when the collision trigger is collided with. If it collides with the player,
    /// the player takes damage.
    /// </summary>
    /// <param name="collision">The collision component that was collided with.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collidedObject = collision.gameObject;
        if(collidedObject.tag == "Player")
        {
            for(int i = 0; i < damage; i++)
                playerController.BroadcastMessage("TakeDamage", SendMessageOptions.DontRequireReceiver);
            PlayerControl.ApplyDamageForce(this.gameObject);
        }
    }

    /// <summary>
    /// Subtracts one life from the enemy's current lives. If they have 0 lives or less, they are killed.
    /// </summary>
    public void TakeDamage()
    {
        curLives--;
        if (curLives <= 0)
            Kill();
    }

    /// <summary>
    /// "Kills" the enemy. Destroys the enemy gameObject.
    /// </summary>
    private void Kill()
    {
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Getter for this enemy's collision force.
    /// </summary>
    /// <returns>The collision force of this enemy.</returns>
    public int GetCollisionForce()
    {
        return this.collisionForce;
    }

    /// <summary>
    /// Setter for this enemy's type.
    /// </summary>
    /// <param name="enemyType">The new enemy type.</param>
    public void SetEnemyType(EnemyType enemyType)
    {
        this.enemyType = enemyType;
    }
}
