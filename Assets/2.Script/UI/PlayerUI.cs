using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerUI : MonoBehaviour
{
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

    private void OnEnable()
    {
        // OnEnable������ �̺�Ʈ ������ ���� �ʽ��ϴ�.
        // GameManager�� ȣ���ϴ� InitializeManagers()���� ó���մϴ�.
    }

    private void OnDisable()
    {
        // ������Ʈ�� ��Ȱ��ȭ�� �� �̺�Ʈ ���� ����
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
        // GameManager���� ���޹��� �ν��Ͻ��� ����ϰų�,
        // ���� null�̶�� ���� �ν��Ͻ��� ã���ϴ�.
        gameManager = gm ?? GameManager.Instance;
        timeManager = tm ?? TimeManager.Instance;
        moneyManager = mm ?? MoneyManager.Instance;

        if (playerInventory == null)
        {
            playerInventory = PlayerInventory.Instance;
        }

        // �̺�Ʈ ���� ����
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

        // �ʱⰪ ����
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
            moneyText.text = newMoney.ToString("N0") + "��";
        }
    }

    private void UpdateDayText()
    {
        if (dayText != null && gameManager != null && gameManager.gameData != null)
        {
            dayText.text = $"{gameManager.gameData.year}�� {gameManager.gameData.month}�� {gameManager.gameData.day}��";
        }
    }
}