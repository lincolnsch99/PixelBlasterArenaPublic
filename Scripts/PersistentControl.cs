using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PersistentControl : MonoBehaviour
{
    private bool PAUSED;
    private int playerScore;

    // Can be implemented later.
    /*
    [SerializeField]
    private GameObject GameOverScreenPrefab;
    [SerializeField]
    private GameObject GameWonScreenPrefab;
    */

    private GameObject loadScreen;
    private Slider loadScreenProgress;
    private bool routineRunning;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        routineRunning = false;
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        // At the very beginning, the initial scene only contains the PersistentController Object, so we need to load
        // the main menu.
        if (SceneManager.GetActiveScene().buildIndex == 0)
            SceneManager.LoadScene("MainMenu");

        // Every scene needs a gameobject containing the loadscreen. It will be used by this persistent script in order to 
        // appear and show progress whenever a scene is being loaded. This is because this does not work with prefabs.
        GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].tag == "LoadScreen")
            {
                loadScreen = objects[i];
                i = objects.Length;
            }
        }
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].tag == "LoadScreenSlider")
            {
                loadScreenProgress = objects[i].GetComponent<Slider>();
                i = objects.Length;
            }

        }
        PAUSED = false;
    }

    /// <summary>
    /// Update is called every frame.
    /// </summary>
    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (PAUSED)
                {
                    UnPauseGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }
    }

    // The next 3 functions are specific to Persistent objects, to help the object know when they are
    // transitioning to new scenes, and reset some of the variables each time.

    /// <summary>
    /// Called when a scene is being loaded.
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Called every time a new scene has been loaded. Resets necessary variables and finsd necessary objects.
    /// </summary>
    /// <param name="scene">Scene loaded</param>
    /// <param name="loadSceneMode">Mode in which the scene is loaded (this program uses asynchronous loading)</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        DontDestroyOnLoad(this.gameObject);

        if (SceneManager.GetActiveScene().name != "InitScene")
        {
            // Every scene needs a gameobject containing the loadscreen. It will be used by this persistent script in order to 
            // appear and show progress whenever a scene is being loaded. This is because this does not work with prefabs.
            GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].tag == "LoadScreen")
                {
                    loadScreen = objects[i];
                    i = objects.Length;
                }
            }
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].tag == "LoadScreenSlider")
                {
                    loadScreenProgress = objects[i].GetComponent<Slider>();
                    i = objects.Length;
                }
            }
        }
    }

    /// <summary>
    /// Called when exiting the current scene.
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Begins the process to load the desired scene.
    // @param sceneName: string, the name of the scene to be loaded.
    public void LoadScene(string sceneName)
    {
        PAUSED = false;
        Time.timeScale = 1f;
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    // Handles the fade in and loading bar for the loading screen.
    // @param sceneName: string, the name of the scene to be loaded.
    IEnumerator LoadSceneAsync(string sceneName)
    {
        routineRunning = true;
        if (loadScreen != null)
            loadScreen.SetActive(true);
        Color temp = loadScreen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
        temp.a = 0F;
        loadScreen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = temp;
        loadScreenProgress.gameObject.SetActive(false);

        while (loadScreen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color.a < 1.0F)
        {
            Color curTransparency = loadScreen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
            curTransparency.a += (Time.deltaTime / 1F) / 1.0F;
            if (curTransparency.a > 1.0F)
                curTransparency.a = 1.0F;
            loadScreen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = curTransparency;
            yield return null;
        }

        loadScreenProgress.gameObject.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9F);
            if (loadScreenProgress != null)
                loadScreenProgress.value = progress;
            yield return null;
        }
        routineRunning = false;
    }

    // When the player uses a "Quit" button, this method will be called. It is possible to add
    // necessary processes that must be executed before the application quits, such as saving 
    // data into files, making sure it is safe to quit from this point, etc. This method IS NOT
    // called when the player uses other methods to close the application, such as 'ALT+F4', or
    // the Task Manager.
    public void QuitGame()
    {
        Application.Quit();
    }

    // Specfic function to load the main game scene.
    public void PlayGame()
    {
        UnPauseGame();
        LoadScene("Game");
    }

    // For the main menu, displays the options screen and disables other screens.
    public void SelectOptions()
    {
        DisplayMenuElement(2);
    }

    // For the main menu, displays the tutorial screen and disables other screens.
    public void SelectTutorial()
    {
        DisplayMenuElement(3);
    }

    // Navigates the player to the main menu, by either loading the main menu scene or switching to the 
    // main menu screen.
    public void ToMainMenu()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            UnPauseGame();
            Cursor.visible = true;
            LoadScene("MainMenu");
        }
        else
            DisplayMenuElement(1);
    }

    // In the list of menu types, displays the desired menu.
    // @param index: int, the index of the menu to be added.
    private void DisplayMenuElement(int index)
    {
        GameObject menuObject = GameObject.FindWithTag("MainMenu");
        int numChildren = menuObject.transform.childCount;
        if (index < numChildren)
        {
            for (int i = 0; i < numChildren; i++)
            {
                if (i == index)
                    menuObject.transform.GetChild(i).gameObject.SetActive(true);
                else if (i != 0) // The background shouldn't be deleted when going through the menu.
                    menuObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
            Debug.LogError("Attempted to load invalid menu index");
    }

    // Increments the player score by the desired amount.
    // @param amount: int, the amount the score should be incremented (or decremented, if negative).
    public void IncrementPlayerScore(int amount)
    {
        playerScore += amount;
    }

    // "Pauses" the game. Time.timeScale changes the speed at which processes are executed, so setting it
    // to 0 will stop any actions that rely on time. Other scripts that run at runtime should check 
    // if(PersistentDataObject.IsPaused()) in their update function, so that when the game is paused, 
    // desired scripts will stop what they're doing. Also enables the cursor to let the player navigate the
    // pause menu.
    public void PauseGame()
    {
        PAUSED = true;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    // "Unpauses" the game. Reset Time.timescale back to normal and makes the cursor invisible again.
    public void UnPauseGame()
    {
        PAUSED = false;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    // Lets other scripts know if the game is paused or not.
    // @return PAUSED: bool, true if the game is paused, false otherwise.
    public bool IsPaused()
    {
        return PAUSED;
    }

    // Getter for playerScore.
    // @return playerScore: int.
    public int GetPlayerScore()
    {
        return playerScore;
    }

    // Plays the button click sound. Currently there is no sound.
    public void Click()
    {
        //GetComponent<AudioSource>().Play();
    }

    // These next methods are not applicable yet, but can eventually be implemented. They simply display the
    // screens that let the player know if they won or lost. These screens can have the player's final score, maybe
    // a leaderboard, whatever. Right now, they are not being used.

    /*
    public void GameLost()
    {
        if (!routineRunning)
            StartCoroutine(DisplayGameOver());
    }

    public void GameWon()
    {
        if (!routineRunning)
            StartCoroutine(DisplayWon());
    }

    IEnumerator DisplayGameOver()
    {
        routineRunning = true;
        PAUSED = false;
        Time.timeScale = 1f;
        float timeElapsed = 0.0f;
        GameObject screen = GameObject.Instantiate(GameOverScreenPrefab);
        Color temp = screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
        temp.a = 0F;
        screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = temp;
        screen.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);

        while (screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color.a < 1.0F)
        {
            Color curTransparency = screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
            curTransparency.a += (Time.deltaTime / 1F) / 4.5F;
            if (curTransparency.a > 1.0F)
                curTransparency.a = 1.0F;
            screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = curTransparency;

            yield return null;
        }

        screen.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);

        while (timeElapsed < 3f)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        routineRunning = false;
        ToMainMenu();
    }

    IEnumerator DisplayWon()
    {
        routineRunning = true;
        PAUSED = false;
        Time.timeScale = 1f;
        float timeElapsed = 0.0f;
        GameObject screen = GameObject.Instantiate(GameWonScreenPrefab);
        Color temp = screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
        temp.a = 0F;
        screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = temp;
        screen.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);

        while (screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color.a < 1.0F)
        {
            Color curTransparency = screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
            curTransparency.a += (Time.deltaTime / 1F) / 3.0F;
            if (curTransparency.a > 1.0F)
                curTransparency.a = 1.0F;
            screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = curTransparency;
            yield return null;
        }

        screen.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);

        while (timeElapsed < 3f)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        routineRunning = false;
        ToMainMenu();
    }
    */
}
