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

        // Awake에서 모든 매니저 인스턴스를 먼저 가져옵니다.
        playerInventory = PlayerInventory.Instance;
        moneyManager = MoneyManager.Instance;
        timeManager = TimeManager.Instance;
        gameManager = GameManager.Instance;
    }

    private void OnEnable()
    {
        // 오브젝트가 활성화될 때 이벤트 구독
        if (moneyManager != null)
        {
            moneyManager.OnMoneyChanged += UpdateMoney;
        }
        if (timeManager != null)
        {
            timeManager.OnTimeChanged += UpdateDayGauge;
        }
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
        }
    }

    void Start()
    {
        if (dayGauge != null)
        {
            dayGauge.maxValue = 1f;
        }

        if (gameManager != null && gameManager.gameData != null)
        {
            UpdateMoney(gameManager.gameData.money);
        }
        UpdateMaxCapacities();
    }

    void Update()
    {
        UpdateGauges();
        if (dayText != null && gameManager != null && gameManager.gameData != null)
        {
            dayText.text = $"{gameManager.gameData.year}년 {gameManager.gameData.month}월 {gameManager.gameData.day}일";
        }
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
}