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

    private string currentSaveFileName;
    public string CurrentSaveFileName => currentSaveFileName;

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
    public List<GameObject> cowPrefabs;
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

        if (TraderManager.Instance != null)
        {
            TraderManager.Instance.Initialize();
        }
        if (TraderUI.Instance != null)
        {
            TraderUI.Instance.Initialize();
        }

        if (timeManager != null)
        {
            timeManager.Initialize(dayLengthInSeconds, runtimeGameData.year, runtimeGameData.month, runtimeGameData.day, runtimeGameData.reputation);
        }

        if (AnimalManager.Instance != null && runtimeGameData.savedCows != null)
        {
            AnimalManager.Instance.LoadCowData(runtimeGameData.savedCows, cowPrefabs);
        }
        if (BuildingManager.Instance != null && runtimeGameData.savedBuildings != null)
        {
            BuildingManager.Instance.LoadBuildingData(runtimeGameData.savedBuildings, buildingPrefabs);
        }

        UpdateUI();
    }

    private void InitializeGameData()
    {
        currentSaveFileName = SaveLoadManager.Instance.nextLoadFileName;
        SaveLoadManager.Instance.SetNextLoadFileName(null);

        // 불러올 파일명이 있고, 파일이 실제로 존재할 때만 데이터를 로드합니다.
        if (!string.IsNullOrEmpty(currentSaveFileName) && SaveLoadManager.Instance.HasSaveFile(currentSaveFileName))
        {
            string loadedJson = SaveLoadManager.Instance.LoadJsonData(currentSaveFileName);

            if (!string.IsNullOrEmpty(loadedJson))
            {
                // 이어하기 모드: runtimeGameData가 null이 아닐 경우 파괴 후 새로 생성
                if (runtimeGameData != null)
                {
                    Destroy(runtimeGameData);
                }
                runtimeGameData = Instantiate(defaultGameData);
                JsonUtility.FromJsonOverwrite(loadedJson, runtimeGameData);
                Debug.Log($"불러오기 성공! {currentSaveFileName} 파일의 데이터로 게임이 시작됩니다.");
            }
            else
            {
                Debug.LogWarning("저장 파일이 손상되어 초기값으로 시작합니다.");
                if (runtimeGameData != null)
                {
                    Destroy(runtimeGameData);
                }
                runtimeGameData = Instantiate(defaultGameData);
            }
        }
        else
        {
            Debug.Log("불러올 파일이 없거나 새 게임 시작. 초기값으로 게임을 시작합니다.");
            // 새 게임 모드: 기존 데이터가 있으면 파괴하고 새로 생성
            if (runtimeGameData != null)
            {
                Destroy(runtimeGameData);
            }
            runtimeGameData = Instantiate(defaultGameData);
        }
    }

    private void InitializeReferences()
    {
        if (playerUI == null) playerUI = PlayerUI.Instance;
        if (timeManager == null) timeManager = TimeManager.Instance;
        if (moneyManager == null) moneyManager = MoneyManager.Instance;

        if (mainCamera == null) mainCamera = Camera.main;
    }

    public void SaveGame()
    {
        if (SaveLoadManager.Instance != null && runtimeGameData != null)
        {
            if (AnimalManager.Instance != null)
            {
                runtimeGameData.savedCows = AnimalManager.Instance.SaveCowData();
            }
            if (BuildingManager.Instance != null)
            {
                runtimeGameData.savedBuildings = BuildingManager.Instance.SaveBuildingData();
            }

            string saveFileName = currentSaveFileName;
            if (string.IsNullOrEmpty(saveFileName))
            {
                saveFileName = "save_slot_1.json";
            }

            SaveLoadManager.Instance.SaveGame(runtimeGameData, saveFileName);
        }
    }

    public void UpdateUI()
    {
        if (InfoPanelManager.Instance != null)
        {
            InfoPanelManager.Instance.UpdateReputationUI();
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
}