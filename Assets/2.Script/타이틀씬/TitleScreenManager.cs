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
        isNewGameMode = false;
        saveSlotPanel.SetActive(true);
    }

    public void OnNewGameClicked()
    {
        isNewGameMode = true;
        saveSlotPanel.SetActive(true);
    }

    public void HideSaveSlotPanel()
    {
        saveSlotPanel.SetActive(false);
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

            // ★★★ JSONUtility.FromJson을 호출하지 않고, FromJsonOverwrite를 사용하여 안전하게 데이터 로드 ★★★
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

                // UI 업데이트 후 임시로 생성한 인스턴스 파괴
                Destroy(loadedData);
            }
            else
            {
                string displayName = $"슬롯 {i} (새 게임)";
                newSlot.SetupEmptySlot(displayName);
            }

            newSlot.AddListener(() => OnSlotClicked(fileName, isFilled));
        }
    }

    public void OnSlotClicked(string fileName, bool isFilled)
    {
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
        if (isNewGameMode)
        {
            saveLoadManager.DeleteSaveFile(selectedFileName);
            saveLoadManager.SetNextLoadFileName(selectedFileName);
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            saveLoadManager.SetNextLoadFileName(selectedFileName);
            SceneManager.LoadScene("MainScene");
        }
    }
}