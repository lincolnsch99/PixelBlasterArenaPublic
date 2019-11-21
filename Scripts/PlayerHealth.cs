/// File Name: PlayerHealth.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: Simple script used for monitoring and updating the player's health.
/// 
/// Date Last Updated: November 8, 2019

using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private GameObject player;
    
    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    /// <summary>
    /// Disables the player GameObject.
    /// </summary>
    private void Kill()
    {
        GameObject.FindWithTag("Persistent").GetComponent<PersistentControl>().GameLost();
        player.SetActive(false);
    }

    /// <summary>
    /// Re-enables the player and sets the player's position to 0,0. Also gives the player a full life pool.
    /// </summary>
    private void Revive()
    {
        player.transform.position = new Vector2(0, 0);
        player.SetActive(true);
    }
}
