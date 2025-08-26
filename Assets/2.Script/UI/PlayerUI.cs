using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerUI : MonoBehaviour
{
    // 싱글톤 패턴
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

    void Start()
    {
        // 각 매니저의 인스턴스를 가져옵니다.
        playerInventory = PlayerInventory.Instance;
        moneyManager = MoneyManager.Instance;
        timeManager = TimeManager.Instance;
        gameManager = GameManager.Instance;

        // 이벤트 구독 로직을 각 매니저에 맞게 수정합니다.
        if (moneyManager != null)
        {
            moneyManager.OnMoneyChanged += UpdateMoney;
        }

        if (timeManager != null)
        {
            // TimeManager의 OnTimeChanged 이벤트는 이제 timeProgress를 전달합니다.
            // PlayerUI의 UpdateDayGauge 함수와 매개변수가 일치하여 바로 연결할 수 있습니다.
            timeManager.OnTimeChanged += UpdateDayGauge;
        }

        if (playerInventory != null)
        {
            playerInventory.OnCapacityChanged += UpdateMaxCapacities;
        }

        // 초기 게이지 최대값 설정
        if (dayGauge != null)
        {
            dayGauge.maxValue = 1f;
        }

        // 게임 시작 시 초기값 설정
        if (moneyManager != null)
        {
            // UI 초기화는 각 매니저에서 하는 것이 더 좋지만, 일단 여기서 호출합니다.
            UpdateMoney(gameManager.gameData.money);
        }
        UpdateMaxCapacities();
    }

    void Update()
    {
        UpdateGauges();
        if (dayText != null && gameManager != null)
        {
            dayText.text = gameManager.CurrentDate;
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
            moneyText.text = newMoney.ToString("C0");
        }
    }
}