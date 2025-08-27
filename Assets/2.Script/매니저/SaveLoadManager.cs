using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        savePath = Path.Combine(Application.persistentDataPath, "savefile.json");
        Debug.Log("Game data save path: " + savePath);
    }

    /// <summary>
    /// Saves the current game data to a file.
    /// </summary>
    public void SaveGame()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance not found. Cannot save game.");
            return;
        }

        GameData dataToSave = GameManager.Instance.gameData;
        string json = JsonUtility.ToJson(dataToSave, true);

        try
        {
            File.WriteAllText(savePath, json);
            Debug.Log("Game saved successfully!");
            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification("Game saved!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }

    /// <summary>
    /// Loads the game data from the save file. Returns the loaded GameData object.
    /// </summary>
    public GameData LoadGame()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                GameData loadedData = JsonUtility.FromJson<GameData>(json);
                return loadedData;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
                if (NotificationManager.Instance != null)
                {
                    NotificationManager.Instance.ShowNotification("Save file corrupted!");
                }
                return null;
            }
        }
        else
        {
            Debug.LogWarning("No save file found!");
            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification("No saved game found.");
            }
            return null;
        }
    }

    /// <summary>
    /// Checks if a save file exists.
    /// </summary>
    /// <returns>Returns true if a save file exists, otherwise false.</returns>
    public bool HasSaveFile()
    {
        return File.Exists(savePath);
    }
}