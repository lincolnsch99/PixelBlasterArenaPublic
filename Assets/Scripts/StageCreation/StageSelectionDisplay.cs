/// File Name: StageSelectionDisplay.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: This script is attached to each individual stage choice that is
/// displayed on screen.
/// 
/// Date Last Updated: November 24, 2019

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectionDisplay : MonoBehaviour
{
    public string stageName;
    public float multiplier;
    public bool powerups;

    [SerializeField]
    private Text nameDisplay;
    [SerializeField]
    private Text multiplierDisplay;
    [SerializeField]
    private Text powerupsDisplay;

    private PersistentControl persistentController;

    /// <summary>
    /// Awake is called on the first frame of instantiation.
    /// </summary>
    private void Awake()
    {
        persistentController = GameObject.FindWithTag("Persistent").GetComponent<PersistentControl>();
    }

    /// <summary>
    /// Sets the name representing the stage choice, updates the UI to match.
    /// </summary>
    /// <param name="stageName">The name of the stage.</param>
    public void SetStageName(string stageName)
    {
        this.stageName = stageName;
        nameDisplay.text = stageName;
    }

    /// <summary>
    /// Sets the multiplier representing the stage choice, updates the UI to match.
    /// </summary>
    /// <param name="multiplier">The multiplier of the stage.</param>
    public void SetMultiplier(float multiplier)
    {
        this.multiplier = multiplier;
        multiplierDisplay.text = multiplier.ToString();
    }

    /// <summary>
    /// Sets whether or not powerups will spawn on the stage, updates UI to match.
    /// </summary>
    /// <param name="powerups">True if powerups can spawn, false otherwise.</param>
    public void SetPowerupsSpawn(bool powerups)
    {
        this.powerups = powerups;
        if (powerups)
            powerupsDisplay.text = "Yes";
        else
            powerupsDisplay.text = "No";
    }

    /// <summary>
    /// Called when the player selects this stage in the stage creator.
    /// </summary>
    public void LoadThisStage()
    {
        List<Stage> curStages = SaveData.GetCustomStages();
        Stage selectedStage;
        foreach(Stage stage in curStages)
        {
            if (stage.Name == stageName)
            {
                selectedStage = stage;
                GameObject.FindWithTag("StageHolder").GetComponent<BoundaryDrawer>().SetMap(stage.Boundary);
                GameObject.FindWithTag("StageHolder").GetComponent<BoundaryDrawer>().DisplayMap();
                GameObject.FindWithTag("StageHolder").GetComponent<StageCreationUIControl>().StageChanged(stageName);
            }
        }
    }

    /// <summary>
    /// Called when the player selects this stage in the stage select.
    /// </summary>
    public void SelectThisStage()
    {
        List<Stage> curStages = SaveData.GetCustomStages();
        foreach (Stage stage in curStages)
        {
            if (stage.Name == stageName)
                persistentController.SelectedStage = stage;
        }
    }
}
