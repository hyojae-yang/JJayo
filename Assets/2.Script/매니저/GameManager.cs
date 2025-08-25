using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    [Header("Game Data")]
    [Tooltip("Current amount of money")]
    [SerializeField]
    private int currentMoney = 1000;

    [Tooltip("플레이어의 명성도")]
    public int playerReputation = 0;

    [Tooltip("오늘 생산된 우유의 양")]
    public int dailyMilkProduced = 0;
    [Tooltip("오늘 생산된 달걀의 양")]
    public int dailyEggsProduced = 0;

    [Header("Player UI")]
    [Tooltip("명성도를 표시할 TextMeshProUGUI 컴포넌트를 연결하세요.")]
    public TextMeshProUGUI reputationText;

    [Header("Time and Date")]
    [Tooltip("Game start date")]
    public DateTime gameDate = new DateTime(1, 1, 1);

    [Tooltip("Game day length in seconds (real time)")]
    public float dayLengthInSeconds = 120f;

    private float timeElapsed = 0f;

    [Header("Pasture Upgrade Data")]
    public PastureUpgradeData pastureUpgradeData;

    [Tooltip("Current pasture level")]
    [SerializeField]
    private int currentPastureLevel = 0;

    public int CurrentPastureLevel => currentPastureLevel;

    [Header("Visual Feedback")]
    [Tooltip("Connect the main camera here")]
    public Camera mainCamera;
    [Tooltip("Set the camera background color for each level")]
    public Color[] pastureColors;

    // Properties
    public int CurrentMoney => currentMoney;
    public string CurrentDate => $"{gameDate.Year}년 {gameDate.Month}월 {gameDate.Day}일";

    // Events
    public event Action<int> OnMoneyChanged;
    public event Action<float> OnTimeChanged;
    public event Action<int> OnDayChanged;
    public event Action OnMonthChanged;
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
        }

        if (mainCamera != null && pastureColors.Length > currentPastureLevel)
        {
            mainCamera.backgroundColor = pastureColors[currentPastureLevel];
        }

        // 명성도 UI 초기화
        UpdateReputationUI();
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        OnTimeChanged?.Invoke(1f - (timeElapsed / dayLengthInSeconds));

        if (timeElapsed >= dayLengthInSeconds)
        {
            int prevMonth = gameDate.Month;
            gameDate = gameDate.AddDays(1);
            timeElapsed -= dayLengthInSeconds;

            // ★★★ 월이 변경되었는지 확인하고 이벤트를 호출 ★★★
            if (gameDate.Month != prevMonth)
            {
                OnMonthChanged?.Invoke();
            }

            OnDayChanged?.Invoke(gameDate.Day);
            NotificationManager.Instance.ShowNotification("새로운 하루가 시작되었습니다!");
            TraderManager.Instance.StartTrade();
        }
    }

    /// <summary>
    /// Adds money by a specified amount.
    /// </summary>
    /// <param name="amount">Amount to add</param>
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
    }

    /// <summary>
    /// Spends money by a specified amount.
    /// </summary>
    /// <param name="amount">Amount to spend</param>
    /// <returns>Returns true if spending was successful, false otherwise</returns>
    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            OnMoneyChanged?.Invoke(currentMoney);
            return true;
        }

        NotificationManager.Instance.ShowNotification($"돈이 부족합니다!");
        return false;
    }

    /// <summary>
    /// 지정된 양만큼 명성도를 변경합니다.
    /// </summary>
    /// <param name="amount">변경할 명성도의 양 (+/-)</param>
    public void ChangeReputation(int amount)
    {
        playerReputation += amount;
        UpdateReputationUI();
        Debug.Log($"명성도 변경: {amount}. 현재 명성도: {playerReputation}");
    }

    /// <summary>
    /// 명성도 UI 텍스트를 현재 값으로 업데이트합니다.
    /// </summary>
    private void UpdateReputationUI()
    {
        if (reputationText != null)
        {
            reputationText.text = $"명성도: {playerReputation}";
        }
    }

    /// <summary>
    /// Upgrades the pasture by 1 level.
    /// </summary>
    public void UpgradePasture()
    {
        if (pastureUpgradeData == null)
        {
            Debug.LogError("PastureUpgradeData is not assigned to the GameManager.");
            return;
        }

        int nextLevel = currentPastureLevel + 1;
        if (nextLevel < pastureUpgradeData.upgradeLevels.Count)
        {
            currentPastureLevel = nextLevel;
            Debug.Log($"Pasture upgraded to level {currentPastureLevel}.");

            if (mainCamera != null && pastureColors.Length > currentPastureLevel)
            {
                mainCamera.backgroundColor = pastureColors[currentPastureLevel];
            }
        }
        else
        {
            Debug.LogWarning("Pasture is already at max level.");
        }
    }
}