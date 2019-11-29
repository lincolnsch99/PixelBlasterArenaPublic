/// File Name: StageSelectionUIControl.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: Handles displaying each available custom stage when selecting a stage
/// to play. Communicates with SaveData.cs so it can access all custom stages.
/// 
/// Date Last Updated: November 26, 2019

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectionUIControl : MonoBehaviour
{
    [SerializeField]
    private GameObject loadStageSinglePrefab;
    [SerializeField]
    private GameObject loadStageContentDisplay;
    [SerializeField]
    private Button squareStageSelect;
    [SerializeField]
    private Button crossStageSelect;

    private PersistentControl persistentController;

    /// <summary>
    /// Awake is called on the first frame of instantation.
    /// </summary>
    private void Awake()
    {
        persistentController = GameObject.FindWithTag("Persistent").GetComponent<PersistentControl>();
        squareStageSelect.onClick.AddListener(() => persistentController.SelectFromDefaultStages("Square"));
        squareStageSelect.onClick.AddListener(() => persistentController.PlayGame());
        crossStageSelect.onClick.AddListener(() => persistentController.SelectFromDefaultStages("Cross"));
        crossStageSelect.onClick.AddListener(() => persistentController.PlayGame());
        foreach (Transform child in loadStageContentDisplay.transform)
            Destroy(child.gameObject);
        List<Stage> customStages = SaveData.GetCustomStages();
        for (int i = 0; i < customStages.Count; i++)
        {
            Stage stage = customStages[i];
            GameObject thisStageDisplay = GameObject.Instantiate(loadStageSinglePrefab, loadStageContentDisplay.transform);
            thisStageDisplay.transform.localPosition = new Vector3(0, -(i * 25), 0);
            StageSelectionDisplay thisDisplayScript = thisStageDisplay.GetComponent<StageSelectionDisplay>();
            thisDisplayScript.SetStageName(stage.Name);
            thisDisplayScript.SetMultiplier(stage.SpawnMultiplier);
            thisDisplayScript.SetPowerupsSpawn(stage.CanPowerupsSpawn);
            thisStageDisplay.transform.GetChild(thisStageDisplay.transform.childCount - 1).GetComponent<Button>()
                .onClick.AddListener(() => persistentController.PlayGame());
        }
        loadStageContentDisplay.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 25 * customStages.Count);
    }

    /// <summary>
    /// Called when the player selects which stage they want to use.
    /// </summary>
    /// <param name="stageName">Name of the stage that the player selects.</param>
    public void SelectStage(string stageName)
    {
        List<Stage> customStages = SaveData.GetCustomStages();
        for (int i = 0; i < customStages.Count; i++)
        {
            if(customStages[i].Name == stageName)
            {
                persistentController.SelectedStage = customStages[i];
            }
        }
    }
}
