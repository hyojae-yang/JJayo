using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerUI : MonoBehaviour
{
    // 싱글톤 패턴이 아닌, 다른 스크립트에서 참조할 수 있도록 public Instance 속성을 추가
    public static PlayerUI Instance { get; private set; }

    [Header("UI 요소 연결")]
    public Slider milkerGauge;
    public Slider basketGauge;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI dayText;

    // ★★★ 새로 추가된 부분: 시간 게이지 연결
    public Slider dayGauge;

    [Header("매니저 연결")]
    private PlayerInventory playerInventory;
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
        playerInventory = PlayerInventory.Instance;
        gameManager = GameManager.Instance;

        // GameManager의 OnMoneyChanged 이벤트에 UpdateMoney 함수를 구독
        gameManager.OnMoneyChanged += UpdateMoney;

        // ★★★ 새로 추가된 부분: GameManager의 OnTimeChanged 이벤트에 UpdateDayGauge 함수를 구독
        gameManager.OnTimeChanged += UpdateDayGauge;

        // PlayerInventory의 OnCapacityChanged 이벤트에 UpdateMaxCapacities 함수를 구독
        playerInventory.OnCapacityChanged += UpdateMaxCapacities;

        // ★★★ 새로 추가된 부분: 시간 게이지 최대값 설정
        if (dayGauge != null)
        {
            dayGauge.maxValue = 1f; // 0.0f에서 1.0f로 진행
        }

        // 게임 시작 시 초기값 설정
        UpdateMoney(gameManager.CurrentMoney);
        UpdateMaxCapacities(); // 초기 게이지 최대값 설정
    }

    void Update()
    {
        UpdateGauges();
        dayText.text = gameManager.CurrentDate;
    }

    // ★★★ 새로 추가된 부분: 시간 게이지 업데이트 함수
    private void UpdateDayGauge(float timeProgress)
    {
        if (dayGauge != null)
        {
            dayGauge.value = timeProgress;
        }
    }

    /// <summary>
    /// 착유기 및 바구니 게이지의 최대 용량을 업데이트하는 함수
    /// </summary>
    private void UpdateMaxCapacities()
    {
        // ★★★ 수정된 부분: 속성(Property)인 MilkerCapacity와 BasketCapacity를 사용합니다. ★★★
        milkerGauge.maxValue = playerInventory.MilkerCapacity;
        basketGauge.maxValue = playerInventory.BasketCapacity;
    }

    private void UpdateGauges()
    {
        // 착유기 게이지 업데이트
        milkerGauge.value = playerInventory.currentMilkFreshness.Count;

        // 바구니 게이지 업데이트
        basketGauge.value = playerInventory.currentEggs;
    }

    /// <summary>
    /// 돈 UI를 업데이트하는 함수
    /// </summary>
    /// <param name="newMoney">새로운 돈의 양</param>
    private void UpdateMoney(int newMoney)
    {
        moneyText.text = newMoney.ToString("C0");
    }
}