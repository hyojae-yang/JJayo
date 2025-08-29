using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Data")]
    public GameData gameData;

    [Header("Player UI")]
    public TextMeshProUGUI reputationText;

    [Header("Time and Date")]
    public float dayLengthInSeconds = 120f;

    [Header("Dependencies")]
    public PastureUpgradeData pastureUpgradeData;
    public Camera mainCamera;
    public Color[] pastureColors;

    public TimeManager timeManager;
    public PlayerUI playerUI;
    public MoneyManager moneyManager;

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

        if (SaveLoadManager.Instance != null)
        {
            GameData loadedData = SaveLoadManager.Instance.LoadGame();
            if (loadedData != null)
            {
                gameData = loadedData;
            }
            else
            {
                InitializeGame();
            }
        }
        else
        {
            InitializeGame();
        }

        if (timeManager == null) timeManager = TimeManager.Instance;
        if (moneyManager == null) moneyManager = MoneyManager.Instance;
        if (playerUI == null) playerUI = PlayerUI.Instance;

        if (timeManager != null)
        {
            timeManager.gameManager = this;
        }
        if (moneyManager != null)
        {
            moneyManager.gameManager = this;
        }

        UpdateUI();
    }

    void Start()
    {
        if (playerUI != null)
        {
            playerUI.InitializeManagers(this, timeManager, moneyManager);
        }
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
        UpdateUI();
        if (PastureManager.Instance != null)
        {
            PastureManager.Instance.UpdateVisuals();
        }
    }

    private void InitializeGame()
    {
        gameData = new GameData();
    }

    public void SaveGame()
    {
        if (SaveLoadManager.Instance != null && gameData != null)
        {
            SaveLoadManager.Instance.SaveGame(gameData);
        }
    }

    public void UpdateUI()
    {
        if (reputationText != null)
        {
            reputationText.text = $"명성도: {gameData.reputation}";
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