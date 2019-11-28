/// File Name: SaveData.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: Used to access save files on the computer. All interaction with the files is done
/// in this script.
/// 
/// Date Last Updated: November 26, 2019

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveData
{
    private static string saveFilePath = Application.persistentDataPath + "/saveFileData.bin";
    private static string newGameFilePath = Application.persistentDataPath + "/newGameData.bin";
    private static string customStagesDirectory = Application.persistentDataPath + "/CustomStages";

    /// <summary>
    /// Creates a file representing the new game statistics. It is only created if there is no
    /// existing file already.
    /// </summary>
    public static void CreateNewGameFile()
    {
        if (File.Exists(newGameFilePath))
        {
            File.Delete(newGameFilePath);
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(newGameFilePath, FileMode.Create);
            GameData data = new GameData();
            formatter.Serialize(stream, data);
            stream.Close();
        }
        else
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(newGameFilePath, FileMode.Create);
            GameData data = new GameData();
            formatter.Serialize(stream, data);
            stream.Close();
        }
    }

    /// <summary>
    /// Saves the data into the save file.
    /// </summary>
    /// <param name="info">Object holding the stats that need to be saved.</param>
    public static void Save(PersistentControl info)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(saveFilePath, FileMode.Create);
        GameData data = new GameData(info);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    /// <summary>
    /// Loads data from the save file, placing it in a GameData object.
    /// </summary>
    /// <returns>A GameData object with stats from the save file.</returns>
    public static GameData Load()
    {
        if (File.Exists(saveFilePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(saveFilePath, FileMode.Open);
            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found, loading new game file.");
            CreateNewGameFile();
            NewGame();
            return null;
        }
    }

    /// <summary>
    /// Iterates through the CustomStages folder and makes stages using the data from the files.
    /// </summary>
    /// <returns>A List of all the stages stored on the computer.</returns>
    public static List<Stage> GetCustomStages()
    {
        List<Stage> stages = new List<Stage>();
        FileStream stream;
        BinaryFormatter formatter = new BinaryFormatter();
        DirectoryInfo dir;
        FileInfo[] info;
        try
        {
            dir = new DirectoryInfo(customStagesDirectory);
            info = dir.GetFiles("*.stage*");
        }
        catch (DirectoryNotFoundException)
        {
            Directory.CreateDirectory(customStagesDirectory);
            dir = new DirectoryInfo(customStagesDirectory);
            info = dir.GetFiles("*.stage*");
        }

        foreach (FileInfo f in info)
        {
            stream = new FileStream(f.FullName, FileMode.Open);
            Stage stage = formatter.Deserialize(stream) as Stage;
            stream.Close();
            stages.Add(stage);
        }
        return stages;
    }

    /// <summary>
    /// Saves an individual stage in a '.stage' file. If the file already exists, this will replace/update the file.
    /// </summary>
    /// <param name="stage">Stage to be saved.</param>
    public static void SaveCustomStage(Stage stage)
    {
        FileStream stream;
        BinaryFormatter formatter;
        try
        {
            stream = new FileStream(Path.Combine(Application.persistentDataPath, "CustomStages", stage.Name + ".stage"), FileMode.Create);
        }
        catch (DirectoryNotFoundException)
        {
            Directory.CreateDirectory(customStagesDirectory);
            stream = new FileStream(Path.Combine(Application.persistentDataPath, "CustomStages", stage.Name + ".stage"), FileMode.Create);
        }

        formatter = new BinaryFormatter();
        formatter.Serialize(stream, stage);
        stream.Close();
    }

    /// <summary>
    /// Pulls data from the new game file and places it in a GameData object.
    /// </summary>
    /// <returns>GameData object containing new game data.</returns>
    public static GameData NewGame()
    {
        if (File.Exists(newGameFilePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(newGameFilePath, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("New Game file not found");
            return null;
        }
    }
}