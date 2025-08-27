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
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI moneyText;

    [Header("Time and Date")]
    public float dayLengthInSeconds = 120f;

    [Header("Dependencies")]
    public PastureUpgradeData pastureUpgradeData;
    public Camera mainCamera;
    public Color[] pastureColors;

    // Properties
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
                Debug.Log("게임을 성공적으로 불러왔습니다!");
            }
            else
            {
                InitializeGame();
                Debug.Log("새로운 게임을 시작합니다!");
            }
        }
        else
        {
            Debug.LogWarning("SaveLoadManager 인스턴스를 찾을 수 없습니다. 새로운 게임을 시작합니다.");
            InitializeGame();
        }

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

    public void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = gameData.money.ToString("N0") + "원";
        }
        if (reputationText != null)
        {
            reputationText.text = $"명성도: {gameData.reputation}";
        }
        // timeText는 TimeManager가 업데이트하도록 역할 분담
    }

    public void ChangeReputation(int amount)
    {
        gameData.reputation += amount;
        UpdateUI();
        Debug.Log($"명성도 변경: {amount}. 현재 명성도: {gameData.reputation}");
    }

    // ★★★ 씬 이동만 담당하도록 수정
    public void GoToTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}