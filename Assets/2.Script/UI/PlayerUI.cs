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

        // 싱글톤 인스턴스 참조는 Awake에서 하는 것이 좋습니다.
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
        // ★★★ PlayerInventory 이벤트 구독 추가 ★★★
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged += UpdateAllGauges;
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
        // ★★★ PlayerInventory 이벤트 구독 해제 추가 ★★★
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= UpdateAllGauges;
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
        // ★★★ 게임 시작 시 게이지 값 초기화 ★★★
        UpdateAllGauges();
    }

    void Update()
    {
        // 이제 Update()에서는 이 로직만 남겨두어도 충분합니다.
        if (dayText != null && gameManager != null && gameManager.gameData != null)
        {
            dayText.text = $"{gameManager.gameData.year}년 {gameManager.gameData.month}월 {gameManager.gameData.day}일";
        }
    }

    // ★★★ 모든 게이지를 한 번에 업데이트하는 새로운 메서드 ★★★
    public void UpdateAllGauges()
    {
        // 최대 용량 업데이트
        UpdateMaxCapacities();
        // 현재 값 업데이트
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
}