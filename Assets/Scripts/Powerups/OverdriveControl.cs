/// File Name: EnemyControl.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: Every GameObject that is representing an enemy has this script attached. This script is used
/// for monitoring and changing enemy behavior. It also stores necessary data for the enemy, such as health and
/// damage.
/// 
/// Date Last Updated: November 26, 2019

using UnityEngine;

public class OverdriveControl : MonoBehaviour
{
    [SerializeField]
    private int lifetime;

    private PlayerControl playerController;
    private float timer;

    /// <summary>
    /// Awake is called on the first frame of instantation.
    /// </summary>
    private void Awake()
    {
        playerController = GameObject.Find("PlayerController").GetComponent<PlayerControl>();
        playerController.OverdriveActive = true;
        timer = 0;
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            playerController.OverdriveActive = false;
            Destroy(this.gameObject);
        }
        else
            playerController.OverdriveActive = true;
    }
}
