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
    public string CurrentDate => $"{gameData.year}�� {gameData.month}�� {gameData.day}��";

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
                Debug.Log("������ ���������� �ҷ��Խ��ϴ�!");
            }
            else
            {
                InitializeGame();
                Debug.Log("���ο� ������ �����մϴ�!");
            }
        }
        else
        {
            Debug.LogWarning("SaveLoadManager �ν��Ͻ��� ã�� �� �����ϴ�. ���ο� ������ �����մϴ�.");
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
            moneyText.text = gameData.money.ToString("N0") + "��";
        }
        if (reputationText != null)
        {
            reputationText.text = $"����: {gameData.reputation}";
        }
        // timeText�� TimeManager�� ������Ʈ�ϵ��� ���� �д�
    }

    public void ChangeReputation(int amount)
    {
        gameData.reputation += amount;
        UpdateUI();
        Debug.Log($"���� ����: {amount}. ���� ����: {gameData.reputation}");
    }

    // �ڡڡ� �� �̵��� ����ϵ��� ����
    public void GoToTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}