/// File Name: PlayerControl.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: This script handles the player movement, along with any interactions that
/// happen from taking damage or firing the weapon.
/// 
/// Date Last Updated: November 12, 2019

using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    [SerializeField]
    private Camera playerCam;
    [SerializeField]
    private GameObject bulletPrefab;

    private PlayerType playerType;
    private float movementForce;
    private float bulletSpeed;
    private float bulletRecoilForce;
    private float maxFireRate;
    private Vector2 velocityVector;
    private static GameObject player;
    private float fireTimer;
    private PersistentControl persistentController;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        persistentController = GameObject.FindWithTag("Persistent").GetComponent<PersistentControl>();
        velocityVector = new Vector2();
        playerType = persistentController.GetPlayerType();
        switch(playerType)
        {
            case PlayerType.NORMAL:
                player.transform.GetChild(0).gameObject.SetActive(true);
                player.transform.GetChild(1).gameObject.SetActive(false);
                movementForce = 15;
                bulletSpeed = 30;
                bulletRecoilForce = 1000;
                maxFireRate = 0.2f;
                break;
            case PlayerType.BLASTER:
                player.transform.GetChild(0).gameObject.SetActive(false);
                player.transform.GetChild(1).gameObject.SetActive(true);
                movementForce = 0;
                bulletSpeed = 15;
                bulletRecoilForce = 2000;
                maxFireRate = 0.1f;
                break;
        }
    }

    private void Awake()
    {
        Cursor.visible = false;
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
            persistentController.GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
        }
        if (playerType == PlayerType.NORMAL)
        {
            if (Input.GetKey(KeyCode.W))
                movementVec.y = movementForce;
            else if (Input.GetKey(KeyCode.S))
                movementVec.y = -movementForce;
            if (Input.GetKey(KeyCode.D))
                movementVec.x = movementForce;
            else if (Input.GetKey(KeyCode.A))
                movementVec.x = -movementForce;
        }

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
        ApplyKickbackForce(Mathf.Deg2Rad * rotateAngle);

        if (playerType == PlayerType.BLASTER)
        {
            angle = Mathf.Atan((mousePos.y - player.transform.position.y) / (mousePos.x - player.transform.position.x));
            angle += Mathf.Deg2Rad * 55f;
            xVelocity = bulletSpeed * Mathf.Cos(angle);
            yVelocity = bulletSpeed * Mathf.Sin(angle);
            rotateAngle = Mathf.Rad2Deg * angle;
            if (mousePos.x < player.transform.position.x)
            {
                xVelocity *= -1;
                yVelocity *= -1;
                rotateAngle += 180;
            }
            velocityVector = new Vector2(xVelocity, yVelocity);
            bullet = GameObject.Instantiate(bulletPrefab, player.transform.position, player.transform.rotation);
            bullet.GetComponent<Rigidbody2D>().velocity = velocityVector;

            angle = Mathf.Atan((mousePos.y - player.transform.position.y) / (mousePos.x - player.transform.position.x));
            angle -= Mathf.Deg2Rad * 55f;
            xVelocity = bulletSpeed * Mathf.Cos(angle);
            yVelocity = bulletSpeed * Mathf.Sin(angle);
            rotateAngle = Mathf.Rad2Deg * angle;
            if (mousePos.x < player.transform.position.x)
            {
                xVelocity *= -1;
                yVelocity *= -1;
                rotateAngle += 180;
            }
            velocityVector = new Vector2(xVelocity, yVelocity);
            bullet = GameObject.Instantiate(bulletPrefab, player.transform.position, player.transform.rotation);
            bullet.GetComponent<Rigidbody2D>().velocity = velocityVector;
        }
    }

    /// <summary>
    /// Applies a force to the player. Used when the player shoots, so it uses the specified kickback force.
    /// </summary>
    /// <param name="fireAngle">The angle that the player is firing towards.</param>
    public void ApplyKickbackForce(float fireAngle)
    {
        fireAngle = Mathf.Deg2Rad * ((Mathf.Rad2Deg * fireAngle) + 180f);
        Vector2 force = new Vector2(bulletRecoilForce * Mathf.Cos(fireAngle),
            bulletRecoilForce * Mathf.Sin(fireAngle));
        player.GetComponent<Rigidbody2D>().AddForce(force);
    }

    /// <summary>
    /// Applies a force to the player. Used when the player is damaged by an enemy, so it uses the enemy's 
    /// specific force magnitude.
    /// </summary>
    /// <param name="damageFrom">The enemy that is damaging the player.</param>
    public void ApplyEnemyForce(GameObject damageFrom)
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
