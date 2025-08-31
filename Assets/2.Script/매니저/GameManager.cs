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
    public GameData gameData;

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

    public int CurrentPastureLevel => gameData.pastureLevel;
    public string CurrentDate => $"{gameData.year}년 {gameData.month}월 {gameData.day}일";

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

        if (SaveLoadManager.Instance != null && SaveLoadManager.Instance.HasSaveFile())
        {
            GameData loadedData = SaveLoadManager.Instance.LoadGame();
            if (loadedData != null)
            {
                gameData = loadedData;
                Debug.Log("불러오기 성공! GameData 객체가 교체되었습니다.");
            }
            else
            {
                InitializeGame();
                Debug.Log("저장 파일이 손상되어 새 게임 시작.");
            }
        }
        else
        {
            InitializeGame();
            Debug.Log("저장 파일이 없어 새 게임 시작.");
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    void Start()
    {
        InitializeReferences();

        if (timeManager != null)
        {
            timeManager.Initialize(dayLengthInSeconds, gameData.year, gameData.month, gameData.day, gameData.reputation);
        }

        // MoneyManager는 이제 InitializeMoney를 호출할 필요가 없습니다.
        // if (moneyManager != null)
        // {
        //     moneyManager.InitializeMoney(gameData.money);
        // }

        UpdateUI();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeReferences();
        UpdateUI();

        if (PastureManager.Instance != null)
        {
            PastureManager.Instance.UpdateVisuals();
        }

        // PlayerUI는 OnEnable에서 자동으로 매니저를 찾고 UI를 업데이트하므로
        // GameManager에서는 UpdateUI만 호출하면 충분합니다.
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

    private void InitializeGame()
    {
        gameData = new GameData();
    }

    public void SaveGame()
    {
        if (SaveLoadManager.Instance != null && gameData != null)
        {
            GatherAnimalDataForSave();
            GatherBuildingDataForSave();
            SaveLoadManager.Instance.SaveGame(gameData);
        }
    }

    private void GatherAnimalDataForSave()
    {
        gameData.savedAnimals.Clear();
        if (AnimalManager.Instance != null)
        {
            foreach (var animal in AnimalManager.Instance.activeAnimals)
            {
                if (animal != null)
                {
                    SavedAnimalData data = new SavedAnimalData();
                    data.posX = animal.transform.position.x;
                    data.posY = animal.transform.position.y;
                    gameData.savedAnimals.Add(data);
                }
            }
        }
    }

    private void GatherBuildingDataForSave()
    {
        gameData.savedBuildings.Clear();
        if (BuildingManager.Instance != null)
        {
            foreach (var building in BuildingManager.Instance.activeBuildings)
            {
                if (building != null)
                {
                    SavedBuildingData data = new SavedBuildingData();
                    data.buildingId = building.name;
                    data.posX = building.transform.position.x;
                    data.posY = building.transform.position.y;
                    gameData.savedBuildings.Add(data);
                }
            }
        }
    }

    private void LoadAnimals()
    {
        if (AnimalManager.Instance != null)
        {
            foreach (var animal in AnimalManager.Instance.activeAnimals.ToArray())
            {
                Destroy(animal.gameObject);
            }
            AnimalManager.Instance.activeAnimals.Clear();
        }

        if (gameData.savedAnimals.Count > 0 && cowPrefab != null)
        {
            foreach (var savedData in gameData.savedAnimals)
            {
                Vector3 position = new Vector3(savedData.posX, savedData.posY, 0);
                GameObject newCow = Instantiate(cowPrefab, position, Quaternion.identity);
                Cow cowComponent = newCow.GetComponent<Cow>();
                if (cowComponent != null && AnimalManager.Instance != null)
                {
                    AnimalManager.Instance.AddAnimal(cowComponent);
                }
            }
        }
    }

    private void LoadBuildings()
    {
        if (BuildingManager.Instance != null)
        {
            foreach (var building in BuildingManager.Instance.activeBuildings.ToArray())
            {
                Destroy(building.gameObject);
            }
            BuildingManager.Instance.activeBuildings.Clear();
        }

        if (gameData.savedBuildings.Count > 0 && buildingPrefabs.Count > 0)
        {
            foreach (var savedData in gameData.savedBuildings)
            {
                GameObject buildingPrefab = buildingPrefabs.Find(p => p.name == savedData.buildingId);
                if (buildingPrefab != null)
                {
                    Vector3 position = new Vector3(savedData.posX, savedData.posY, 0);
                    GameObject newBuilding = Instantiate(buildingPrefab, position, Quaternion.identity);
                    if (BuildingManager.Instance != null)
                    {
                        BuildingManager.Instance.AddBuilding(newBuilding);
                    }
                }
            }
        }
    }

    public void UpdateUI()
    {
        if (reputationText != null)
        {
            reputationText.text = $"명성도: {gameData.reputation}";
        }

        if (playerUI != null)
        {
            playerUI.UpdateDayText(gameData.day);
        }
    }

    public void ChangeReputation(int amount)
    {
        gameData.reputation += amount;
        UpdateUI();
    }

    public void GoToTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}