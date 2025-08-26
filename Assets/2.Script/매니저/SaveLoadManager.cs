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

        savePath = Application.persistentDataPath + "/savefile.json";
        Debug.Log("Game data save path: " + savePath);
    }

    /// <summary>
    /// Saves the current game data to a file.
    /// </summary>
    public void SaveGame()
    {
        // 1. Create a SaveData object and populate it with current game data.
        SaveData data = new SaveData();

        // Get data from GameManager
        data.currentMoney = GameManager.Instance.CurrentMoney;
        data.reputation = GameManager.Instance.playerReputation;
        data.gameDay = GameManager.Instance.gameDate.Day;
        data.gameMonth = GameManager.Instance.gameDate.Month;
        data.gameYear = GameManager.Instance.gameDate.Year;

        // Get milk data from PlayerInventory
        data.milkInventory = new List<MilkData>();
        foreach (var milk in PlayerInventory.Instance.milkList)
        {
            data.milkInventory.Add(new MilkData { freshness = milk.freshness });
        }

        // Get egg data from Warehouse
        data.eggCount = Warehouse.Instance.GetEggCount();

        // 2. Convert the SaveData object to a JSON string.
        string json = JsonUtility.ToJson(data, true);

        // 3. Write the JSON string to a file.
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved successfully!");
        NotificationManager.Instance.ShowNotification("Game saved!");
    }

    /// <summary>
    /// Loads the game data from the save file.
    /// </summary>
    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            // 1. Read the JSON string from the file.
            string json = File.ReadAllText(savePath);

            // 2. Convert the JSON string to a SaveData object.
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // 3. Restore game state from the loaded data.
            GameManager.Instance.LoadGameData(data.currentMoney, data.reputation, new System.DateTime(data.gameYear, data.gameMonth, data.gameDay));

            // Restore milk data
            PlayerInventory.Instance.milkList.Clear();
            foreach (var milkData in data.milkInventory)
            {
                PlayerInventory.Instance.AddMilk(new Milk(milkData.freshness));
            }

            // Restore egg data
            Warehouse.Instance.SetEggCount(data.eggCount);

            // ★★★ 변경된 부분: 여기에서 NotificationManager를 직접 호출하지 않습니다. ★★★
            Debug.Log("Game loaded successfully!");
        }
        else
        {
            // 저장 파일이 없을 때는 현재 씬의 NotificationManager를 호출해도 안전합니다.
            Debug.LogWarning("No save file found!");
            NotificationManager.Instance.ShowNotification("No saved game found.");
        }
    }

    /// <summary>
    /// Checks if a save file exists.
    /// </summary>
    /// <returns>Returns true if a save file exists, otherwise false.</returns>
    // ★★★ 추가된 부분: 저장 파일 존재 여부를 확인하는 함수 ★★★
    public bool HasSaveFile()
    {
        return File.Exists(savePath);
    }
}