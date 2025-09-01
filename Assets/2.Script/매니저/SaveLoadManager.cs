using UnityEngine;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }
    private string savePathRoot;

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

        savePathRoot = Application.persistentDataPath;
        Debug.Log("Game data save path root: " + savePathRoot);
    }

    public void SaveGame(GameData dataToSave, string fileName)
    {
        if (dataToSave == null)
        {
            Debug.LogError("GameData 객체가 없어 저장에 실패했습니다.");
            return;
        }

        string fullPath = Path.Combine(savePathRoot, fileName);
        string json = JsonUtility.ToJson(dataToSave, true);

        try
        {
            File.WriteAllText(fullPath, json);
            Debug.Log("게임이 성공적으로 저장되었습니다! 경로: " + fullPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"게임 저장 실패: {e.Message}");
        }
    }

    // ★★★ 이 메서드가 누락되었었습니다. ★★★
    public string LoadJsonData(string fileName)
    {
        string fullPath = Path.Combine(savePathRoot, fileName);
        if (File.Exists(fullPath))
        {
            try
            {
                string json = File.ReadAllText(fullPath);
                Debug.Log("게임 데이터 JSON 불러오기 성공!");
                return json;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"게임 데이터 JSON 불러오기 실패: {e.Message}");
                return null;
            }
        }
        else
        {
            Debug.LogWarning("저장 파일이 존재하지 않습니다! 경로: " + fullPath);
            return null;
        }
    }

    public bool HasSaveFile(string fileName)
    {
        string fullPath = Path.Combine(savePathRoot, fileName);
        return File.Exists(fullPath);
    }

    public void DeleteSaveFile(string fileName)
    {
        string filePath = Path.Combine(savePathRoot, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"세이브 파일 삭제 성공: {filePath}");
        }
        else
        {
            Debug.LogWarning("삭제할 세이브 파일이 존재하지 않습니다.");
        }
    }
}