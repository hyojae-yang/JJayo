using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    // Path for the save file. Application.persistentDataPath is a secure path for data on each OS.
    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps the object from being destroyed when a scene changes
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

        // Use the centralized GameData object from GameManager.
        GameData dataToSave = GameManager.Instance.gameData;

        // Convert the GameData object to a JSON string.
        string json = JsonUtility.ToJson(dataToSave, true);

        // Write the JSON string to a file.
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
    /// Loads the game data from the save file.
    /// </summary>
    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            try
            {
                // Read the JSON string from the file.
                string json = File.ReadAllText(savePath);

                // Convert the JSON string to a GameData object.
                GameData loadedData = JsonUtility.FromJson<GameData>(json);

                // Pass the loaded data to GameManager to handle restoration.
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.LoadGameData(loadedData);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
                if (NotificationManager.Instance != null)
                {
                    NotificationManager.Instance.ShowNotification("Save file corrupted!");
                }
            }
        }
        else
        {
            // If the file doesn't exist, show a notification.
            Debug.LogWarning("No save file found!");
            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification("No saved game found.");
            }
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