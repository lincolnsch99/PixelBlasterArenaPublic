using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
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
    private GameObject player;
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
    public void ApplyDamageForce(GameObject damageFrom)
    {
        float xHeight = damageFrom.transform.position.x - player.transform.position.x;
        float yHeight = damageFrom.transform.position.y - player.transform.position.y;
        float angleInRad = Mathf.Atan(yHeight / xHeight);
        int collisionForce = damageFrom.GetComponent<EnemyControl>().GetCollisionForce();

        angleInRad = Mathf.Deg2Rad * ((Mathf.Rad2Deg * angleInRad) + 180f);
        Vector2 force = new Vector2(collisionForce * Mathf.Cos(angleInRad),
            collisionForce * Mathf.Sin(angleInRad));
        player.GetComponent<Rigidbody2D>().AddForce(force);
    }
}
