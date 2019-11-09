/// File Name: PlayerHealth.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: Simple script used for monitoring and updating the player's health.
/// 
/// Date Last Updated: November 8, 2019

using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private int startingLives;

    private int curLives;
    private GameObject player;
    
    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        curLives = startingLives;
        player = GameObject.FindWithTag("Player");
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Revive();
        }
    }

    /// <summary>
    /// Subtracts one life from the player's current life pool.
    /// </summary>
    public void TakeDamage()
    {
        curLives--;
    }

    /// <summary>
    /// Removes all life from the player and disables the player GameObject.
    /// </summary>
    public void InstantKill()
    {
        curLives = 0;
        Kill();
    }

    /// <summary>
    /// Adds the specified amount of lives to the player's life pool.
    /// </summary>
    /// <param name="lives">The number of lives being added.</param>
    public void AddLives(int lives)
    {
        curLives += lives;
    }

    /// <summary>
    /// Disables the player GameObject.
    /// </summary>
    private void Kill()
    {
        player.SetActive(false);
    }

    /// <summary>
    /// Re-enables the player and sets the player's position to 0,0. Also gives the player a full life pool.
    /// </summary>
    private void Revive()
    {
        player.transform.position = new Vector2(0, 0);
        player.SetActive(true);
        curLives = startingLives;
    }
}
