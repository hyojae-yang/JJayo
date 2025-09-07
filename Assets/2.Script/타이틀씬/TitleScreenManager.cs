using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

public class TitleScreenManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject saveSlotPanel;
    public Transform saveSlotParent;
    public GameObject saveSlotPrefab;
    // ★★★ 추가된 부분 ★★★
    public GameObject howToPlayPanel;
    public GameObject settingsPanel;

    [Header("Save Slot Settings")]
    public int numberOfSaveSlots = 3;

    [Header("Confirmation Panel")]
    public ConfirmationPanel confirmationPanel;

    private SaveLoadManager saveLoadManager;
    private bool isNewGameMode;
    private string selectedFileName;

    private void Start()
    {
        saveLoadManager = SaveLoadManager.Instance;
        GenerateSaveSlots();
    }

    public void OnContinueClicked()
    {
        SoundManager.Instance.PlaySFX(SFXType.Button_Click);
        isNewGameMode = false;
        saveSlotPanel.SetActive(true);
    }

    public void OnNewGameClicked()
    {
        SoundManager.Instance.PlaySFX(SFXType.Button_Click);
        isNewGameMode = true;
        saveSlotPanel.SetActive(true);
    }

    public void HideSaveSlotPanel()
    {
        SoundManager.Instance.PlaySFX(SFXType.Button_Click);
        saveSlotPanel.SetActive(false);
    }

    // ★★★ 추가된 메서드 ★★★
    public void ShowHowToPlayPanel()
    {
        SoundManager.Instance.PlaySFX(SFXType.Button_Click);
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }
    }

    // ★★★ 추가된 메서드 ★★★
    public void HideHowToPlayPanel()
    {
        SoundManager.Instance.PlaySFX(SFXType.Button_Click);
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }

    // ★★★ 추가된 메서드 ★★★
    public void ShowSettingsPanel()
    {
        SoundManager.Instance.PlaySFX(SFXType.Button_Click);
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    // ★★★ 추가된 메서드 ★★★
    public void HideSettingsPanel()
    {
        SoundManager.Instance.PlaySFX(SFXType.Button_Click);
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // ★★★ 추가된 메서드 ★★★
    public void QuitGame()
    {
        SoundManager.Instance.PlaySFX(SFXType.Button_Click);
        Debug.Log("게임 종료!");
        Application.Quit();

#if UNITY_EDITOR
        // 에디터에서 실행할 경우 게임 종료 시 동작
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void GenerateSaveSlots()
    {
        foreach (Transform child in saveSlotParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 1; i <= numberOfSaveSlots; i++)
        {
            GameObject newSlotGO = Instantiate(saveSlotPrefab, saveSlotParent);
            SaveSlotItem newSlot = newSlotGO.GetComponent<SaveSlotItem>();

            if (newSlot == null)
            {
                Debug.LogError("SaveSlotItem 컴포넌트를 찾을 수 없습니다. 프리팹에 스크립트가 제대로 추가되었는지 확인해주세요.");
                continue;
            }

            string fileName = $"save_slot_{i}.json";

            string loadedJson = saveLoadManager.LoadJsonData(fileName);
            GameData loadedData = null;
            bool isFilled = !string.IsNullOrEmpty(loadedJson);

            if (isFilled)
            {
                loadedData = ScriptableObject.CreateInstance<GameData>();
                JsonUtility.FromJsonOverwrite(loadedJson, loadedData);

                string date = $"{loadedData.year}년 {loadedData.month}월 {loadedData.day}일";
                string displayName = $"슬롯 {i} (저장됨)";
                newSlot.Setup(displayName, date, loadedData.money, loadedData.reputation);

                Destroy(loadedData);
            }
            else
            {
                string displayName = $"슬롯 {i} (새 게임)";
                newSlot.SetupEmptySlot(displayName);
            }

            // ★★★ 이 부분을 아래와 같이 수정해야 합니다. ★★★
            newSlot.AddListener(() => {
                SoundManager.Instance.PlaySFX(SFXType.Button_Click);
                OnSlotClicked(fileName, isFilled);
            });
        }
    }

    public void OnSlotClicked(string fileName, bool isFilled)
    {
        SoundManager.Instance.PlaySFX(SFXType.Button_Click);
        selectedFileName = fileName;

        if (isNewGameMode)
        {
            string message = isFilled
                ? "기존 데이터를 삭제하고 새 게임을 시작하시겠습니까?"
                : "새 게임을 시작하시겠습니까?";

            confirmationPanel.Show(message, OnConfirmClicked);
        }
        else
        {
            if (isFilled)
            {
                confirmationPanel.Show("게임을 이어서 하시겠습니까?", OnConfirmClicked);
            }
        }
    }

    public void OnConfirmClicked()
    {
        SoundManager.Instance.PlaySFX(SFXType.Button_Click);
        if (string.IsNullOrEmpty(selectedFileName))
        {
            Debug.LogWarning("선택된 파일명이 없습니다. 게임을 시작할 수 없습니다.");
            return;
        }

        if (isNewGameMode)
        {
            saveLoadManager.DeleteSaveFile(selectedFileName);
        }

        saveLoadManager.SetNextLoadInfo(selectedFileName, isNewGameMode);
        GameManager.Instance.InitializeGameData();
        SceneManager.LoadScene("MainScene");
    }
}