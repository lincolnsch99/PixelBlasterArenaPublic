/// File Name: MenuButtoncs.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: MenuButtons is used for assigning listeners to UI buttons that rely
/// on the PersistentControl object.
/// 
/// Date Last Updated: November 12, 2019

using UnityEngine;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    public string ButtonType;
    public AudioClip UiClick;
    private PersistentControl PersistentInfo;

    /// <summary>
    /// Awake is called on the first frame of instantiation.
    /// </summary>
    private void Awake()
    {
        PersistentInfo = GameObject.FindWithTag("Persistent").GetComponent<PersistentControl>();
        Button thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(this.Click);
        if (ButtonType == "Resume")
            thisButton.onClick.AddListener(PersistentInfo.UnPauseGame);
        else if (ButtonType == "Menu")
            thisButton.onClick.AddListener(PersistentInfo.ToMainMenu);
        else if (ButtonType == "Options")
            thisButton.onClick.AddListener(PersistentInfo.SelectOptions);
        else if (ButtonType == "Quit")
            thisButton.onClick.AddListener(PersistentInfo.QuitGame);
        else if (ButtonType == "Play")
            thisButton.onClick.AddListener(PersistentInfo.PlayGame);
        else if (ButtonType == "Unpause")
            thisButton.onClick.AddListener(PersistentInfo.UnPauseGame);
        else if (ButtonType == "Tutorial")
            thisButton.onClick.AddListener(PersistentInfo.SelectTutorial);
        else if (ButtonType == "NormalPlayer")
            thisButton.onClick.AddListener(() => PersistentInfo.SelectPlayerType(PlayerType.NORMAL));
        else if (ButtonType == "BlasterPlayer")
            thisButton.onClick.AddListener(() => PersistentInfo.SelectPlayerType(PlayerType.BLASTER));
        else if (ButtonType == "StageCreator")
            thisButton.onClick.AddListener(() => PersistentInfo.LoadScene("StageCreator"));
        else if (ButtonType == "StageSelector")
            thisButton.onClick.AddListener(() => PersistentInfo.ToStageSelect());
        else
            Debug.LogError("Invalid ButtonType");
    }

    private void Click()
    {
        PersistentInfo.GetComponent<AudioSource>().PlayOneShot(UiClick);
    }

    private void OnDestroy()
    {
        Button thisButton = GetComponent<Button>();
        thisButton.onClick.RemoveAllListeners();
    }
}
