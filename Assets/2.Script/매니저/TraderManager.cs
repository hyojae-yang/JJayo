using UnityEngine;
using System.Linq;
using UnityEditor;

public class TraderManager : MonoBehaviour
{
    public static TraderManager Instance { get; private set; }

    [Header("Dependencies")]
    private GameData gameData; // GameData 객체를 직접 참조
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
        if (gameData == null) Debug.LogError("GameData를 찾을 수 없습니다.");
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
        // ★★★ 수정: PlayerInventory 대신 Warehouse의 CanSellMilk 호출 ★★★
        if (Warehouse.Instance != null && Warehouse.Instance.CanSellMilk(traderData.requiredMilkAmount, traderData.requiredFreshness))
        {
            // ★★★ 수정: PlayerInventory 대신 Warehouse의 SellMilk 호출 ★★★
            Warehouse.Instance.SellMilk(traderData.requiredMilkAmount);
            gameData.money += traderData.offeredPrice;
            gameData.reputation += reputationPerTrade;
            if (TraderUI.Instance != null)
            {
                TraderUI.Instance.DisplayResult(true, traderData.offeredPrice, reputationPerTrade, "거래를 성공적으로 완료했습니다!", traderData.eggsSoldToday, traderData.eggRevenueToday);
            }
        }
        else
        {
            gameData.reputation -= 1;
            if (TraderUI.Instance != null)
            {
                TraderUI.Instance.DisplayResult(false, 0, -1, "요구 개수 또는 신선도가 부족하여 거래가 무산되었습니다.", traderData.eggsSoldToday, traderData.eggRevenueToday);
            }
        }
    }

    public void OnDeclineButtonClicked()
    {
        gameData.reputation -= reputationDeclinePenalty;
        if (TraderUI.Instance != null)
        {
            TraderUI.Instance.DisplayResult(false, 0, -reputationDeclinePenalty, "상인의 거래를 거절하여 명성도가 떨어졌습니다.", traderData.eggsSoldToday, traderData.eggRevenueToday);
        }
    }

    public void OnHaggleButtonClicked()
    {
        // ★★★ 수정: PlayerInventory 대신 Warehouse의 CanSellMilk 호출 ★★★
        if (Warehouse.Instance != null && Warehouse.Instance.CanSellMilk(traderData.requiredMilkAmount, traderData.requiredFreshness))
        {
            int haggleChance = UnityEngine.Random.Range(0, 100);
            int successThreshold = 50;

            if (haggleChance <= successThreshold)
            {
                int bonusPrice = traderData.offeredPrice;
                int finalPrice = traderData.offeredPrice + bonusPrice;
                gameData.money += finalPrice;
                // 흥정 성공 시 명성도 변화가 없으므로 0으로 설정
                gameData.reputation += 0;
                if (TraderUI.Instance != null)
                {
                    TraderUI.Instance.DisplayResult(true, finalPrice, 0, $"흥정에 성공하여 {bonusPrice} 골드를 추가로 받았습니다!", traderData.eggsSoldToday, traderData.eggRevenueToday);
                }
            }
            else
            {
                gameData.reputation -= reputationHaggleFailurePenalty;
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

    /// <summary>
    /// 달걀 판매를 처리합니다.
    /// </summary>
    public void SellEggs(int count)
    {
        float totalRevenue = count * traderData.currentEggPrice;
        gameData.money += Mathf.RoundToInt(totalRevenue);
        // ★★★ 수정: PlayerInventory 대신 Warehouse의 RemoveEggs 호출 ★★★
        Warehouse.Instance.RemoveEggs(count);

        traderData.eggsSoldToday += count;
        traderData.eggRevenueToday += Mathf.RoundToInt(totalRevenue);
    }
}

// TraderData 클래스 정의를 여기에 다시 추가합니다.
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