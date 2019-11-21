using System.Collections;
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

    private void Awake()
    {
        persistentController = GameObject.FindWithTag("Persistent").GetComponent<PersistentControl>();
    }

    private void Update()
    {
        float.TryParse(multiplierDisplay.textComponent.text, out float tempFloat);
        if (nameDisplay.textComponent.text == "" || nameDisplay.textComponent.text == "Square"
            || nameDisplay.textComponent.text == "Cross" || multiplierDisplay.textComponent.text == ""
            || tempFloat < 1.1f || tempFloat > 2f)
        {
            saveButton.interactable = false;
        }
        else
        {
            saveButton.interactable = true;
        }
    }

    public void OpenLoadStageScreen()
    {
        foreach (Transform child in loadStageContentDisplay.transform)
            Destroy(child.gameObject);
        List<Stage> customStages = persistentController.CustomStages;
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

    public void StageChanged(string newStageName)
    {
        List<Stage> customStages = persistentController.CustomStages;
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
