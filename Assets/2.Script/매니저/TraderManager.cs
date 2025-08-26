using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class TraderManager : MonoBehaviour
{
    public static TraderManager Instance { get; private set; }

    [Header("Dependencies")]
    public Trader traderData;        // Trader.cs ��ũ��Ʈ (�䱸 ���� ������)
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
    public int reputationHaggleFailurePenalty = 10;

    [Header("�ް� �Ǹ� UI")]
    public GameObject eggSellPanel; // �ް� �Ǹ� �г�
    public TextMeshProUGUI eggPriceText; // ���� �ް� �ü� �ؽ�Ʈ
    public TextMeshProUGUI availableEggCountText; // ���� ���� �ް� ���� �ؽ�Ʈ
    public TextMeshProUGUI sellCountText; // �Ǹ��� �ް� ���� �ؽ�Ʈ

    [Header("�ް� �ŷ� ����")]
    public float baseEggPrice = 100f;
    [HideInInspector]
    public float currentEggPrice; // ���� ������ �ް� ����

    private int eggsToSell = 0;
    private int eggsSoldToday = 0; // ���� �Ǹ��� �ް� ����
    private int eggRevenueToday = 0; // ���� �ް� �Ǹŷ� ������� �� �ݾ�

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
        eggsSoldToday = 0; // �Ϸ� ���� �� �ʱ�ȭ
        eggRevenueToday = 0; // �Ϸ� ���� �� �ʱ�ȭ
        GenerateTradeDemand();
        GenerateNewEggPrice(); // �ް� ���� ����
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
        int dailyBonus = (GameManager.Instance.gameDate.Day / 10) * 5;

        int requiredAmount = Random.Range(minRequiredMilk + dailyBonus, maxRequiredMilk + dailyBonus);
        int requiredFreshness = Random.Range(minRequiredFreshness + dailyBonus, maxRequiredFreshness + dailyBonus);

        traderData.requiredMilkAmount = requiredAmount;
        traderData.requiredFreshness = requiredFreshness;

        traderData.offeredPrice = requiredAmount * (baseMilkPrice + (requiredFreshness / 10));
    }

    /// <summary>
    /// ���� �ް� �ü��� �����մϴ�.
    /// </summary>
    public void GenerateNewEggPrice()
    {
        float priceMultiplier = UnityEngine.Random.Range(0.5f, 2.5f);
        currentEggPrice = Mathf.Round(baseEggPrice * priceMultiplier);

        if (eggPriceText != null)
        {
            eggPriceText.text = $"������ �ް� �ü�: {currentEggPrice} ���";
        }
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
            requiredFreshnessText.text = traderData.requiredFreshness.ToString();
        }
        if (offeredPriceText != null)
        {
            offeredPriceText.text = traderData.offeredPrice.ToString() + " ���";
        }

        if (playerMoneyText != null)
        {
            playerMoneyText.text = $"{GameManager.Instance.CurrentMoney} ���";
        }
        if (PlayerInventory.Instance != null)
        {
            if (playerMilkCountText != null)
            {
                playerMilkCountText.text = $"{PlayerInventory.Instance.GetMilkCount()} ��";
            }
            if (playerAverageFreshnessText != null)
            {
                playerAverageFreshnessText.text = $"{PlayerInventory.Instance.GetAverageFreshness():F2} ��";
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
                TradeResultUI.Instance.DisplayResult(true, traderData.offeredPrice, reputationPerTrade, "�ŷ��� ���������� �Ϸ��߽��ϴ�!", eggsSoldToday, eggRevenueToday);
            }
        }
        else
        {
            // �ŷ� ����
            GameManager.Instance.ChangeReputation(-1);

            if (TradeResultUI.Instance != null)
            {
                TradeResultUI.Instance.DisplayResult(false, 0, -1, "�䱸 ���� �Ǵ� �ż����� �����Ͽ� �ŷ��� ����Ǿ����ϴ�.", eggsSoldToday, eggRevenueToday);
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
            TradeResultUI.Instance.DisplayResult(false, 0, -reputationDeclinePenalty, "������ �ŷ��� �����Ͽ� ������ ���������ϴ�.", eggsSoldToday, eggRevenueToday);
        }
    }

    /// <summary>
    /// '����' ��ư Ŭ�� �� ȣ��˴ϴ�.
    /// </summary>
    public void OnHaggleButtonClicked()
    {
        if (PlayerInventory.Instance != null && PlayerInventory.Instance.CanSellMilk(traderData.requiredMilkAmount, traderData.requiredFreshness))
        {
            int haggleChance = UnityEngine.Random.Range(0, 100);
            int successThreshold = 50;

            if (haggleChance <= successThreshold)
            {
                int bonusPrice = traderData.offeredPrice;
                int finalPrice = traderData.offeredPrice + bonusPrice;

                GameManager.Instance.AddMoney(finalPrice);

                if (TradeResultUI.Instance != null)
                {
                    TradeResultUI.Instance.DisplayResult(true, finalPrice, 0, $"������ �����Ͽ� {bonusPrice} ��带 �߰��� �޾ҽ��ϴ�!", eggsSoldToday, eggRevenueToday);
                }
            }
            else
            {
                GameManager.Instance.ChangeReputation(-reputationHaggleFailurePenalty);

                if (TradeResultUI.Instance != null)
                {
                    TradeResultUI.Instance.DisplayResult(false, 0, -reputationHaggleFailurePenalty, "������ �����߽��ϴ�. ������ ����� ���� ������ ũ�� ���������ϴ�.", eggsSoldToday, eggRevenueToday);
                }
            }
        }
        else
        {
            if (TradeResultUI.Instance != null)
            {
                TradeResultUI.Instance.DisplayResult(false, 0, 0, "�䱸 ���� �Ǵ� �ż����� �����Ͽ� ������ �õ��� �� �����ϴ�.", eggsSoldToday, eggRevenueToday);
            }
        }
    }

    // --- �ް� �Ǹ� �ý��� �߰� ---

    /// <summary>
    /// '�ް� �Ǹ�' ��ư Ŭ�� �� ȣ��˴ϴ�.
    /// </summary>
    public void OnOpenEggPanelButtonClicked()
    {
        if (eggSellPanel != null)
        {
            eggSellPanel.SetActive(true);
        }

        eggsToSell = 0;
        UpdateEggUI();
    }

    /// <summary>
    /// �ް� �Ǹ� ���� ���� ��ư
    /// </summary>
    public void OnIncreaseEggCount()
    {
        if (eggsToSell < Warehouse.Instance.GetEggCount())
        {
            eggsToSell++;
            UpdateEggUI();
        }
    }

    /// <summary>
    /// �ް� �Ǹ� ���� ���� ��ư
    /// </summary>
    public void OnDecreaseEggCount()
    {
        if (eggsToSell > 0)
        {
            eggsToSell--;
            UpdateEggUI();
        }
    }

    /// <summary>
    /// UI �ؽ�Ʈ�� ������Ʈ�մϴ�.
    /// </summary>
    private void UpdateEggUI()
    {
        if (availableEggCountText != null)
        {
            availableEggCountText.text = $"���� �ް�: {Warehouse.Instance.GetEggCount()} ��";
        }
        if (sellCountText != null)
        {
            sellCountText.text = eggsToSell.ToString();
        }
    }

    /// <summary>
    /// '�Ǹ�' ��ư Ŭ�� �� ȣ��˴ϴ�.
    /// </summary>
    public void OnSellEggsButtonClicked()
    {
        if (eggsToSell > 0)
        {
            float totalRevenue = eggsToSell * currentEggPrice;
            GameManager.Instance.AddMoney(Mathf.RoundToInt(totalRevenue));

            Warehouse.Instance.RemoveEggs(eggsToSell);

            // �Ǹ� ��� ����
            eggsSoldToday += eggsToSell;
            eggRevenueToday += Mathf.RoundToInt(totalRevenue);

            NotificationManager.Instance.ShowNotification($"�ް� {eggsToSell}���� {Mathf.RoundToInt(totalRevenue)} ��忡 �Ǹ��߽��ϴ�!");

            OnCloseEggPanelButtonClicked();
        }
        else
        {
            NotificationManager.Instance.ShowNotification("�Ǹ��� �ް��� �����ϴ�.");
        }
    }

    /// <summary>
    /// '�ݱ�' ��ư Ŭ�� �� ȣ��˴ϴ�.
    /// </summary>
    public void OnCloseEggPanelButtonClicked()
    {
        if (eggSellPanel != null)
        {
            eggSellPanel.SetActive(false);
        }
    }
}