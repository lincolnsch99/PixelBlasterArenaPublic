/// File Name: PlayerControl.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: This script handles the player movement, along with any interactions that
/// happen from taking damage or firing the weapon.
/// 
/// Date Last Updated: November 8, 2019

using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private Camera playerCam;
    [SerializeField]
    private float movementForce;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private float bulletSpeed;
    [SerializeField]
    private float bulletKickbackForce;
    [SerializeField]
    private float maxFireRate;

    private Vector2 velocityVector;
    private static GameObject player;
    private float fireTimer;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        velocityVector = new Vector2();
    }

    /// <summary>
    /// Update is called every frame.
    /// </summary>
    private void Update()
    {
        fireTimer += Time.deltaTime;
        Vector2 mousePos = playerCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 movementVec = new Vector2(0, 0);
        AlignRotation(mousePos);
        if(Input.GetMouseButtonDown(0) && fireTimer >= maxFireRate && player.activeSelf)
        {
            fireTimer = 0;
            FireProjectile(mousePos);
        }

        if (Input.GetKey(KeyCode.W))
            movementVec.y = movementForce;
        else if (Input.GetKey(KeyCode.S))
            movementVec.y = -movementForce;
        if (Input.GetKey(KeyCode.D))
            movementVec.x = movementForce;
        else if (Input.GetKey(KeyCode.A))
            movementVec.x = -movementForce;

        player.GetComponent<Rigidbody2D>().AddForce(movementVec);
    }

    /// <summary>
    /// Rotates the player model so that it points to where the mouse is on screen.
    /// </summary>
    /// <param name="mousePos">The current mouse position.</param>
    private void AlignRotation(Vector2 mousePos)
    {
        float angle = Mathf.Atan((mousePos.y - player.transform.position.y) / (mousePos.x - player.transform.position.x));
        angle *= Mathf.Rad2Deg;
        Vector3 rotate = new Vector3(0, 0, angle);
        if (mousePos.x > player.transform.position.x)
        {
            rotate.z += 180;
        }
        player.transform.localEulerAngles = rotate;
    }

    /// <summary>
    /// Fires a bullet projectile towards the current mouse position.
    /// </summary>
    /// <param name="mousePos">The current mouse position.</param>
    private void FireProjectile(Vector2 mousePos)
    {
        float angle = Mathf.Atan((mousePos.y - player.transform.position.y) / (mousePos.x - player.transform.position.x));
        float xVelocity = bulletSpeed * Mathf.Cos(angle);
        float yVelocity = bulletSpeed * Mathf.Sin(angle);
        float rotateAngle = Mathf.Rad2Deg * angle;
        if (mousePos.x < player.transform.position.x)
        {
            xVelocity *= -1;
            yVelocity *= -1;
            rotateAngle += 180;
        }
        velocityVector = new Vector2(xVelocity, yVelocity);
        GameObject bullet = GameObject.Instantiate(bulletPrefab, player.transform.position, player.transform.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = velocityVector;
        bullet.transform.localEulerAngles = new Vector3(0, 0, rotateAngle);
        ApplyKickbackForce(Mathf.Deg2Rad * rotateAngle);
    }

    /// <summary>
    /// Applies a force to the player. Used when the player shoots, so it uses the specified kickback force.
    /// </summary>
    /// <param name="fireAngle">The angle that the player is firing towards.</param>
    public void ApplyKickbackForce(float fireAngle)
    {
        fireAngle = Mathf.Deg2Rad * ((Mathf.Rad2Deg * fireAngle) + 180f);
        Vector2 force = new Vector2(bulletKickbackForce * Mathf.Cos(fireAngle),
            bulletKickbackForce * Mathf.Sin(fireAngle));
        player.GetComponent<Rigidbody2D>().AddForce(force);
    }

    /// <summary>
    /// Applies a force to the player. Used when the player is damaged by an enemy, so it uses the enemy's 
    /// specific force magnitude.
    /// </summary>
    /// <param name="damageFrom">The enemy that is damaging the player.</param>
    public static void ApplyDamageForce(GameObject damageFrom)
    {
        float angleInRad = Mathf.Atan((damageFrom.transform.position.y - player.transform.position.y) / (damageFrom.transform.position.x - player.transform.position.x));
        int collisionForce = damageFrom.GetComponent<EnemyControl>().GetCollisionForce();
        float xForce = collisionForce * Mathf.Cos(angleInRad);
        float yForce = collisionForce * Mathf.Sin(angleInRad);
        if (player.transform.position.x < damageFrom.transform.position.x)
        {
            xForce *= -1;
            yForce *= -1;
        }

        Vector2 force = new Vector2(xForce, yForce);
        player.GetComponent<Rigidbody2D>().AddForce(force);
    }
}
