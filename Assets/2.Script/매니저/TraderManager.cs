using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class TraderManager : MonoBehaviour
{
    public static TraderManager Instance { get; private set; }

    [Header("Dependencies")]
    public Trader traderData;        // Trader.cs 스크립트 (요구 조건 데이터)
    public GameObject traderUIPanel;    // 상인 UI 패널 (부모 오브젝트)
    public GameObject tradeResultPanel; // 거래 결과를 표시할 모달 UI 패널 (에디터에서 연결해야 합니다!)

    [Header("UI Text Components")]
    // 상인 요구 조건 표시용 텍스트
    public TextMeshProUGUI requiredAmountText; // 요구 개수
    public TextMeshProUGUI requiredFreshnessText; // 요구 신선도
    public TextMeshProUGUI offeredPriceText; // 제시 가격

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

    [Header("달걀 판매 UI")]
    public GameObject eggSellPanel; // 달걀 판매 패널
    public TextMeshProUGUI eggPriceText; // 오늘 달걀 시세 텍스트
    public TextMeshProUGUI availableEggCountText; // 현재 보유 달걀 개수 텍스트
    public TextMeshProUGUI sellCountText; // 판매할 달걀 개수 텍스트

    [Header("달걀 거래 설정")]
    public float baseEggPrice = 100f;
    [HideInInspector]
    public float currentEggPrice; // 매일 변동될 달걀 가격

    private int eggsToSell = 0;
    private int eggsSoldToday = 0; // 오늘 판매한 달걀 개수
    private int eggRevenueToday = 0; // 오늘 달걀 판매로 벌어들인 총 금액

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
    /// GameManager에서 하루 종료 시 호출되는 함수. 상인 등장 및 거래 시작.
    /// </summary>
    public void StartTrade()
    {
        Time.timeScale = 0;
        eggsSoldToday = 0; // 하루 시작 시 초기화
        eggRevenueToday = 0; // 하루 시작 시 초기화
        GenerateTradeDemand();
        GenerateNewEggPrice(); // 달걀 가격 갱신
        UpdateUITexts();
        if (traderUIPanel != null)
        {
            traderUIPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 상인의 요구 조건(개수, 신선도, 가격)을 랜덤하게 생성합니다.
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
    /// 매일 달걀 시세를 갱신합니다.
    /// </summary>
    public void GenerateNewEggPrice()
    {
        float priceMultiplier = UnityEngine.Random.Range(0.5f, 2.5f);
        currentEggPrice = Mathf.Round(baseEggPrice * priceMultiplier);

        if (eggPriceText != null)
        {
            eggPriceText.text = $"오늘의 달걀 시세: {currentEggPrice} 골드";
        }
    }

    /// <summary>
    /// 생성된 요구 조건 데이터를 UI 텍스트에 반영합니다.
    /// </summary>
    private void UpdateUITexts()
    {
        if (requiredAmountText != null)
        {
            requiredAmountText.text = traderData.requiredMilkAmount.ToString() + " 개 요구";
        }
        if (requiredFreshnessText != null)
        {
            requiredFreshnessText.text = traderData.requiredFreshness.ToString();
        }
        if (offeredPriceText != null)
        {
            offeredPriceText.text = traderData.offeredPrice.ToString() + " 골드";
        }

        if (playerMoneyText != null)
        {
            playerMoneyText.text = $"{GameManager.Instance.CurrentMoney} 골드";
        }
        if (PlayerInventory.Instance != null)
        {
            if (playerMilkCountText != null)
            {
                playerMilkCountText.text = $"{PlayerInventory.Instance.GetMilkCount()} 개";
            }
            if (playerAverageFreshnessText != null)
            {
                playerAverageFreshnessText.text = $"{PlayerInventory.Instance.GetAverageFreshness():F2} 점";
            }
        }
    }

    /// <summary>
    /// '수락' 버튼 클릭 시 호출됩니다.
    /// </summary>
    public void OnAcceptButtonClicked()
    {
        if (PlayerInventory.Instance != null && PlayerInventory.Instance.CanSellMilk(traderData.requiredMilkAmount, traderData.requiredFreshness))
        {
            // 1. 거래 진행
            PlayerInventory.Instance.SellMilk(traderData.requiredMilkAmount);
            GameManager.Instance.AddMoney(traderData.offeredPrice);
            GameManager.Instance.ChangeReputation(reputationPerTrade);

            // 2. 결과 알림창 띄우기
            if (TradeResultUI.Instance != null)
            {
                TradeResultUI.Instance.DisplayResult(true, traderData.offeredPrice, reputationPerTrade, "거래를 성공적으로 완료했습니다!", eggsSoldToday, eggRevenueToday);
            }
        }
        else
        {
            // 거래 실패
            GameManager.Instance.ChangeReputation(-1);

            if (TradeResultUI.Instance != null)
            {
                TradeResultUI.Instance.DisplayResult(false, 0, -1, "요구 개수 또는 신선도가 부족하여 거래가 무산되었습니다.", eggsSoldToday, eggRevenueToday);
            }
        }
    }

    /// <summary>
    /// '거부' 버튼 클릭 시 호출됩니다.
    /// </summary>
    public void OnDeclineButtonClicked()
    {
        GameManager.Instance.ChangeReputation(-reputationDeclinePenalty);

        if (TradeResultUI.Instance != null)
        {
            TradeResultUI.Instance.DisplayResult(false, 0, -reputationDeclinePenalty, "상인의 거래를 거절하여 명성도가 떨어졌습니다.", eggsSoldToday, eggRevenueToday);
        }
    }

    /// <summary>
    /// '흥정' 버튼 클릭 시 호출됩니다.
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
                    TradeResultUI.Instance.DisplayResult(true, finalPrice, 0, $"흥정에 성공하여 {bonusPrice} 골드를 추가로 받았습니다!", eggsSoldToday, eggRevenueToday);
                }
            }
            else
            {
                GameManager.Instance.ChangeReputation(-reputationHaggleFailurePenalty);

                if (TradeResultUI.Instance != null)
                {
                    TradeResultUI.Instance.DisplayResult(false, 0, -reputationHaggleFailurePenalty, "흥정에 실패했습니다. 상인의 기분이 나빠 명성도가 크게 떨어졌습니다.", eggsSoldToday, eggRevenueToday);
                }
            }
        }
        else
        {
            if (TradeResultUI.Instance != null)
            {
                TradeResultUI.Instance.DisplayResult(false, 0, 0, "요구 개수 또는 신선도가 부족하여 흥정을 시도할 수 없습니다.", eggsSoldToday, eggRevenueToday);
            }
        }
    }

    // --- 달걀 판매 시스템 추가 ---

    /// <summary>
    /// '달걀 판매' 버튼 클릭 시 호출됩니다.
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
    /// 달걀 판매 개수 증가 버튼
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
    /// 달걀 판매 개수 감소 버튼
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
    /// UI 텍스트를 업데이트합니다.
    /// </summary>
    private void UpdateEggUI()
    {
        if (availableEggCountText != null)
        {
            availableEggCountText.text = $"보유 달걀: {Warehouse.Instance.GetEggCount()} 개";
        }
        if (sellCountText != null)
        {
            sellCountText.text = eggsToSell.ToString();
        }
    }

    /// <summary>
    /// '판매' 버튼 클릭 시 호출됩니다.
    /// </summary>
    public void OnSellEggsButtonClicked()
    {
        if (eggsToSell > 0)
        {
            float totalRevenue = eggsToSell * currentEggPrice;
            GameManager.Instance.AddMoney(Mathf.RoundToInt(totalRevenue));

            Warehouse.Instance.RemoveEggs(eggsToSell);

            // 판매 결과 저장
            eggsSoldToday += eggsToSell;
            eggRevenueToday += Mathf.RoundToInt(totalRevenue);

            NotificationManager.Instance.ShowNotification($"달걀 {eggsToSell}개를 {Mathf.RoundToInt(totalRevenue)} 골드에 판매했습니다!");

            OnCloseEggPanelButtonClicked();
        }
        else
        {
            NotificationManager.Instance.ShowNotification("판매한 달걀이 없습니다.");
        }
    }

    /// <summary>
    /// '닫기' 버튼 클릭 시 호출됩니다.
    /// </summary>
    public void OnCloseEggPanelButtonClicked()
    {
        if (eggSellPanel != null)
        {
            eggSellPanel.SetActive(false);
        }
    }
}