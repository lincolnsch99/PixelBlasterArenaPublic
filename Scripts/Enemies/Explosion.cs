/// File Name: Explosion.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: Very simple script used to remove the buster enemy's explosion once the 
/// animation has played out.
/// 
/// Date Last Updated: November 8, 2019

using UnityEngine;

public class Explosion : MonoBehaviour
{
    /// <summary>
    /// Removes this GameObject from the game.
    /// </summary>
    public void Despawn()
    {
        Destroy(this.gameObject);
    }
}
