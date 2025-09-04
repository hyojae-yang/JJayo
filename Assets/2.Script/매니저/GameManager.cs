using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;

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
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            InitializeReferences();

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
        else
        {
            Debug.Log("메인 씬이 아니므로 초기화를 건너뜁니다.");
        }
    }

    public void ResetGameData()
    {
        if (runtimeGameData != null)
        {
            Destroy(runtimeGameData);
            runtimeGameData = null;
        }
    }

    public void InitializeGameData()
    {
        currentSaveFileName = SaveLoadManager.Instance.nextLoadFileName;
        bool isNewGame = SaveLoadManager.Instance.isNewGameMode;

        SaveLoadManager.Instance.SetNextLoadInfo(null, false);

        if (isNewGame)
        {
            runtimeGameData = Instantiate(defaultGameData);
        }
        else
        {
            if (!string.IsNullOrEmpty(currentSaveFileName) && SaveLoadManager.Instance.HasSaveFile(currentSaveFileName))
            {
                string loadedJson = SaveLoadManager.Instance.LoadJsonData(currentSaveFileName);

                if (!string.IsNullOrEmpty(loadedJson))
                {
                    if (runtimeGameData == null)
                    {
                        runtimeGameData = Instantiate(defaultGameData);
                    }
                    JsonUtility.FromJsonOverwrite(loadedJson, runtimeGameData);
                }
                else
                {
                    Debug.LogWarning("저장 파일이 손상되어 초기값으로 시작합니다.");
                    runtimeGameData = Instantiate(defaultGameData);
                }
            }
            else
            {
                Debug.LogError("이어하기: 저장 파일이 존재하지 않아 게임을 시작할 수 없습니다. 초기값으로 시작합니다.");
                runtimeGameData = Instantiate(defaultGameData);
                currentSaveFileName = null;
            }
        }

        if (ChickenCoop.Instance != null)
        {
            // 위 코드는 이제 필요 없습니다. ChickenCoop 스크립트 자체가 GameData를 직접 참조하도록 변경되었기 때문입니다.
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

            if (ChickenCoop.Instance != null)
            {
                // 위 코드는 이제 필요 없습니다. 다른 스크립트에서 GameData를 직접 수정하기 때문입니다.
            }

            SaveLoadManager.Instance.SaveGame(runtimeGameData, currentSaveFileName);

            Debug.Log($"게임을 {currentSaveFileName} 파일에 저장했습니다.");
        }
    }

    public void UpdateUI()
    {
        if (InfoPanelManager.Instance != null)
        {
            InfoPanelManager.Instance.UpdateReputationUI();
            // ★★★ 추가된 코드: 총알 UI도 함께 업데이트합니다. ★★★
            InfoPanelManager.Instance.UpdateBulletCountUI();
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