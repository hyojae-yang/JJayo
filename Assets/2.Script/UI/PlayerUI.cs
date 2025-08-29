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

    private void OnEnable()
    {
        // OnEnable에서는 이벤트 구독을 하지 않습니다.
        // GameManager가 호출하는 InitializeManagers()에서 처리합니다.
    }

    private void OnDisable()
    {
        // 오브젝트가 비활성화될 때 이벤트 구독 해제
        if (moneyManager != null)
        {
            moneyManager.OnMoneyChanged -= UpdateMoney;
        }
        if (timeManager != null)
        {
            timeManager.OnTimeChanged -= UpdateDayGauge;
            timeManager.OnDayChanged -= (day) => UpdateDayText();
            timeManager.OnMonthChanged -= UpdateDayText;
            timeManager.OnYearChanged -= UpdateDayText;
        }
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= UpdateAllGauges;
        }
    }

    public void InitializeManagers(GameManager gm, TimeManager tm, MoneyManager mm)
    {
        // GameManager에서 전달받은 인스턴스를 사용하거나,
        // 만약 null이라면 직접 인스턴스를 찾습니다.
        gameManager = gm ?? GameManager.Instance;
        timeManager = tm ?? TimeManager.Instance;
        moneyManager = mm ?? MoneyManager.Instance;

        if (playerInventory == null)
        {
            playerInventory = PlayerInventory.Instance;
        }

        // 이벤트 구독 시작
        if (moneyManager != null)
        {
            moneyManager.OnMoneyChanged += UpdateMoney;
        }
        if (timeManager != null)
        {
            timeManager.OnTimeChanged += UpdateDayGauge;
            timeManager.OnDayChanged += (day) => UpdateDayText();
            timeManager.OnMonthChanged += UpdateDayText;
            timeManager.OnYearChanged += UpdateDayText;
        }
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged += UpdateAllGauges;
        }

        // 초기값 설정
        if (dayGauge != null)
        {
            dayGauge.maxValue = 1f;
        }

        if (gameManager != null && gameManager.gameData != null)
        {
            UpdateMoney(gameManager.gameData.money);
        }

        UpdateDayText();
        UpdateAllGauges();
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

    private void UpdateMoney(int newMoney)
    {
        if (moneyText != null)
        {
            moneyText.text = newMoney.ToString("N0") + "원";
        }
    }

    private void UpdateDayText()
    {
        if (dayText != null && gameManager != null && gameManager.gameData != null)
        {
            dayText.text = $"{gameManager.gameData.year}년 {gameManager.gameData.month}월 {gameManager.gameData.day}일";
        }
    }
}