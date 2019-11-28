/// File Name: PlayerUI.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: Handles displaying the correct score and round to the player.
/// 
/// Date Last Updated: November 15, 2019

using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private GameObject scoreDisplay;
    [SerializeField]
    private GameObject roundDisplay;

    private PersistentControl persistentControl;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        persistentControl = GameObject.FindWithTag("Persistent").GetComponent<PersistentControl>();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        scoreDisplay.GetComponent<Text>().text = "Score: " + persistentControl.GetPlayerScore().ToString();
        roundDisplay.GetComponent<Text>().text = "Round: " + persistentControl.GetRound().ToString();
    }
}
