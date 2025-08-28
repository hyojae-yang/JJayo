using UnityEngine;
using TMPro;
using System;
using System.Linq; // LINQ�� ����ϱ� ���� �߰�

public class InfoPanelManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static InfoPanelManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject mainInfoPanel;
    public GameObject panel1_UpgradePanel;
    public GameObject panel2_InventoryPanel;
    public GameObject panel3_CowInfoPanel;
    public GameObject panel4_CowPlacementPanel;

    [Header("Panel 1: Upgrade UI Texts")]
    public TextMeshProUGUI pastureLevelText;
    public TextMeshProUGUI pastureStatsText;
    public TextMeshProUGUI basketLevelText;
    public TextMeshProUGUI basketStatsText;
    public TextMeshProUGUI milkerLevelText;
    public TextMeshProUGUI milkerStatsText;
    public TextMeshProUGUI gunLevelText;
    public TextMeshProUGUI gunStatsText;

    [Header("Panel 2: Inventory UI Texts")]
    public TextMeshProUGUI milkCountText;
    public TextMeshProUGUI eggCountText;
    public TextMeshProUGUI avgFreshnessText;
    public TextMeshProUGUI dailyMilkText;
    public TextMeshProUGUI dailyEggText;
    public TextMeshProUGUI bulletsCountText;

    // �ʿ��� �Ŵ������� ���� ������ �߰�
    private GameManager gameManager;
    private PlayerInventory playerInventory;
    private Warehouse warehouse;
    private PastureManager pastureManager;

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

    void Start()
    {
        gameManager = GameManager.Instance;
        playerInventory = PlayerInventory.Instance;
        warehouse = Warehouse.Instance;
        pastureManager = PastureManager.Instance;
    }

    public void ToggleInfoPanel()
    {
        bool isActive = mainInfoPanel.activeSelf;
        mainInfoPanel.SetActive(!isActive);
        Time.timeScale = isActive ? 1 : 0;

        if (!isActive)
        {
            ShowPanel(1);
        }
    }

    public void ShowPanel(int panelIndex)
    {
        panel1_UpgradePanel.SetActive(false);
        panel2_InventoryPanel.SetActive(false);
        panel3_CowInfoPanel.SetActive(false);
        panel4_CowPlacementPanel.SetActive(false);

        switch (panelIndex)
        {
            case 1:
                panel1_UpgradePanel.SetActive(true);
                UpdateUpgradeInfo();
                break;
            case 2:
                panel2_InventoryPanel.SetActive(true);
                UpdateInventoryInfo();
                break;
            case 3:
                panel3_CowInfoPanel.SetActive(true);
                break;
            case 4:
                panel4_CowPlacementPanel.SetActive(true);
                break;
        }
    }

    private void UpdateUpgradeInfo()
    {
        var allShopItems = ShopService.Instance.GetShopItems();

        // ���� ����
        if (pastureManager != null && pastureManager.pastureUpgradeData != null)
        {
            int level = gameManager.gameData.pastureLevel;
            pastureLevelText.text = $"����: {level}";
            var freshnessRange = pastureManager.pastureUpgradeData.GetFreshnessRange(level);
            pastureStatsText.text = $"�ż��� ����: {freshnessRange.min}% ~ {freshnessRange.max}%";
        }
        else
        {
            if (pastureLevelText != null) pastureLevelText.text = "����: 0";
            if (pastureStatsText != null) pastureStatsText.text = "�ɷ�ġ ���� ����";
        }

        // �ڡڡ� ���� UpgradeData���� ���� �ɷ�ġ ������ �����ɴϴ�.

        // �ٱ��� ����
        var basketUpgradeData = allShopItems.FirstOrDefault(i => i.upgradeData is BasketUpgradeData)?.upgradeData as BasketUpgradeData;
        if (basketLevelText != null && basketStatsText != null && basketUpgradeData != null)
        {
            int level = gameManager.gameData.basketLevel;
            int capacity = basketUpgradeData.GetCapacity(level);
            basketLevelText.text = $"����: {level}";
            basketStatsText.text = $"�뷮: {capacity}��";
        }

        // ������ ����
        var milkerUpgradeData = allShopItems.FirstOrDefault(i => i.upgradeData is MilkerUpgradeData)?.upgradeData as MilkerUpgradeData;
        if (milkerLevelText != null && milkerStatsText != null && milkerUpgradeData != null)
        {
            int level = gameManager.gameData.milkerLevel;
            int capacity = milkerUpgradeData.GetCapacity(level);
            int milkingYield = milkerUpgradeData.GetMilkingYield(level);
            milkerLevelText.text = $"����: {level}";
            milkerStatsText.text = $"�뷮: {capacity}L\n������: {milkingYield}��";
        }

        // �ѱ� ����
        var gunUpgradeData = allShopItems.FirstOrDefault(i => i.upgradeData is GunUpgradeData)?.upgradeData as GunUpgradeData;
        if (gunLevelText != null && gunStatsText != null && gunUpgradeData != null)
        {
            int level = gameManager.gameData.gunLevel;
            float damage = gunUpgradeData.GetDamage(level);
            gunLevelText.text = $"����: {level}";
            gunStatsText.text = $"������: {damage:F1}";
        }
    }

    private void UpdateInventoryInfo()
    {
        if (warehouse != null)
        {
            if (milkCountText != null)
                milkCountText.text = $"{warehouse.GetMilkCount()}��";
            if (eggCountText != null)
                eggCountText.text = $"{warehouse.GetEggCount()}��";
            if (avgFreshnessText != null)
                avgFreshnessText.text = $"{warehouse.GetAverageMilkFreshness():F2}%";
        }
        else
        {
            if (milkCountText != null) milkCountText.text = "0��";
            if (eggCountText != null) eggCountText.text = "0��";
            if (avgFreshnessText != null) avgFreshnessText.text = "0.00%";
        }

        if (gameManager != null)
        {
            if (dailyMilkText != null)
                dailyMilkText.text = $"{gameManager.gameData.dailyMilkProduced}��";
            if (dailyEggText != null)
                dailyEggText.text = $"{gameManager.gameData.dailyEggsProduced}��";

            if (bulletsCountText != null)
            {
                bulletsCountText.text = $"�Ѿ�:{gameManager.gameData.bulletCount}��";
            }
        }
    }
}