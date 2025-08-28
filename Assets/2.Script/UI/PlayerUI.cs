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

        // �̱��� �ν��Ͻ� ������ Awake���� �ϴ� ���� �����ϴ�.
        playerInventory = PlayerInventory.Instance;
        moneyManager = MoneyManager.Instance;
        timeManager = TimeManager.Instance;
        gameManager = GameManager.Instance;
    }

    private void OnEnable()
    {
        // ������Ʈ�� Ȱ��ȭ�� �� �̺�Ʈ ����
        if (moneyManager != null)
        {
            moneyManager.OnMoneyChanged += UpdateMoney;
        }
        if (timeManager != null)
        {
            timeManager.OnTimeChanged += UpdateDayGauge;
        }
        // �ڡڡ� PlayerInventory �̺�Ʈ ���� �߰� �ڡڡ�
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged += UpdateAllGauges;
        }
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
        }
        // �ڡڡ� PlayerInventory �̺�Ʈ ���� ���� �߰� �ڡڡ�
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
        // �ڡڡ� ���� ���� �� ������ �� �ʱ�ȭ �ڡڡ�
        UpdateAllGauges();
    }

    void Update()
    {
        // ���� Update()������ �� ������ ���ܵξ ����մϴ�.
        if (dayText != null && gameManager != null && gameManager.gameData != null)
        {
            dayText.text = $"{gameManager.gameData.year}�� {gameManager.gameData.month}�� {gameManager.gameData.day}��";
        }
    }

    // �ڡڡ� ��� �������� �� ���� ������Ʈ�ϴ� ���ο� �޼��� �ڡڡ�
    public void UpdateAllGauges()
    {
        // �ִ� �뷮 ������Ʈ
        UpdateMaxCapacities();
        // ���� �� ������Ʈ
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
}