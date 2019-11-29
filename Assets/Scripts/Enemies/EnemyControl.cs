/// File Name: EnemyControl.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: Every GameObject that is representing an enemy has this script attached. This script is used
/// for monitoring and changing enemy behavior. It also stores necessary data for the enemy, such as health and
/// damage.
/// 
/// Date Last Updated: November 26, 2019

using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    [SerializeField]
    private EnemyType enemyType;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private int maxLives;
    [SerializeField]
    private int collisionForce;
    [SerializeField]
    private int scoreValue;
    [SerializeField]
    private GameObject explosionPrefab;

    private int curLives, forceAppliedTime;
    private GameObject player, playerController, persistentController;
    private bool hasPath;
    private float timeToExplode, timeFromForce;
    private EnemySpawner spawner;
    
    /// <summary>
    /// Awake is called on the first frame of instantiation.
    /// </summary>
    private void Awake()
    {
        curLives = maxLives;
        player = GameObject.FindWithTag("Player");
        playerController = GameObject.Find("PlayerController");
        persistentController = GameObject.FindWithTag("Persistent");
        spawner = GameObject.FindWithTag("SpawnHolder").GetComponent<EnemySpawner>();
        hasPath = false;
        GetPath();
        if (enemyType == EnemyType.BUSTER)
            timeToExplode = 7f;
        timeFromForce = forceAppliedTime = 2;
    }

    /// <summary>
    /// FixedUpdate is called every fixed framerate frame.
    /// </summary>
    private void FixedUpdate()
    {
        if (!persistentController.GetComponent<PersistentControl>().IsPaused())
        {
            timeFromForce += Time.deltaTime;

            if (enemyType == EnemyType.SHREDDER)
            {
                transform.GetChild(transform.childCount - 1).Rotate(0, 0, 5);
            }
            else if (enemyType == EnemyType.PURSUER && timeFromForce > forceAppliedTime)
            {
                GetPath();
            }
            else if (enemyType == EnemyType.BUSTER)
            {
                timeToExplode -= Time.deltaTime;
                if (timeToExplode <= 0)
                {
                    Kill();
                }
                transform.GetChild(transform.childCount - 1).Rotate(0, 0, 40f / timeToExplode);
                transform.GetChild(0).Rotate(0, 0, -40f / timeToExplode);
            }

            if (CheckIfOffScreen())
            {
                Destroy(this.gameObject);
            }

            if(timeFromForce > forceAppliedTime && !hasPath)
            {
                GetPath();
            }
            else if(timeFromForce < forceAppliedTime)
            {
                GetComponent<Rigidbody2D>().drag = 0.5f;
            }

        }
    }

    /// <summary>
    /// Checks if the enemy has travelled outside of the playable area.
    /// </summary>
    /// <returns>True if the enemy is outside of bounds, false otherwise.</returns>
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
        if(player.transform.position.x < transform.position.x)
        {
            xSpeed *= -1;
            ySpeed *= -1;
            angleInDeg += 180;
        }
        Vector2 movePath = new Vector2(xSpeed, ySpeed);
        GetComponent<Rigidbody2D>().velocity = movePath;
        transform.localEulerAngles = new Vector3(0, 0, angleInDeg);
        hasPath = true;
        GetComponent<Rigidbody2D>().drag = 0;
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
            playerController.BroadcastMessage("ApplyEnemyForce", this.gameObject, SendMessageOptions.DontRequireReceiver);
            if (enemyType == EnemyType.PURSUER)
            {
                TakeDamage();
            }
            else if(enemyType == EnemyType.BUSTER)
            {
                timeToExplode = 0;
            }
        }  
    }

    /// <summary>
    /// Subtracts one life from the enemy's current lives. If they have 0 lives or less, they are killed.
    /// </summary>
    public void TakeDamage()
    {
        if (enemyType == EnemyType.BUSTER)
        {
            persistentController.GetComponent<PersistentControl>().IncrementPlayerScore(scoreValue);
        }
        else
        {
            curLives--;
            if (curLives < 1 && curLives + 1 > 0)
            {
                persistentController.GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
                GetComponent<Animator>().SetInteger("curLives", curLives);
            }
        }
    }

    /// <summary>
    /// "Kills" the enemy. Adds to player score and destroys the enemy gameObject.
    /// </summary>
    private void Kill()
    {
        if(enemyType == EnemyType.BUSTER)
        {
            persistentController.GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
            float radius = 10f;
            Collider2D[] collidersInRadius = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach(Collider2D collider in collidersInRadius)
            {
                if(collider.gameObject.tag == "Player")
                {
                    playerController.BroadcastMessage("ApplyEnemyForce", this.gameObject, SendMessageOptions.DontRequireReceiver);
                }
                else if(collider.gameObject.tag == "Enemy" && !collider.gameObject.Equals(this.gameObject))
                {
                    collider.gameObject.BroadcastMessage("ApplyEnemyForce", this.gameObject, SendMessageOptions.DontRequireReceiver);
                }
            }
            GameObject.Instantiate(explosionPrefab, transform.position, new Quaternion());
        }

        persistentController.GetComponent<PersistentControl>().IncrementPlayerScore(scoreValue);
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

    /// <summary>
    /// Applies a force to the enemy.
    /// </summary>
    /// <param name="damageFrom">The enemy that is applying force.</param>
    public void ApplyEnemyForce(GameObject damageFrom)
    {
        float angleInRad = Mathf.Atan((damageFrom.transform.position.y - transform.position.y) / (damageFrom.transform.position.x - transform.position.x));
        float angleInDeg = Mathf.Rad2Deg * angleInRad;
        float collisionForce;
        try
        {
            collisionForce = (float)damageFrom.GetComponent<EnemyControl>().GetCollisionForce() / 2f;
        }
        catch(System.NullReferenceException)
        {
            collisionForce = (float)damageFrom.GetComponent<ShieldControl>().CollisionForce;
        }
        float xForce = collisionForce * Mathf.Cos(angleInRad);
        float yForce = collisionForce * Mathf.Sin(angleInRad);
        if (transform.position.x < damageFrom.transform.position.x)
        {
            xForce *= -1;
            yForce *= -1;
            angleInDeg += 180f;
        }

        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        Vector2 force = new Vector2(xForce, yForce);
        GetComponent<Rigidbody2D>().AddForce(force);
        transform.localEulerAngles = new Vector3(0, 0, angleInDeg);
        timeFromForce = 0;
        GetComponent<Rigidbody2D>().drag = 0.5f;
        hasPath = false;
    }
}
