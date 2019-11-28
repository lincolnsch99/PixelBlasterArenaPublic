/// File Name: StageCreationUIControl.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: Handles interactions between the player when naming their stage and giving 
/// statistics. Also handles the saving and loading of stages.
/// 
/// Date Last Updated: November 24, 2019

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageCreationUIControl : MonoBehaviour
{
    [SerializeField]
    private GameObject loadStageSinglePrefab;
    [SerializeField]
    private GameObject loadStageContentDisplay;
    [SerializeField]
    private InputField nameDisplay;
    [SerializeField]
    private InputField multiplierDisplay;
    [SerializeField]
    private Toggle powerupsDisplay;
    [SerializeField]
    private Button saveButton;

    private PersistentControl persistentController;

    /// <summary>
    /// Awake is called on the first frame of instantiation.
    /// </summary>
    private void Awake()
    {
        persistentController = GameObject.FindWithTag("Persistent").GetComponent<PersistentControl>();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        float.TryParse(multiplierDisplay.textComponent.text, out float tempFloat);
        if (nameDisplay.textComponent.text == "" || nameDisplay.textComponent.text == "Square"
            || nameDisplay.textComponent.text == "Cross" || multiplierDisplay.textComponent.text == ""
            || tempFloat < 1.1f || tempFloat > 5f)
        {
            saveButton.interactable = false;
        }
        else
        {
            saveButton.interactable = true;
        }
    }

    /// <summary>
    /// Displays the load screen window, refreshing the list of possible stages to choose from.
    /// </summary>
    public void OpenLoadStageScreen()
    {
        foreach (Transform child in loadStageContentDisplay.transform)
            Destroy(child.gameObject);
        List<Stage> customStages = SaveData.GetCustomStages();
        for(int i = 0; i < customStages.Count; i++)
        {
            Stage stage = customStages[i];
            GameObject thisStageDisplay = GameObject.Instantiate(loadStageSinglePrefab, loadStageContentDisplay.transform);
            thisStageDisplay.transform.localPosition = new Vector3(0, -(i * 25), 0);
            StageSelectionDisplay thisDisplayScript = thisStageDisplay.GetComponent<StageSelectionDisplay>();
            thisDisplayScript.SetStageName(stage.Name);
            thisDisplayScript.SetMultiplier(stage.SpawnMultiplier);
            thisDisplayScript.SetPowerupsSpawn(stage.CanPowerupsSpawn);
        }
        loadStageContentDisplay.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 25 * customStages.Count);
    }

    /// <summary>
    /// Saves the current stage that is being edited/displayed on screen.
    /// </summary>
    public void SaveCurrentStage()
    {
        Stage stageToSave = new Stage();
        stageToSave.Boundary = new BoundaryMap(GetComponent<BoundaryDrawer>().GetMap());
        stageToSave.Name = nameDisplay.textComponent.text;
        float.TryParse(multiplierDisplay.textComponent.text, out float tempFloat);
        stageToSave.SpawnMultiplier = tempFloat;
        stageToSave.CanPowerupsSpawn = powerupsDisplay.isOn;
        persistentController.SaveCustomStage(stageToSave);
    }

    /// <summary>
    /// Updates the display to show the new current stage.
    /// </summary>
    /// <param name="newStageName">New stage to be displayed.</param>
    public void StageChanged(string newStageName)
    {
        List<Stage> customStages = SaveData.GetCustomStages();
        for(int i = 0; i < customStages.Count; i++)
        {
            if(customStages[i].Name == newStageName)
            {
                nameDisplay.SetTextWithoutNotify(newStageName);
                multiplierDisplay.SetTextWithoutNotify(customStages[i].SpawnMultiplier.ToString());
                powerupsDisplay.isOn = customStages[i].CanPowerupsSpawn;
            }
        }
    }
}
