using System.Collections;
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

    private void Awake()
    {
        persistentController = GameObject.FindWithTag("Persistent").GetComponent<PersistentControl>();
    }

    public void SetStageName(string stageName)
    {
        this.stageName = stageName;
        nameDisplay.text = stageName;
    }

    public void SetMultiplier(float multiplier)
    {
        this.multiplier = multiplier;
        multiplierDisplay.text = multiplier.ToString();
    }

    public void SetPowerupsSpawn(bool powerups)
    {
        this.powerups = powerups;
        if (powerups)
            powerupsDisplay.text = "Yes";
        else
            powerupsDisplay.text = "No";
    }

    public void LoadThisStage()
    {
        List<Stage> curStages = persistentController.CustomStages;
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

    public void SelectThisStage()
    {
        List<Stage> curStages = persistentController.CustomStages;
        foreach (Stage stage in curStages)
        {
            if (stage.Name == stageName)
                persistentController.SelectedStage = stage;
        }
    }
}
