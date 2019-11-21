using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameData
{
    private int allTimeHighScore;
    public int AllTimeHighScore { get { return allTimeHighScore; } set { allTimeHighScore = value; } }

    private int allTimeHighRound;
    public int AllTimeHighRound { get { return allTimeHighRound; } set { allTimeHighRound = value; } }

    private List<Stage> defaultStages;
    public List<Stage> DefaultStages { get { return defaultStages; } set { defaultStages = value; } }

    private List<Stage> customStages;
    public List<Stage> CustomStages { get { return customStages; } set { customStages = value; } }

    private int mouseSens;
    public int MouseSens { get { return mouseSens; } set { mouseSens = value; } }

    private int sfxVolume;
    public int SfxVolume { get { return sfxVolume; } set { sfxVolume = value; } }

    private int musicVolume;
    public int MusicVolume { get { return musicVolume; } set { musicVolume = value; } }

    public GameData(PersistentControl info)
    {
        allTimeHighScore = info.AllTimeHighScore;
        allTimeHighRound = info.AllTimeHighRound;
        defaultStages = GenerateDefaultStages();
        customStages = info.CustomStages;
        mouseSens = info.MouseSens;
        sfxVolume = info.SfxVolume;
        musicVolume = info.MusicVolume;
    }

    public GameData()
    {
        allTimeHighScore = 0;
        allTimeHighRound = 0;
        defaultStages = GenerateDefaultStages();
        customStages = new List<Stage>();
        mouseSens = 5;
        sfxVolume = 100;
        musicVolume = 100;
    }

    private List<Stage> GenerateDefaultStages()
    {
        List<Stage> defaults = new List<Stage>();
        Stage squareStage = new Stage();
        squareStage.Name = "Square";
        squareStage.SpawnMultiplier = 1.2f;
        squareStage.CanPowerupsSpawn = true;
        BoundaryMap squareStageMap = new BoundaryMap(20, 20);
        squareStageMap.UpdateSpace(19, 19, SingleSpaceType.CORNER_1Q);
        squareStageMap.UpdateSpace(0, 19, SingleSpaceType.CORNER_2Q);
        squareStageMap.UpdateSpace(0, 0, SingleSpaceType.CORNER_3Q);
        squareStageMap.UpdateSpace(19, 0, SingleSpaceType.CORNER_4Q);
        for (int i = 1; i < 19; i++)
        {
            squareStageMap.UpdateSpace(0, i, SingleSpaceType.VERTICAL_STRAIGHT);
            squareStageMap.UpdateSpace(19, i, SingleSpaceType.VERTICAL_STRAIGHT);
            squareStageMap.UpdateSpace(i, 19, SingleSpaceType.HORIZONTAL_STRAIGHT);
            squareStageMap.UpdateSpace(i, 0, SingleSpaceType.HORIZONTAL_STRAIGHT);
        }
        squareStage.Boundary = squareStageMap;

        Stage crossStage = new Stage();
        crossStage.Name = "Cross";
        crossStage.SpawnMultiplier = 1.2f;
        crossStage.CanPowerupsSpawn = true;
        BoundaryMap crossStageMap = new BoundaryMap(20, 20);
        crossStageMap.UpdateSpace(0, 5, SingleSpaceType.CORNER_3Q);
        crossStageMap.UpdateSpace(5, 5, SingleSpaceType.CORNER_1Q);
        crossStageMap.UpdateSpace(5, 0, SingleSpaceType.CORNER_3Q);
        crossStageMap.UpdateSpace(0, 14, SingleSpaceType.CORNER_2Q);
        crossStageMap.UpdateSpace(5, 14, SingleSpaceType.CORNER_4Q);
        crossStageMap.UpdateSpace(5, 19, SingleSpaceType.CORNER_2Q);
        crossStageMap.UpdateSpace(19, 14, SingleSpaceType.CORNER_1Q);
        crossStageMap.UpdateSpace(14, 14, SingleSpaceType.CORNER_3Q);
        crossStageMap.UpdateSpace(14, 19, SingleSpaceType.CORNER_1Q);
        crossStageMap.UpdateSpace(19, 5, SingleSpaceType.CORNER_4Q);
        crossStageMap.UpdateSpace(14, 5, SingleSpaceType.CORNER_2Q);
        crossStageMap.UpdateSpace(14, 0, SingleSpaceType.CORNER_4Q);
        crossStage.Boundary = crossStageMap;
        for(int i = 6; i < 14; i++)
        {
            crossStageMap.UpdateSpace(0, i, SingleSpaceType.VERTICAL_STRAIGHT);
            crossStageMap.UpdateSpace(19, i, SingleSpaceType.VERTICAL_STRAIGHT);
            crossStageMap.UpdateSpace(i, 0, SingleSpaceType.HORIZONTAL_STRAIGHT);
            crossStageMap.UpdateSpace(i, 19, SingleSpaceType.HORIZONTAL_STRAIGHT);
        }
        for(int i = 1; i < 5; i++)
        {
            crossStageMap.UpdateSpace(i, 5, SingleSpaceType.HORIZONTAL_STRAIGHT);
            crossStageMap.UpdateSpace(5, i, SingleSpaceType.VERTICAL_STRAIGHT);
            crossStageMap.UpdateSpace(i + 14, 5, SingleSpaceType.HORIZONTAL_STRAIGHT);
            crossStageMap.UpdateSpace(14, i, SingleSpaceType.VERTICAL_STRAIGHT);
            crossStageMap.UpdateSpace(i, 14, SingleSpaceType.HORIZONTAL_STRAIGHT);
            crossStageMap.UpdateSpace(5, i + 14, SingleSpaceType.VERTICAL_STRAIGHT);
            crossStageMap.UpdateSpace(i + 14, 14, SingleSpaceType.HORIZONTAL_STRAIGHT);
            crossStageMap.UpdateSpace(14, i + 14, SingleSpaceType.VERTICAL_STRAIGHT);
        }

        defaults.Add(squareStage);
        defaults.Add(crossStage);
        return defaults;
    }
}
