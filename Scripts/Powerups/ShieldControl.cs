/// File Name: ShieldControl.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: Handles the actions/movement necessary for the shield powerup.
/// 
/// Date Last Updated: November 25, 2019

using UnityEngine;

public class ShieldControl : MonoBehaviour
{
    [SerializeField]
    private int collisionForce;
    public int CollisionForce { get { return collisionForce; } set { collisionForce = value; } }

    [SerializeField]
    private int lifetime;

    [Header("Sounds")]
    [SerializeField]
    private AudioClip contactSound;

    private GameObject player;
    private float timer;
    private PersistentControl persistentController;

    /// <summary>
    /// Awake is called on the first frame of instantiation.
    /// </summary>
    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        persistentController = GameObject.FindWithTag("Persistent").GetComponent<PersistentControl>();
        transform.position = player.transform.position;
        timer = 0;
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        transform.position = player.transform.position;
        transform.localEulerAngles = player.transform.localEulerAngles;

        timer += Time.deltaTime;
        if (timer >= lifetime)
            Destroy(this.gameObject);
    }

    /// <summary>
    /// When an enemy enters the trigger radius, push it away.
    /// </summary>
    /// <param name="collision">The collider that has collided with our trigger.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            collision.GetComponent<EnemyControl>().ApplyEnemyForce(this.gameObject);
            persistentController.GetComponent<AudioSource>().PlayOneShot(contactSound);
        }
    }
}
