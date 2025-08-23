using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("UI ��� ����")]
    public Slider milkerGauge;
    public Slider basketGauge;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI dayText;

    [Header("�Ŵ��� ����")]
    private PlayerInventory playerInventory;
    private GameManager gameManager;

    void Start()
    {
        playerInventory = PlayerInventory.Instance;
        gameManager = GameManager.Instance;

        // GameManager�� OnMoneyChanged �̺�Ʈ�� UpdateMoney �Լ��� ����
        gameManager.OnMoneyChanged += UpdateMoney;

        // ������ �������� �ִ밪�� PlayerInventory�� �뷮���� ����
        milkerGauge.maxValue = playerInventory.milkerCapacity;

        // �ٱ��� �������� �ִ밪�� PlayerInventory�� �뷮���� ����
        basketGauge.maxValue = playerInventory.basketCapacity;

        // ���� ���� �� �� �� ������Ʈ
        UpdateMoney(gameManager.CurrentMoney);
    }

    void Update()
    {
        UpdateGauges();
        dayText.text = gameManager.CurrentDate; // ��¥ �ؽ�Ʈ�� �� ������ ������Ʈ
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