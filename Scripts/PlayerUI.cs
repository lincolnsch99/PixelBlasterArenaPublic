using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private GameObject scoreDisplay;
    [SerializeField]
    private GameObject roundDisplay;

    private PersistentControl persistentControl;

    // Start is called before the first frame update
    void Start()
    {
        persistentControl = GameObject.FindWithTag("Persistent").GetComponent<PersistentControl>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreDisplay.GetComponent<Text>().text = "Score: " + persistentControl.GetPlayerScore().ToString();
        roundDisplay.GetComponent<Text>().text = "Round: " + persistentControl.GetRound().ToString();
    }
}
