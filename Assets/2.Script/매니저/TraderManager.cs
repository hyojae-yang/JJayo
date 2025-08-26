using UnityEngine;
using System.Linq;
using UnityEditor;

public class TraderManager : MonoBehaviour
{
    public static TraderManager Instance { get; private set; }

    [Header("Dependencies")]
    private GameData gameData; // GameData ��ü�� ���� ����
    public TraderData traderData = new TraderData();

    [Header("Trade Settings")]
    public int minRequiredMilk = 5;
    public int maxRequiredMilk = 20;
    public int minRequiredFreshness = 20;
    public int maxRequiredFreshness = 25;
    public int baseMilkPrice = 30;

    public int reputationPerTrade = 1;
    public int reputationDeclinePenalty = 5;
    public int reputationHaggleFailurePenalty = 10;

    [Header("�ް� �ŷ� ����")]
    public float baseEggPrice = 10f;

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
    }

    void Start()
    {
        gameData = GameManager.Instance.gameData;
        if (gameData == null) Debug.LogError("GameData�� ã�� �� �����ϴ�.");
    }

    public void StartTrade()
    {
        traderData.eggsSoldToday = 0;
        traderData.eggRevenueToday = 0;
        GenerateTradeDemand();
        GenerateNewEggPrice();
    }

    private void GenerateTradeDemand()
    {
        int dailyBonus = (gameData.day / 10) * 5;
        traderData.requiredMilkAmount = Random.Range(minRequiredMilk + dailyBonus, maxRequiredMilk + dailyBonus);
        traderData.requiredFreshness = Random.Range(minRequiredFreshness + dailyBonus, maxRequiredFreshness + dailyBonus);
        traderData.offeredPrice = traderData.requiredMilkAmount * (baseMilkPrice + (traderData.requiredFreshness / 10));
    }

    public void GenerateNewEggPrice()
    {
        float priceMultiplier = UnityEngine.Random.Range(0.5f, 2.5f);
        traderData.currentEggPrice = Mathf.Round(baseEggPrice * priceMultiplier);
    }

    public void OnAcceptButtonClicked()
    {
        // �ڡڡ� ����: PlayerInventory ��� Warehouse�� CanSellMilk ȣ�� �ڡڡ�
        if (Warehouse.Instance != null && Warehouse.Instance.CanSellMilk(traderData.requiredMilkAmount, traderData.requiredFreshness))
        {
            // �ڡڡ� ����: PlayerInventory ��� Warehouse�� SellMilk ȣ�� �ڡڡ�
            Warehouse.Instance.SellMilk(traderData.requiredMilkAmount);
            gameData.money += traderData.offeredPrice;
            gameData.reputation += reputationPerTrade;
            if (TraderUI.Instance != null)
            {
                TraderUI.Instance.DisplayResult(true, traderData.offeredPrice, reputationPerTrade, "�ŷ��� ���������� �Ϸ��߽��ϴ�!", traderData.eggsSoldToday, traderData.eggRevenueToday);
            }
        }
        else
        {
            gameData.reputation -= 1;
            if (TraderUI.Instance != null)
            {
                TraderUI.Instance.DisplayResult(false, 0, -1, "�䱸 ���� �Ǵ� �ż����� �����Ͽ� �ŷ��� ����Ǿ����ϴ�.", traderData.eggsSoldToday, traderData.eggRevenueToday);
            }
        }
    }

    public void OnDeclineButtonClicked()
    {
        gameData.reputation -= reputationDeclinePenalty;
        if (TraderUI.Instance != null)
        {
            TraderUI.Instance.DisplayResult(false, 0, -reputationDeclinePenalty, "������ �ŷ��� �����Ͽ� ������ ���������ϴ�.", traderData.eggsSoldToday, traderData.eggRevenueToday);
        }
    }

    public void OnHaggleButtonClicked()
    {
        // �ڡڡ� ����: PlayerInventory ��� Warehouse�� CanSellMilk ȣ�� �ڡڡ�
        if (Warehouse.Instance != null && Warehouse.Instance.CanSellMilk(traderData.requiredMilkAmount, traderData.requiredFreshness))
        {
            int haggleChance = UnityEngine.Random.Range(0, 100);
            int successThreshold = 50;

            if (haggleChance <= successThreshold)
            {
                int bonusPrice = traderData.offeredPrice;
                int finalPrice = traderData.offeredPrice + bonusPrice;
                gameData.money += finalPrice;
                // ���� ���� �� ���� ��ȭ�� �����Ƿ� 0���� ����
                gameData.reputation += 0;
                if (TraderUI.Instance != null)
                {
                    TraderUI.Instance.DisplayResult(true, finalPrice, 0, $"������ �����Ͽ� {bonusPrice} ��带 �߰��� �޾ҽ��ϴ�!", traderData.eggsSoldToday, traderData.eggRevenueToday);
                }
            }
            else
            {
                gameData.reputation -= reputationHaggleFailurePenalty;
                if (TraderUI.Instance != null)
                {
                    TraderUI.Instance.DisplayResult(false, 0, -reputationHaggleFailurePenalty, "������ �����߽��ϴ�. ������ ����� ���� ������ ũ�� ���������ϴ�.", traderData.eggsSoldToday, traderData.eggRevenueToday);
                }
            }
        }
        else
        {
            if (TraderUI.Instance != null)
            {
                TraderUI.Instance.DisplayResult(false, 0, 0, "�䱸 ���� �Ǵ� �ż����� �����Ͽ� ������ �õ��� �� �����ϴ�.", traderData.eggsSoldToday, traderData.eggRevenueToday);
            }
        }
    }

    /// <summary>
    /// �ް� �ǸŸ� ó���մϴ�.
    /// </summary>
    public void SellEggs(int count)
    {
        float totalRevenue = count * traderData.currentEggPrice;
        gameData.money += Mathf.RoundToInt(totalRevenue);
        // �ڡڡ� ����: PlayerInventory ��� Warehouse�� RemoveEggs ȣ�� �ڡڡ�
        Warehouse.Instance.RemoveEggs(count);

        traderData.eggsSoldToday += count;
        traderData.eggRevenueToday += Mathf.RoundToInt(totalRevenue);
    }
}

// TraderData Ŭ���� ���Ǹ� ���⿡ �ٽ� �߰��մϴ�.
[System.Serializable]
public class TraderData
{
    public int requiredMilkAmount;
    public int requiredFreshness;
    public int offeredPrice;
    public float currentEggPrice;
    public int eggsSoldToday = 0;
    public int eggRevenueToday = 0;
}