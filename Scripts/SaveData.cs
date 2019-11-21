using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveData
{
    private static string saveFilePath = Application.persistentDataPath + "/saveFileData.bin";
    private static string newGameFilePath = Application.persistentDataPath + "/newGameData.bin";

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

    public static void Save(PersistentControl info)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(saveFilePath, FileMode.Create);
        GameData data = new GameData(info);
        formatter.Serialize(stream, data);
        stream.Close();
    }

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