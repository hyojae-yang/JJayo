using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TraderUI : MonoBehaviour
{
    public static TraderUI Instance { get; private set; }

    [Header("���� ���� UI")]
    public GameObject traderUIPanel;
    public TextMeshProUGUI requiredAmountText;
    public TextMeshProUGUI requiredFreshnessText;
    public TextMeshProUGUI offeredPriceText;

    [Header("�÷��̾� ���� UI")]
    public TextMeshProUGUI playerMoneyText;
    public TextMeshProUGUI playerMilkCountText;
    public TextMeshProUGUI playerAverageFreshnessText;

    [Header("�ް� �Ǹ� UI")]
    public GameObject eggSellPanel;
    public TextMeshProUGUI eggPriceText;
    public TextMeshProUGUI availableEggCountText;
    public TextMeshProUGUI sellCountText;
    public Button sellEggsButton;
    public Button increaseEggCountButton;
    public Button decreaseEggCountButton;

    [Header("�ŷ� ��� UI")]
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
        // GameManager �ν��Ͻ����� GameData�� ���� �����ɴϴ�.
        gameData = GameManager.Instance.gameData;
        if (gameData == null)
        {
            Debug.LogError("GameData�� ã�� �� �����ϴ�.");
        }

        // ��ư�� �̺�Ʈ �����ʸ� �� ���� �߰��մϴ�.
        if (confirmResultButton != null) confirmResultButton.onClick.AddListener(OnConfirmButtonClicked);
        if (sellEggsButton != null) sellEggsButton.onClick.AddListener(OnSellEggsButtonClicked);
        if (increaseEggCountButton != null) increaseEggCountButton.onClick.AddListener(OnIncreaseEggCount);
        if (decreaseEggCountButton != null) decreaseEggCountButton.onClick.AddListener(OnDecreaseEggCount);
    }

    /// <summary>
    /// ���� UI�� Ȱ��ȭ�ϰ� ���� �ð��� �����մϴ�.
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
    /// ���� UI�� ��Ȱ��ȭ�ϰ� ���� �ð��� �簳�մϴ�.
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
    /// UI �ؽ�Ʈ���� �ֽ� �����ͷ� �����մϴ�.
    /// </summary>
    public void UpdateUI()
    {
        if (traderManager == null)
        {
            Debug.LogError("TraderManager is not assigned!");
            return;
        }

        requiredAmountText.text = traderManager.traderData.requiredMilkAmount.ToString() + " ��";
        requiredFreshnessText.text = traderManager.traderData.requiredFreshness.ToString();
        offeredPriceText.text = traderManager.traderData.offeredPrice.ToString("C0");

        playerMoneyText.text = $"{gameData.money} ��";

        // �ڡڡ� ����: PlayerInventory�� �ƴ� Warehouse���� ���� ������ ���������� ���� �ڡڡ�
        playerMilkCountText.text = $"{Warehouse.Instance.GetMilkCount()} ��";
        playerAverageFreshnessText.text = $"{Warehouse.Instance.GetAverageMilkFreshness():F2}";
        eggPriceText.text = $"������ �ް� �ü�: {traderManager.traderData.currentEggPrice} ��";
    }

    /// <summary>
    /// �ŷ� ����� UI�� ǥ���մϴ�.
    /// </summary>
    public void DisplayResult(bool success, int moneyChange, int reputationChange, string message, int eggsSold = 0, int eggRevenue = 0)
    {
        titleText.text = success ? "�ŷ� ����!" : "�ŷ� ����";
        string moneySign = moneyChange >= 0 ? "+" : "";
        moneyText.text = $"���� �Ǹ�: {moneySign}{moneyChange} ���";
        string repSign = reputationChange >= 0 ? "+" : "";
        reputationText.text = $"���� ��ȭ: {repSign}{reputationChange} ��";
        detailMessageText.text = message;

        if (eggsSold > 0)
        {
            eggResultText.text = $"�ް� �Ǹ�: {eggsSold}�� ({eggRevenue} ���)";
        }
        else
        {
            eggResultText.text = "�ް� �Ǹ�: ����";
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
        // �ڡڡ� ����: PlayerInventory�� �ƴ� Warehouse���� �ް� ������ ���������� ���� �ڡڡ�
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
        // �ڡڡ� ����: PlayerInventory�� �ƴ� Warehouse���� �ް� ������ ���������� ���� �ڡڡ�
        if (availableEggCountText != null && Warehouse.Instance != null)
        {
            availableEggCountText.text = $"���� �ް�: {Warehouse.Instance.GetEggCount()} ��";
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
            // �ް� �Ǹ� ������ TraderManager�� ����
            traderManager.SellEggs(eggsToSell);

            NotificationManager.Instance.ShowNotification($"�ް� {eggsToSell}���� {eggsToSell * traderManager.traderData.currentEggPrice} ��忡 �Ǹ��߽��ϴ�!");

            OnCloseEggPanelButtonClicked();
        }
        else
        {
            NotificationManager.Instance.ShowNotification("�Ǹ��� �ް��� �����ϴ�.");
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