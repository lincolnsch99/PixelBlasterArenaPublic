/// File Name: PauseScreenControl.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: Handles the displaying of the pause screen.
/// 
/// Date Last Updated: November 27, 2019

using UnityEngine;

public class PauseScreenControl : MonoBehaviour
{
    private PersistentControl persistentController;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        persistentController = GameObject.FindWithTag("Persistent").GetComponent<PersistentControl>();
    }

    /// <summary>
    /// Update is called every frame.
    /// </summary>
    private void Update()
    {
        if (persistentController.IsPaused())
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
            transform.GetChild(0).gameObject.SetActive(false);
    }
}
