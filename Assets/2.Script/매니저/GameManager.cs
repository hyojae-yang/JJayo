using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    [Header("Game Data")]
    public GameData gameData = new GameData();

    [Header("Player UI")]
    // UI ������Ʈ�� ���� GameManager�� ���� ó���ϰų�,
    // �� �Ŵ����� UI ������Ʈ�� ����ϰ� �� �� �ֽ��ϴ�.
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

    // Start() �Լ����� �������� �ʱ�ȭ�� �ʿ䰡 �����ϴ�.
    // �ٸ� �Ŵ������� ������ Awake/Start���� GameManager�� �����ϸ� �˴ϴ�.

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGame()
    {
        // ������ ó�� ������ ���� ȣ��˴ϴ�.
        gameData = new GameData();
        UpdateReputationUI();
    }

    

    

    public void LoadGameData(GameData loadedData)
    {
        // ����� �����͸� gameData�� ����ϴ�.
        gameData = loadedData;

        // �ٸ� �Ŵ����鿡�� �ε�� �����͸� ������� �ڽ��� ���¸� �����϶�� �˸��ϴ�.
        // �� �Ŵ����� �ڽ��� Start()���� GameManager.Instance.gameData�� �����Ͽ�
        // ������ �����ϰ� �ϴ� ���� �� ���� ����Դϴ�.
        if (PastureManager.Instance != null)
        {
            PastureManager.Instance.UpdateVisuals();
        }

        // UI�� �� ������ ������Ʈ�ϵ��� ����
        UpdateUI();

        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.ShowNotification("������ �ҷ��Խ��ϴ�!");
        }
    }

    // ���� �����Ϳ� ���� �����ϴ� �޼���� �ּ�ȭ�մϴ�.
    // �ٸ� �Ŵ������� gameData�� ���� �����ϰ� ����ϴ�.
    public void ChangeReputation(int amount)
    {
        gameData.reputation += amount;
        UpdateReputationUI();
        Debug.Log($"���� ����: {amount}. ���� ����: {gameData.reputation}");
    }

    // UI ������Ʈ�� �����ϴ� �޼���
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
        if (timeText != null)
        {
            // TimeManager�� ������Ʈ �ϵ��� ����
        }
    }

    private void UpdateReputationUI()
    {
        if (reputationText != null)
        {
            reputationText.text = $"����: {gameData.reputation}";
        }
    }

    // �� �޼���� ���� PastureManager�� ���� ȣ��
    // public void UpgradePasture()
    // {
    //     if (PastureManager.Instance != null)
    //     {
    //         PastureManager.Instance.UpgradePasture();
    //     }
    // }

    public void GoToTitleScene()
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.SaveGame();
            Debug.Log("������ �����ϰ� Ÿ��Ʋ ������ �̵��մϴ�.");
        }
        else
        {
            Debug.LogWarning("SaveLoadManager �ν��Ͻ��� ã�� �� �����ϴ�. ���� ���� �̵��մϴ�.");
        }

        SceneManager.LoadScene("TitleScene");
    }
}
// GameData Ŭ������ �״�� ����
[System.Serializable]
public class GameData
{
    public int money;
    public int reputation;
    public int pastureLevel;
    public int day;
    public int month;
    public int year;
    public int dailyMilkProduced;
    public int dailyEggsProduced;
    public int traderRequiredMilkAmount;
    public int traderRequiredFreshness;
    public int traderOfferedPrice;
    public float traderCurrentEggPrice;
    public int milkCount;
    public float milkAverageFreshness;
    public int eggCount;
    public bool hasGun;
    public int gunLevel;
    public int basketLevel;
    public int milkerLevel;
    public int bulletCount;

    public GameData()
    {
        this.money = 1000;
        this.reputation = 0;
        this.pastureLevel = 0;
        this.day = 1;
        this.month = 1;
        this.year = 1;
        this.dailyMilkProduced = 0;
        this.dailyEggsProduced = 0;
        this.traderRequiredMilkAmount = 0;
        this.traderRequiredFreshness = 0;
        this.traderOfferedPrice = 0;
        this.traderCurrentEggPrice = 0;
        this.milkCount = 0;
        this.milkAverageFreshness = 0f;
        this.eggCount = 0;
        this.hasGun = false;
        this.gunLevel = 0;
        this.basketLevel = 0;
        this.milkerLevel = 0;
        this.bulletCount = 0;
    }
}