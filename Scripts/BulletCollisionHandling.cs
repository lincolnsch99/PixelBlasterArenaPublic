using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollisionHandling : MonoBehaviour
{
    private int damage = 1;

    /// <summary>
    /// When the bullet collides with an object, it checks if it is an enemy. If so, damage the enemy.
    /// The bullet is destroyed if it collides with a boundary wall.
    /// </summary>
    /// <param name="collision">The collider that the bullet has collided with.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collidedObject = collision.gameObject;
        if(collidedObject.tag == "Enemy")
        {
            for(int i = 0; i < damage; i++)
                collidedObject.BroadcastMessage("TakeDamage", SendMessageOptions.DontRequireReceiver);
            Destroy(this.gameObject);
        }
        else if(collidedObject.tag == "Boundary")
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Setter for the damage of this bullet.
    /// </summary>
    /// <param name="damage">The desired amount of damage.</param>
    public void SetDamage(int damage)
    {
        this.damage = damage;
    }
}
