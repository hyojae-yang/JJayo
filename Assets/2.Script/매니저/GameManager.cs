using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement; // 씬 관리를 위해 추가

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
    public int currentPastureLevel = 0;

    public int CurrentPastureLevel => currentPastureLevel;

    [Header("Visual Feedback")]
    [Tooltip("Connect the main camera here")]
    public Camera mainCamera;
    [Tooltip("Set the camera background color for each level")]
    public Color[] pastureColors;

    // Properties
    public int CurrentMoney
    {
        get => currentMoney;
        set
        {
            currentMoney = value;
            OnMoneyChanged?.Invoke(currentMoney);
        }
    }
    public string CurrentDate => $"{gameDate.Year}년 {gameDate.Month}월 {gameDate.Day}일";

    // Events
    public event Action<int> OnMoneyChanged;
    public event Action<float> OnTimeChanged;
    public event Action<int> OnDayChanged;
    public event Action OnMonthChanged;

    // ★★★ 추가된 변수: 게임이 로드되었는지 확인하는 플래그 ★★★
    private bool isGameLoaded = false;

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

        UpdateReputationUI();
    }

    // ★★★ 추가된 Start() 함수: 게임이 로드되었을 때 알림을 띄웁니다. ★★★
    void Start()
    {
        // GameManager는 씬 전환 시 파괴되지 않으므로, Start에서 알림을 띄울 수 있습니다.
        if (isGameLoaded)
        {
            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification("게임을 불러왔습니다!");
            }
            isGameLoaded = false; // 알림을 띄운 후 플래그 초기화
        }
    }

    public void LoadGameData(int money, int reputation, DateTime date)
    {
        CurrentMoney = money;
        playerReputation = reputation;
        gameDate = date;
        UpdateReputationUI();
        OnDayChanged?.Invoke(gameDate.Day);

        // ★★★ 게임이 로드되었음을 알리는 플래그 설정 ★★★
        isGameLoaded = true;
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

            if (SaveLoadManager.Instance != null)
            {
                SaveLoadManager.Instance.SaveGame();
            }

            if (gameDate.Month != prevMonth)
            {
                OnMonthChanged?.Invoke();
            }

            OnDayChanged?.Invoke(gameDate.Day);
            NotificationManager.Instance.ShowNotification("새로운 하루가 시작되었습니다!");
            TraderManager.Instance.StartTrade();

            if (WolfManager.Instance != null)
            {
                WolfManager.Instance.ReturnAllWolvesToPool();
            }
        }
    }

    /// <summary>
    /// Adds money by a specified amount.
    /// </summary>
    /// <param name="amount">Amount to add</param>
    public void AddMoney(int amount)
    {
        CurrentMoney += amount;
    }

    /// <summary>
    /// Spends money by a specified amount.
    /// </summary>
    /// <param name="amount">Amount to spend</param>
    /// <returns>Returns true if spending was successful, false otherwise</returns>
    public bool SpendMoney(int amount)
    {
        if (CurrentMoney >= amount)
        {
            CurrentMoney -= amount;
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

    /// <summary>
    /// Saves the game and returns to the title scene.
    /// </summary>
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

        // 타이틀 씬으로 전환
        SceneManager.LoadScene("TitleScene");
    }
}