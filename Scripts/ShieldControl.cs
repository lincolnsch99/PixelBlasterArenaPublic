using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldControl : MonoBehaviour
{
    [SerializeField]
    private int collisionForce;
    public int CollisionForce { get { return collisionForce; } set { collisionForce = value; } }

    [SerializeField]
    private int lifetime;

    private GameObject player;
    private float timer;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        transform.position = player.transform.position;
        timer = 0;
    }

    private void Update()
    {
        transform.position = player.transform.position;
        transform.localEulerAngles = player.transform.localEulerAngles;

        timer += Time.deltaTime;
        if (timer >= lifetime)
            Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            collision.GetComponent<EnemyControl>().ApplyEnemyForce(this.gameObject);
        }
    }
}
