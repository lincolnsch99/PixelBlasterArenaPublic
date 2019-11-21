using System.Collections;
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

    private void Awake()
    {
        persistentController = GameObject.FindWithTag("Persistent").GetComponent<PersistentControl>();
        squareStageSelect.onClick.AddListener(() => persistentController.SelectFromDefaultStages("Square"));
        squareStageSelect.onClick.AddListener(() => persistentController.ToPlayerSelect());
        crossStageSelect.onClick.AddListener(() => persistentController.SelectFromDefaultStages("Cross"));
        crossStageSelect.onClick.AddListener(() => persistentController.ToPlayerSelect());
        foreach (Transform child in loadStageContentDisplay.transform)
            Destroy(child.gameObject);
        List<Stage> customStages = persistentController.CustomStages;
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
                .onClick.AddListener(() => persistentController.ToPlayerSelect());
        }
        loadStageContentDisplay.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 25 * customStages.Count);
    }

    public void SelectStage(string stageName)
    {
        List<Stage> customStages = persistentController.CustomStages;
        for (int i = 0; i < customStages.Count; i++)
        {
            if(customStages[i].Name == stageName)
            {
                persistentController.SelectedStage = customStages[i];
            }
        }
    }
}
