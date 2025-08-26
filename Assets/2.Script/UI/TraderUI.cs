using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TraderUI : MonoBehaviour
{
    public static TraderUI Instance { get; private set; }

    [Header("메인 상인 UI")]
    public GameObject traderUIPanel;
    public TextMeshProUGUI requiredAmountText;
    public TextMeshProUGUI requiredFreshnessText;
    public TextMeshProUGUI offeredPriceText;

    [Header("플레이어 상태 UI")]
    public TextMeshProUGUI playerMoneyText;
    public TextMeshProUGUI playerMilkCountText;
    public TextMeshProUGUI playerAverageFreshnessText;

    [Header("달걀 판매 UI")]
    public GameObject eggSellPanel;
    public TextMeshProUGUI eggPriceText;
    public TextMeshProUGUI availableEggCountText;
    public TextMeshProUGUI sellCountText;
    public Button sellEggsButton;
    public Button increaseEggCountButton;
    public Button decreaseEggCountButton;

    [Header("거래 결과 UI")]
    public GameObject resultPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI reputationText;
    public TextMeshProUGUI detailMessageText;
    public TextMeshProUGUI eggResultText;
    public Button confirmResultButton;

    [Header("Dependencies")]
    public TraderManager traderManager;
    private GameData gameData;

    private int eggsToSell = 0;

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

    private void Start()
    {
        // GameManager 인스턴스에서 GameData를 직접 가져옵니다.
        gameData = GameManager.Instance.gameData;
        if (gameData == null)
        {
            Debug.LogError("GameData를 찾을 수 없습니다.");
        }

        // 버튼에 이벤트 리스너를 한 번만 추가합니다.
        if (confirmResultButton != null) confirmResultButton.onClick.AddListener(OnConfirmButtonClicked);
        if (sellEggsButton != null) sellEggsButton.onClick.AddListener(OnSellEggsButtonClicked);
        if (increaseEggCountButton != null) increaseEggCountButton.onClick.AddListener(OnIncreaseEggCount);
        if (decreaseEggCountButton != null) decreaseEggCountButton.onClick.AddListener(OnDecreaseEggCount);
    }

    /// <summary>
    /// 상인 UI를 활성화하고 게임 시간을 정지합니다.
    /// </summary>
    public void ShowTraderUI()
    {
        if (traderUIPanel != null)
        {
            traderUIPanel.SetActive(true);
            Time.timeScale = 0;
            UpdateUI();
        }
    }

    /// <summary>
    /// 상인 UI를 비활성화하고 게임 시간을 재개합니다.
    /// </summary>
    public void HideTraderUI()
    {
        if (traderUIPanel != null)
        {
            traderUIPanel.SetActive(false);
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// UI 텍스트들을 최신 데이터로 갱신합니다.
    /// </summary>
    public void UpdateUI()
    {
        if (traderManager == null)
        {
            Debug.LogError("TraderManager is not assigned!");
            return;
        }

        requiredAmountText.text = traderManager.traderData.requiredMilkAmount.ToString() + " 개";
        requiredFreshnessText.text = traderManager.traderData.requiredFreshness.ToString();
        offeredPriceText.text = traderManager.traderData.offeredPrice.ToString("C0");

        playerMoneyText.text = $"{gameData.money} 원";

        // ★★★ 수정: PlayerInventory가 아닌 Warehouse에서 우유 정보를 가져오도록 변경 ★★★
        playerMilkCountText.text = $"{Warehouse.Instance.GetMilkCount()} 개";
        playerAverageFreshnessText.text = $"{Warehouse.Instance.GetAverageMilkFreshness():F2}";
        eggPriceText.text = $"오늘의 달걀 시세: {traderManager.traderData.currentEggPrice} 원";
    }

    /// <summary>
    /// 거래 결과를 UI에 표시합니다.
    /// </summary>
    public void DisplayResult(bool success, int moneyChange, int reputationChange, string message, int eggsSold = 0, int eggRevenue = 0)
    {
        titleText.text = success ? "거래 성공!" : "거래 실패";
        string moneySign = moneyChange >= 0 ? "+" : "";
        moneyText.text = $"우유 판매: {moneySign}{moneyChange} 골드";
        string repSign = reputationChange >= 0 ? "+" : "";
        reputationText.text = $"명성도 변화: {repSign}{reputationChange} 점";
        detailMessageText.text = message;

        if (eggsSold > 0)
        {
            eggResultText.text = $"달걀 판매: {eggsSold}개 ({eggRevenue} 골드)";
        }
        else
        {
            eggResultText.text = "달걀 판매: 없음";
        }

        resultPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void OnConfirmButtonClicked()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
        HideTraderUI();
    }

    public void OnOpenEggPanelButtonClicked()
    {
        if (eggSellPanel != null)
        {
            eggSellPanel.SetActive(true);
        }
        eggsToSell = 0;
        UpdateEggUI();
    }

    public void OnIncreaseEggCount()
    {
        // ★★★ 수정: PlayerInventory가 아닌 Warehouse에서 달걀 개수를 가져오도록 변경 ★★★
        if (eggsToSell < Warehouse.Instance.GetEggCount())
        {
            eggsToSell++;
            UpdateEggUI();
        }
    }

    public void OnDecreaseEggCount()
    {
        if (eggsToSell > 0)
        {
            eggsToSell--;
            UpdateEggUI();
        }
    }

    private void UpdateEggUI()
    {
        // ★★★ 수정: PlayerInventory가 아닌 Warehouse에서 달걀 개수를 가져오도록 변경 ★★★
        if (availableEggCountText != null && Warehouse.Instance != null)
        {
            availableEggCountText.text = $"보유 달걀: {Warehouse.Instance.GetEggCount()} 개";
        }
        if (sellCountText != null)
        {
            sellCountText.text = eggsToSell.ToString();
        }
    }

    public void OnSellEggsButtonClicked()
    {
        if (eggsToSell > 0)
        {
            // 달걀 판매 로직을 TraderManager에 위임
            traderManager.SellEggs(eggsToSell);

            NotificationManager.Instance.ShowNotification($"달걀 {eggsToSell}개를 {eggsToSell * traderManager.traderData.currentEggPrice} 골드에 판매했습니다!");

            OnCloseEggPanelButtonClicked();
        }
        else
        {
            NotificationManager.Instance.ShowNotification("판매한 달걀이 없습니다.");
        }
    }

    public void OnCloseEggPanelButtonClicked()
    {
        if (eggSellPanel != null)
        {
            eggSellPanel.SetActive(false);
        }
    }
}