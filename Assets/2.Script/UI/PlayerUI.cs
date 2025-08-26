using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerUI : MonoBehaviour
{
    // �̱��� ����
    public static PlayerUI Instance { get; private set; }

    [Header("UI ��� ����")]
    public Slider milkerGauge;
    public Slider basketGauge;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI dayText;
    public Slider dayGauge;

    [Header("�Ŵ��� ����")]
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
        // �� �Ŵ����� �ν��Ͻ��� �����ɴϴ�.
        playerInventory = PlayerInventory.Instance;
        moneyManager = MoneyManager.Instance;
        timeManager = TimeManager.Instance;
        gameManager = GameManager.Instance;

        // �̺�Ʈ ���� ������ �� �Ŵ����� �°� �����մϴ�.
        if (moneyManager != null)
        {
            moneyManager.OnMoneyChanged += UpdateMoney;
        }

        if (timeManager != null)
        {
            // TimeManager�� OnTimeChanged �̺�Ʈ�� ���� timeProgress�� �����մϴ�.
            // PlayerUI�� UpdateDayGauge �Լ��� �Ű������� ��ġ�Ͽ� �ٷ� ������ �� �ֽ��ϴ�.
            timeManager.OnTimeChanged += UpdateDayGauge;
        }

        if (playerInventory != null)
        {
            playerInventory.OnCapacityChanged += UpdateMaxCapacities;
        }

        // �ʱ� ������ �ִ밪 ����
        if (dayGauge != null)
        {
            dayGauge.maxValue = 1f;
        }

        // ���� ���� �� �ʱⰪ ����
        if (moneyManager != null)
        {
            // UI �ʱ�ȭ�� �� �Ŵ������� �ϴ� ���� �� ������, �ϴ� ���⼭ ȣ���մϴ�.
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