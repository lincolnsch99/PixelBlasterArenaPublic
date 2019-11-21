using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreenControl : MonoBehaviour
{
    private PersistentControl persistentController;

    private void Start()
    {
        persistentController = GameObject.FindWithTag("Persistent").GetComponent<PersistentControl>();
    }

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
