using UnityEngine;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    [HideInInspector] public string nextLoadFileName;

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

    public void SetNextLoadFileName(string fileName)
    {
        nextLoadFileName = fileName;
        Debug.Log($"다음에 로드할 파일명으로 '{fileName}'이 설정되었습니다.");
    }

    public void SaveGame(GameData data, string saveFileName)
    {
        string json = JsonUtility.ToJson(data);
        string path = GetFilePath(saveFileName);

        try
        {
            File.WriteAllText(path, json);
            Debug.Log($"게임 저장 성공! '{path}'");
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
                Debug.Log($"게임 불러오기 성공! '{path}'");
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

    // ★★★ GameData 객체 대신 JSON 문자열을 직접 반환하도록 수정 ★★★
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
            Debug.Log($"저장 파일 삭제 성공: '{path}'");
        }
    }

    private string GetFilePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }
}