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
    // UI 업데이트는 이제 GameManager가 직접 처리하거나,
    // 각 매니저가 UI 업데이트를 담당하게 할 수 있습니다.
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

    // Start() 함수에서 의존성을 초기화할 필요가 없습니다.
    // 다른 매니저들은 각자의 Awake/Start에서 GameManager를 참조하면 됩니다.

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
        // 게임을 처음 시작할 때만 호출됩니다.
        gameData = new GameData();
        UpdateReputationUI();
    }

    

    

    public void LoadGameData(GameData loadedData)
    {
        // 저장된 데이터를 gameData에 덮어씁니다.
        gameData = loadedData;

        // 다른 매니저들에게 로드된 데이터를 기반으로 자신의 상태를 복원하라고 알립니다.
        // 각 매니저가 자신의 Start()에서 GameManager.Instance.gameData를 참조하여
        // 스스로 복원하게 하는 것이 더 좋은 방법입니다.
        if (PastureManager.Instance != null)
        {
            PastureManager.Instance.UpdateVisuals();
        }

        // UI를 한 곳에서 업데이트하도록 통합
        UpdateUI();

        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.ShowNotification("게임을 불러왔습니다!");
        }
    }

    // 게임 데이터에 직접 접근하는 메서드는 최소화합니다.
    // 다른 매니저들이 gameData를 직접 수정하게 만듭니다.
    public void ChangeReputation(int amount)
    {
        gameData.reputation += amount;
        UpdateReputationUI();
        Debug.Log($"명성도 변경: {amount}. 현재 명성도: {gameData.reputation}");
    }

    // UI 업데이트를 통합하는 메서드
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
        if (timeText != null)
        {
            // TimeManager가 업데이트 하도록 변경
        }
    }

    private void UpdateReputationUI()
    {
        if (reputationText != null)
        {
            reputationText.text = $"명성도: {gameData.reputation}";
        }
    }

    // 이 메서드는 이제 PastureManager가 직접 호출
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
            Debug.Log("게임을 저장하고 타이틀 씬으로 이동합니다.");
        }
        else
        {
            Debug.LogWarning("SaveLoadManager 인스턴스를 찾을 수 없습니다. 저장 없이 이동합니다.");
        }

        SceneManager.LoadScene("TitleScene");
    }
}
// GameData 클래스는 그대로 유지
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