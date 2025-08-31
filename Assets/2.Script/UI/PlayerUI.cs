using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance { get; private set; }

    [Header("UI 요소 연결")]
    public Slider milkerGauge;
    public Slider basketGauge;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI dayText;
    public Slider dayGauge;

    [Header("매니저 연결")]
    private PlayerInventory playerInventory;
    private MoneyManager moneyManager;
    private TimeManager timeManager;
    private GameManager gameManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 매니저를 찾고 이벤트에 구독하는 로직을 Start()로 옮겼습니다.
    private void OnEnable()
    {
        // 오브젝트가 비활성화된 후 다시 활성화될 때 구독을 해제하지 않도록 수정했습니다.
    }

    // 오브젝트가 비활성화될 때 이벤트 구독을 해제합니다.
    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    // Start()에서 매니저들을 찾아 구독을 시작하고 UI를 초기화합니다.
    private void Start()
    {
        gameManager = GameManager.Instance;
        timeManager = TimeManager.Instance;
        moneyManager = MoneyManager.Instance;
        playerInventory = PlayerInventory.Instance;

        // 모든 매니저가 정상적으로 연결되었는지 확인
        if (gameManager != null && timeManager != null && moneyManager != null && playerInventory != null)
        {
            SubscribeToEvents();
            InitializeUI();
        }
        else
        {
            Debug.LogError("매니저 인스턴스를 찾지 못했습니다. PlayerUI 초기화 실패.");
        }
    }

    private void SubscribeToEvents()
    {
        if (moneyManager != null)
        {
            moneyManager.OnMoneyChanged += UpdateMoney;
        }
        if (timeManager != null)
        {
            timeManager.OnTimeChanged += UpdateDayGauge;
            timeManager.OnDayChanged += UpdateDayText;
            timeManager.OnMonthChanged += UpdateMonthText;
            timeManager.OnYearChanged += UpdateYearText;
        }
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged += UpdateAllGauges;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (moneyManager != null)
        {
            moneyManager.OnMoneyChanged -= UpdateMoney;
        }
        if (timeManager != null)
        {
            timeManager.OnTimeChanged -= UpdateDayGauge;
            timeManager.OnDayChanged -= UpdateDayText;
            timeManager.OnMonthChanged -= UpdateMonthText;
            timeManager.OnYearChanged -= UpdateYearText;
        }
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= UpdateAllGauges;
        }
    }

    private void InitializeUI()
    {
        if (dayGauge != null)
        {
            dayGauge.maxValue = 1f;
        }

        if (dayGauge != null && timeManager != null)
        {
            dayGauge.value = timeManager.timeProgress;
        }

        UpdateAllGauges();
        if (gameManager != null && gameManager.gameData != null)
        {
            UpdateMoney(gameManager.gameData.money);
        }
        UpdateDayText(gameManager.gameData.day);
    }

    public void UpdateAllGauges()
    {
        UpdateMaxCapacities();
        UpdateGauges();
    }

    private void UpdateDayGauge(float timeProgress)
    {
        if (dayGauge != null)
        {
            dayGauge.value = timeProgress;
        }
    }

    private void UpdateMaxCapacities()
    {
        if (playerInventory != null)
        {
            milkerGauge.maxValue = playerInventory.MilkerCapacity;
            basketGauge.maxValue = playerInventory.BasketCapacity;
        }
    }

    private void UpdateGauges()
    {
        if (playerInventory != null)
        {
            milkerGauge.value = playerInventory.milkList.Count;
            basketGauge.value = playerInventory.currentEggs;
        }
    }

    public void UpdateMoney(int newMoney)
    {
        if (moneyText != null)
        {
            moneyText.text = newMoney.ToString("N0") + "원";
        }
    }

    public void UpdateDayText(int day)
    {
        if (dayText != null && gameManager != null && gameManager.gameData != null)
        {
            dayText.text = $"{gameManager.gameData.year}년 {gameManager.gameData.month}월 {day}일";
        }
    }

    public void UpdateMonthText()
    {
        if (dayText != null && gameManager != null && gameManager.gameData != null)
        {
            dayText.text = $"{gameManager.gameData.year}년 {gameManager.gameData.month}월 {gameManager.gameData.day}일";
        }
    }

    public void UpdateYearText()
    {
        if (dayText != null && gameManager != null && gameManager.gameData != null)
        {
            dayText.text = $"{gameManager.gameData.year}년 {gameManager.gameData.month}월 {gameManager.gameData.day}일";
        }
    }
}