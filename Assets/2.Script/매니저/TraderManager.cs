using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TraderManager : MonoBehaviour
{
    public static TraderManager Instance { get; private set; }

    [Header("Dependencies")]
    public Trader traderData;           // Trader.cs ��ũ��Ʈ (�䱸 ���� ������)
    public GameObject traderUIPanel;    // ���� UI �г� (�θ� ������Ʈ)
    public GameObject tradeResultPanel; // �ŷ� ����� ǥ���� ��� UI �г� (�����Ϳ��� �����ؾ� �մϴ�!)

    [Header("UI Text Components")]
    // ���� �䱸 ���� ǥ�ÿ� �ؽ�Ʈ
    public TextMeshProUGUI requiredAmountText; // �䱸 ����
    public TextMeshProUGUI requiredFreshnessText; // �䱸 �ż���
    public TextMeshProUGUI offeredPriceText; // ���� ����

    [Header("Player UI Components")]
    public TextMeshProUGUI playerMoneyText;
    public TextMeshProUGUI playerMilkCountText;
    public TextMeshProUGUI playerAverageFreshnessText;

    [Header("Trade Settings")]
    public int minRequiredMilk = 5;
    public int maxRequiredMilk = 20;
    public int minRequiredFreshness = 50;
    public int maxRequiredFreshness = 90;
    public int baseMilkPrice = 10;

    public int reputationPerTrade = 10;
    public int reputationDeclinePenalty = 5;
    public int reputationHaggleFailurePenalty = 10; // �ڡڡ� 15���� 10���� ���� �ڡڡ�

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (traderUIPanel != null)
        {
            traderUIPanel.SetActive(false);
        }
    }

    /// <summary>
    /// GameManager���� �Ϸ� ���� �� ȣ��Ǵ� �Լ�. ���� ���� �� �ŷ� ����.
    /// </summary>
    public void StartTrade()
    {
        Time.timeScale = 0;
        GenerateTradeDemand();
        UpdateUITexts();
        if (traderUIPanel != null)
        {
            traderUIPanel.SetActive(true);
        }
    }

    /// <summary>
    /// ������ �䱸 ����(����, �ż���, ����)�� �����ϰ� �����մϴ�.
    /// </summary>
    private void GenerateTradeDemand()
    {
        int dailyBonus = (GameManager.Instance.gameDate.Day / 10)*5;

        int requiredAmount = Random.Range(minRequiredMilk + dailyBonus, maxRequiredMilk + dailyBonus);
        int requiredFreshness = Random.Range(minRequiredFreshness + dailyBonus, maxRequiredFreshness + dailyBonus);

        traderData.requiredMilkAmount = requiredAmount;
        traderData.requiredFreshness = requiredFreshness;

        traderData.offeredPrice = requiredAmount * (baseMilkPrice + (requiredFreshness / 10));
    }

    /// <summary>
    /// ������ �䱸 ���� �����͸� UI �ؽ�Ʈ�� �ݿ��մϴ�.
    /// </summary>
    private void UpdateUITexts()
    {
        if (requiredAmountText != null)
        {
            requiredAmountText.text = traderData.requiredMilkAmount.ToString() + " �� �䱸";
        }
        if (requiredFreshnessText != null)
        {
            requiredFreshnessText.text = "�ּ� �ż���: " + traderData.requiredFreshness.ToString() + " ��";
        }
        if (offeredPriceText != null)
        {
            offeredPriceText.text = "���� �ݾ�: " + traderData.offeredPrice.ToString() + " ���";
        }

        if (playerMoneyText != null)
        {
            playerMoneyText.text = $"���� �ڱ�: {GameManager.Instance.CurrentMoney} ���";
        }
        if (PlayerInventory.Instance != null)
        {
            if (playerMilkCountText != null)
            {
                playerMilkCountText.text = $"���� ����: {PlayerInventory.Instance.GetMilkCount()} ��";
            }
            if (playerAverageFreshnessText != null)
            {
                playerAverageFreshnessText.text = $"��� �ż���: {PlayerInventory.Instance.GetAverageFreshness():F2} ��";
            }
        }
    }

    /// <summary>
    /// '����' ��ư Ŭ�� �� ȣ��˴ϴ�.
    /// </summary>
    public void OnAcceptButtonClicked()
    {
        if (PlayerInventory.Instance != null && PlayerInventory.Instance.CanSellMilk(traderData.requiredMilkAmount, traderData.requiredFreshness))
        {
            // 1. �ŷ� ����
            PlayerInventory.Instance.SellMilk(traderData.requiredMilkAmount);
            GameManager.Instance.AddMoney(traderData.offeredPrice);
            GameManager.Instance.ChangeReputation(reputationPerTrade);

            // 2. ��� �˸�â ����
            if (TradeResultUI.Instance != null)
            {
                TradeResultUI.Instance.DisplayResult(true, traderData.offeredPrice, reputationPerTrade, "�ŷ��� ���������� �Ϸ��߽��ϴ�!");
            }
        }
        else
        {
            // �ŷ� ����
            GameManager.Instance.ChangeReputation(-1);

            if (TradeResultUI.Instance != null)
            {
                // CanSellMilk ���ο��� �̹� ���� �˸��� �������, ���â�� ���� �ٽ� �����մϴ�.
                TradeResultUI.Instance.DisplayResult(false, 0, -1, "�䱸 ���� �Ǵ� �ż����� �����Ͽ� �ŷ��� ����Ǿ����ϴ�.");
            }
        }
    }

    /// <summary>
    /// '�ź�' ��ư Ŭ�� �� ȣ��˴ϴ�.
    /// </summary>
    public void OnDeclineButtonClicked()
    {
        GameManager.Instance.ChangeReputation(-reputationDeclinePenalty);

        if (TradeResultUI.Instance != null)
        {
            TradeResultUI.Instance.DisplayResult(false, 0, -reputationDeclinePenalty, "������ �ŷ��� �����Ͽ� ������ ���������ϴ�.");
        }
    }

    /// <summary>
    /// '����' ��ư Ŭ�� �� ȣ��˴ϴ�.
    /// </summary>
    public void OnHaggleButtonClicked()
    {
        // �ڡڡ� �ŷ� ���� ���� ���� �߰� �ڡڡ�
        if (PlayerInventory.Instance != null && PlayerInventory.Instance.CanSellMilk(traderData.requiredMilkAmount, traderData.requiredFreshness))
        {
            int haggleChance = UnityEngine.Random.Range(0, 100);
            int successThreshold = 50;

            if (haggleChance <= successThreshold)
            {
                // �ڡڡ� ���ʽ� �ݾ� 2��� ���� �ڡڡ�
                int bonusPrice = traderData.offeredPrice; // ���� �ݾ��� 100% ���ʽ� = 2��
                int finalPrice = traderData.offeredPrice + bonusPrice;

                GameManager.Instance.AddMoney(finalPrice);

                if (TradeResultUI.Instance != null)
                {
                    TradeResultUI.Instance.DisplayResult(true, finalPrice, 0, $"������ �����Ͽ� {bonusPrice} ��带 �߰��� �޾ҽ��ϴ�!");
                }
            }
            else
            {
                // �ڡڡ� ���� ���Ƽ 10���� ���� �ڡڡ�
                GameManager.Instance.ChangeReputation(-reputationHaggleFailurePenalty);

                if (TradeResultUI.Instance != null)
                {
                    TradeResultUI.Instance.DisplayResult(false, 0, -reputationHaggleFailurePenalty, "������ �����߽��ϴ�. ������ ����� ���� ������ ũ�� ���������ϴ�.");
                }
            }
        }
        else
        {
            // ���� ���� ���� ��
            if (TradeResultUI.Instance != null)
            {
                TradeResultUI.Instance.DisplayResult(false, 0, 0, "�䱸 ���� �Ǵ� �ż����� �����Ͽ� ������ �õ��� �� �����ϴ�.");
            }
        }
    }
}