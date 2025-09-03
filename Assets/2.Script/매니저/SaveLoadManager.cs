using UnityEngine;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    [HideInInspector] public string nextLoadFileName;
    [HideInInspector] public bool isNewGameMode;

    void Awake()
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
    }

    public void SetNextLoadInfo(string fileName, bool isNewGame)
    {
        nextLoadFileName = fileName;
        isNewGameMode = isNewGame;
    }

    public void SaveGame(GameData data, string saveFileName)
    {
        string json = JsonUtility.ToJson(data);
        string path = GetFilePath(saveFileName);

        try
        {
            File.WriteAllText(path, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"파일 저장 중 오류 발생: {e.Message}");
        }
    }

    public string LoadJsonData(string saveFileName)
    {
        string path = GetFilePath(saveFileName);

        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                return json;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"파일 불러오기 중 오류 발생: {e.Message}");
                return string.Empty;
            }
        }
        return string.Empty;
    }

    public string LoadGameDataJson(string saveFileName)
    {
        return LoadJsonData(saveFileName);
    }

    public bool HasSaveFile(string saveFileName)
    {
        return File.Exists(GetFilePath(saveFileName));
    }

    public void DeleteSaveFile(string saveFileName)
    {
        string path = GetFilePath(saveFileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private string GetFilePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }
}