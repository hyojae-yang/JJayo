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

    [Tooltip("�÷��̾��� ����")]
    public int playerReputation = 0;

    [Tooltip("���� ����� ������ ��")]
    public int dailyMilkProduced = 0;
    [Tooltip("���� ����� �ް��� ��")]
    public int dailyEggsProduced = 0;

    [Header("Player UI")]
    [Tooltip("������ ǥ���� TextMeshProUGUI ������Ʈ�� �����ϼ���.")]
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
    public string CurrentDate => $"{gameDate.Year}�� {gameDate.Month}�� {gameDate.Day}��";

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

        // ���� UI �ʱ�ȭ
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

            // �ڡڡ� ���� ����Ǿ����� Ȯ���ϰ� �̺�Ʈ�� ȣ�� �ڡڡ�
            if (gameDate.Month != prevMonth)
            {
                OnMonthChanged?.Invoke();
            }

            OnDayChanged?.Invoke(gameDate.Day);
            NotificationManager.Instance.ShowNotification("���ο� �Ϸ簡 ���۵Ǿ����ϴ�!");
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

        NotificationManager.Instance.ShowNotification($"���� �����մϴ�!");
        return false;
    }

    /// <summary>
    /// ������ �縸ŭ ������ �����մϴ�.
    /// </summary>
    /// <param name="amount">������ ������ �� (+/-)</param>
    public void ChangeReputation(int amount)
    {
        playerReputation += amount;
        UpdateReputationUI();
        Debug.Log($"���� ����: {amount}. ���� ����: {playerReputation}");
    }

    /// <summary>
    /// ���� UI �ؽ�Ʈ�� ���� ������ ������Ʈ�մϴ�.
    /// </summary>
    private void UpdateReputationUI()
    {
        if (reputationText != null)
        {
            reputationText.text = $"����: {playerReputation}";
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