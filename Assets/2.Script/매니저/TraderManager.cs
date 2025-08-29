using UnityEngine;
using System.Linq;
using UnityEditor;

public class TraderManager : MonoBehaviour
{
    public static TraderManager Instance { get; private set; }

    [Header("Dependencies")]
    private GameData gameData;
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

    [Header("달걀 거래 설정")]
    public int baseEggPrice = 10;

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
        if (gameData == null) Debug.LogError("GameData를 찾을 수 없습니다.");

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnDayChanged += OnDayEnd;
        }
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnDayChanged -= OnDayEnd;
        }
    }

    private void OnDayEnd(int day)
    {
        StartTrade();
        if (TraderUI.Instance != null)
        {
            TraderUI.Instance.ShowTraderUI();
        }
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

        traderData.offeredPrice = traderData.requiredMilkAmount * (baseMilkPrice + (int)(traderData.requiredFreshness / 10f));
    }

    public void GenerateNewEggPrice()
    {
        int currentBasePrice = (gameData.day == 1) ? baseEggPrice : traderData.currentEggPrice;

        float priceMultiplier = UnityEngine.Random.Range(0.1f, 3.0f);

        float newPrice = currentBasePrice * priceMultiplier;

        // ★★★ 수정된 부분: 가격을 올림(Ceiling)하고 최소값을 1로 보장 ★★★
        traderData.currentEggPrice = Mathf.Max(1, Mathf.CeilToInt(newPrice));
    }

    public void OnAcceptButtonClicked()
    {
        if (Warehouse.Instance != null && Warehouse.Instance.CanSellMilk(traderData.requiredMilkAmount, traderData.requiredFreshness))
        {
            Warehouse.Instance.SellMilk(traderData.requiredMilkAmount);
            MoneyManager.Instance.AddMoney(traderData.offeredPrice);
            GameManager.Instance.ChangeReputation(reputationPerTrade);
            if (TraderUI.Instance != null)
            {
                TraderUI.Instance.DisplayResult(true, traderData.offeredPrice, reputationPerTrade, "거래를 성공적으로 완료했습니다!", traderData.eggsSoldToday, traderData.eggRevenueToday);
            }
        }
        else
        {
            GameManager.Instance.ChangeReputation(-1);
            if (TraderUI.Instance != null)
            {
                TraderUI.Instance.DisplayResult(false, 0, -1, "요구 개수 또는 신선도가 부족하여 거래가 무산되었습니다.", traderData.eggsSoldToday, traderData.eggRevenueToday);
            }
        }
    }

    public void OnDeclineButtonClicked()
    {
        GameManager.Instance.ChangeReputation(-reputationDeclinePenalty);
        if (TraderUI.Instance != null)
        {
            TraderUI.Instance.DisplayResult(false, 0, -reputationDeclinePenalty, "상인의 거래를 거절하여 명성도가 떨어졌습니다.", traderData.eggsSoldToday, traderData.eggRevenueToday);
        }
    }

    public void OnHaggleButtonClicked()
    {
        if (Warehouse.Instance != null && Warehouse.Instance.CanSellMilk(traderData.requiredMilkAmount, traderData.requiredFreshness))
        {
            int haggleChance = UnityEngine.Random.Range(0, 100);
            int successThreshold = 50;
            if (haggleChance <= successThreshold)
            {
                int bonusPrice = traderData.offeredPrice;
                int finalPrice = traderData.offeredPrice + bonusPrice;
                MoneyManager.Instance.AddMoney(finalPrice);
                GameManager.Instance.ChangeReputation(0);
                if (TraderUI.Instance != null)
                {
                    TraderUI.Instance.DisplayResult(true, finalPrice, 0, $"흥정에 성공하여 {bonusPrice} 골드를 추가로 받았습니다!", traderData.eggsSoldToday, traderData.eggRevenueToday);
                }
            }
            else
            {
                GameManager.Instance.ChangeReputation(-reputationHaggleFailurePenalty);
                if (TraderUI.Instance != null)
                {
                    TraderUI.Instance.DisplayResult(false, 0, -reputationHaggleFailurePenalty, "흥정에 실패했습니다. 상인의 기분이 나빠 명성도가 크게 떨어졌습니다.", traderData.eggsSoldToday, traderData.eggRevenueToday);
                }
            }
        }
        else
        {
            if (TraderUI.Instance != null)
            {
                TraderUI.Instance.DisplayResult(false, 0, 0, "요구 개수 또는 신선도가 부족하여 흥정을 시도할 수 없습니다.", traderData.eggsSoldToday, traderData.eggRevenueToday);
            }
        }
    }

    public void SellEggs(int count)
    {
        int totalRevenue = count * traderData.currentEggPrice;
        MoneyManager.Instance.AddMoney(totalRevenue);
        Warehouse.Instance.RemoveEggs(count);
        traderData.eggsSoldToday += count;
        traderData.eggRevenueToday += totalRevenue;
    }
}

[System.Serializable]
public class TraderData
{
    public int requiredMilkAmount;
    public int requiredFreshness;
    public int offeredPrice;
    public int currentEggPrice;
    public int eggsSoldToday = 0;
    public int eggRevenueToday = 0;
}