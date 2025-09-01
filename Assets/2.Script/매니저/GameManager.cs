using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Data")]
    public GameData defaultGameData;

    private GameData runtimeGameData;

    public GameData CurrentGameData => runtimeGameData;

    [Header("Player UI")]
    [SerializeField] private TextMeshProUGUI reputationText;

    [Header("Time and Date")]
    public float dayLengthInSeconds = 120f;

    [Header("Dependencies")]
    public PastureUpgradeData pastureUpgradeData;
    [SerializeField] private Camera mainCamera;
    public Color[] pastureColors;

    [SerializeField] private TimeManager timeManager;
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private MoneyManager moneyManager;

    [Header("Prefabs to Load")]
    public GameObject cowPrefab;
    public List<GameObject> buildingPrefabs;

    public int CurrentPastureLevel => runtimeGameData.pastureLevel;
    public string CurrentDate => $"{runtimeGameData.year}년 {runtimeGameData.month}월 {runtimeGameData.day}일";

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
            return;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeReferences();
        InitializeGameData();

        if (PastureManager.Instance != null)
        {
            PastureManager.Instance.Initialize();
        }

        if (ShopUI.Instance != null)
        {
            ShopUI.Instance.Initialize();
        }

        if (UpgradeHandler.Instance != null)
        {
            UpgradeHandler.Instance.Initialize();
        }

        if (EquipmentHandler.Instance != null)
        {
            EquipmentHandler.Instance.Initialize();
        }

        // ★★★ TraderManager와 TraderUI 초기화 순서 보장 ★★★
        if (TraderManager.Instance != null)
        {
            TraderManager.Instance.Initialize();
        }
        if (TraderUI.Instance != null)
        {
            TraderUI.Instance.Initialize();
        }
        // ------------------------------------------

        if (timeManager != null)
        {
            timeManager.Initialize(dayLengthInSeconds, runtimeGameData.year, runtimeGameData.month, runtimeGameData.day, runtimeGameData.reputation);
        }

        UpdateUI();
    }

    private void InitializeGameData()
    {
        string saveFileName = "default_save.json";

        if (defaultGameData == null)
        {
            Debug.LogError("Error: 'Default Game Data' 에셋이 GameManager에 연결되지 않았습니다. 게임 초기값이 모두 0으로 설정됩니다.");
            runtimeGameData = ScriptableObject.CreateInstance<GameData>();
            return;
        }

        if (SaveLoadManager.Instance != null && SaveLoadManager.Instance.HasSaveFile(saveFileName))
        {
            string loadedJson = SaveLoadManager.Instance.LoadJsonData(saveFileName);
            if (!string.IsNullOrEmpty(loadedJson))
            {
                runtimeGameData = Instantiate(defaultGameData);
                JsonUtility.FromJsonOverwrite(loadedJson, runtimeGameData);
                Debug.Log($"불러오기 성공! {saveFileName} 파일의 데이터로 게임이 시작됩니다.");
            }
            else
            {
                Debug.LogWarning("저장 파일이 손상되어 초기값으로 시작합니다.");
                runtimeGameData = Instantiate(defaultGameData);
            }
        }
        else
        {
            Debug.Log("저장 파일이 없어 새 게임 시작.");
            runtimeGameData = Instantiate(defaultGameData);
        }
    }

    private void InitializeReferences()
    {
        if (playerUI == null) playerUI = PlayerUI.Instance;
        if (timeManager == null) timeManager = TimeManager.Instance;
        if (moneyManager == null) moneyManager = MoneyManager.Instance;

        GameObject reputationObject = GameObject.FindWithTag("ReputationText");
        if (reputationObject != null)
        {
            reputationText = reputationObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            reputationText = null;
        }
        if (mainCamera == null) mainCamera = Camera.main;
    }

    public void SaveGame()
    {
        if (SaveLoadManager.Instance != null && runtimeGameData != null)
        {
            SaveLoadManager.Instance.SaveGame(runtimeGameData, "default_save.json");
        }
    }

    public void UpdateUI()
    {
        if (reputationText != null)
        {
            reputationText.text = $"명성도: {runtimeGameData.reputation}";
        }

        if (playerUI != null)
        {
            playerUI.UpdateDayText(runtimeGameData.day);
        }
    }

    public void ChangeReputation(int amount)
    {
        runtimeGameData.reputation += amount;
        UpdateUI();
    }

    public void GoToTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}