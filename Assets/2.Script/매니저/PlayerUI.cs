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
        milkerGauge.value = (float)playerInventory.currentMilkFreshness.Count / playerInventory.milkerCapacity;

        // �ٱ��� ������ ������Ʈ
        basketGauge.value = (float)playerInventory.currentEggs / playerInventory.basketCapacity;
    }

    /// <summary>
    /// �� UI�� ������Ʈ�ϴ� �Լ�
    /// </summary>
    /// <param name="newMoney">���ο� ���� ��</param>
    private void UpdateMoney(int newMoney)
    {
        // ��ȣ�� ���� �߰��ϰų�, ��ȭ�ǿ� �°� �ڵ����� ǥ�õǵ��� ����
        moneyText.text = newMoney.ToString("C0");
    }
}