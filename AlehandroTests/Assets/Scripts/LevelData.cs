using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int TokenToDestroy;
    public int AmountOfTokensToDestroy;
    public int ObstacleToDestroy;
    public int AmountOfObstaclesToDestroy;
    public int ScoreToGet;
    public int MovesLeft;

    [SerializeField] public int[] Board = new int[45];

    public bool WriteToFile(string LevelName)
    {
        string fullPath = Application.dataPath + "/Data/" + LevelName + ".json";

        AmountOfObstaclesToDestroy = 0;
        if (ObstacleToDestroy > 0)
            AmountOfObstaclesToDestroy = AmountOfObstaclesOfTypeOnBoard(ObstacleToDestroy);

        try
        {
            File.WriteAllText(fullPath, JsonUtility.ToJson(this));
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to {fullPath} with exception {e}");
        }

        return false;
    }

    public bool LoadFromFile(string LevelName)
    {
        string fullPath = Application.dataPath + "/Data/" + LevelName + ".json";

        try
        {
            JsonUtility.FromJsonOverwrite(File.ReadAllText(fullPath), this);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read from {fullPath} with exception {e}");
            return false;
        }
    }

    public bool LoadFromTextAsset(TextAsset textAsset)
    {
        try
        {
            JsonUtility.FromJsonOverwrite(textAsset.text, this);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read from {textAsset.name} file with exception {e}");
            return false;
        }
    }

    private int AmountOfObstaclesOfTypeOnBoard(int obstacleTypeIndex)
    {
        int amount = 0;

        for(int i = 0; i < 45; i++ )
        {
            if (Board[i] == obstacleTypeIndex) amount++;
        }

        return amount;
    }
}