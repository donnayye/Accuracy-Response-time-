using System.Collections;
using System.Collections.Generic;       // Allow dynamic lists 
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


[System.Serializable]
public class Player
{
    public string _name;

    public Player()
    {
        // Accessible if only other classes are Serializable
        this._name = "";

    }
}

// Creating a Serialize Class
[System.Serializable]       // tells this script can be serialized 
public class _GameData
{
    public static _GameData current;
    public Player Student;

    public _GameData()
    {
        Student = new Player();
    }
}


public static class SaveLoad_
{
    public static List<_GameData> savedGame = new List<_GameData>();


    public static void Save()
    {
        savedGame.Add(_GameData.current);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd");
        bf.Serialize(file, SaveLoad_.savedGame);
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + ".savedGames.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + ".savedGames.gd", FileMode.Open);
            SaveLoad_.savedGame = (List<_GameData>)bf.Deserialize(file);
            file.Close();
        }
    }
}
