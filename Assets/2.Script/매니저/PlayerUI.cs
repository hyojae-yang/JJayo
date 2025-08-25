using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerUI : MonoBehaviour
{
    // �̱��� ������ �ƴ�, �ٸ� ��ũ��Ʈ���� ������ �� �ֵ��� public Instance �Ӽ��� �߰�
    public static PlayerUI Instance { get; private set; }

    [Header("UI ��� ����")]
    public Slider milkerGauge;
    public Slider basketGauge;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI dayText;

    // �ڡڡ� ���� �߰��� �κ�: �ð� ������ ����
    public Slider dayGauge;

    [Header("�Ŵ��� ����")]
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

        // GameManager�� OnMoneyChanged �̺�Ʈ�� UpdateMoney �Լ��� ����
        gameManager.OnMoneyChanged += UpdateMoney;

        // �ڡڡ� ���� �߰��� �κ�: GameManager�� OnTimeChanged �̺�Ʈ�� UpdateDayGauge �Լ��� ����
        gameManager.OnTimeChanged += UpdateDayGauge;

        // PlayerInventory�� OnCapacityChanged �̺�Ʈ�� UpdateMaxCapacities �Լ��� ����
        playerInventory.OnCapacityChanged += UpdateMaxCapacities;

        // �ڡڡ� ���� �߰��� �κ�: �ð� ������ �ִ밪 ����
        if (dayGauge != null)
        {
            dayGauge.maxValue = 1f; // 0.0f���� 1.0f�� ����
        }

        // ���� ���� �� �ʱⰪ ����
        UpdateMoney(gameManager.CurrentMoney);
        UpdateMaxCapacities(); // �ʱ� ������ �ִ밪 ����
    }

    void Update()
    {
        UpdateGauges();
        dayText.text = gameManager.CurrentDate;
    }

    // �ڡڡ� ���� �߰��� �κ�: �ð� ������ ������Ʈ �Լ�
    private void UpdateDayGauge(float timeProgress)
    {
        if (dayGauge != null)
        {
            dayGauge.value = timeProgress;
        }
    }

    /// <summary>
    /// ������ �� �ٱ��� �������� �ִ� �뷮�� ������Ʈ�ϴ� �Լ�
    /// </summary>
    private void UpdateMaxCapacities()
    {
        // �ڡڡ� ������ �κ�: �Ӽ�(Property)�� MilkerCapacity�� BasketCapacity�� ����մϴ�. �ڡڡ�
        milkerGauge.maxValue = playerInventory.MilkerCapacity;
        basketGauge.maxValue = playerInventory.BasketCapacity;
    }

    private void UpdateGauges()
    {
        // ������ ������ ������Ʈ
        milkerGauge.value = playerInventory.currentMilkFreshness.Count;

        // �ٱ��� ������ ������Ʈ
        basketGauge.value = playerInventory.currentEggs;
    }

    /// <summary>
    /// �� UI�� ������Ʈ�ϴ� �Լ�
    /// </summary>
    /// <param name="newMoney">���ο� ���� ��</param>
    private void UpdateMoney(int newMoney)
    {
        moneyText.text = newMoney.ToString("C0");
    }
}