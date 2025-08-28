using UnityEngine;
using TMPro;
using System;
using System.Linq; // LINQ를 사용하기 위해 추가

public class InfoPanelManager : MonoBehaviour
{
    // 싱글톤 인스턴스
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

    // 필요한 매니저들을 참조 변수로 추가
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

        // 목장 정보
        if (pastureManager != null && pastureManager.pastureUpgradeData != null)
        {
            int level = gameManager.gameData.pastureLevel;
            pastureLevelText.text = $"레벨: {level}";
            var freshnessRange = pastureManager.pastureUpgradeData.GetFreshnessRange(level);
            pastureStatsText.text = $"신선도 범위: {freshnessRange.min}% ~ {freshnessRange.max}%";
        }
        else
        {
            if (pastureLevelText != null) pastureLevelText.text = "레벨: 0";
            if (pastureStatsText != null) pastureStatsText.text = "능력치 정보 없음";
        }

        // ★★★ 이제 UpgradeData에서 직접 능력치 정보를 가져옵니다.

        // 바구니 정보
        var basketUpgradeData = allShopItems.FirstOrDefault(i => i.upgradeData is BasketUpgradeData)?.upgradeData as BasketUpgradeData;
        if (basketLevelText != null && basketStatsText != null && basketUpgradeData != null)
        {
            int level = gameManager.gameData.basketLevel;
            int capacity = basketUpgradeData.GetCapacity(level);
            basketLevelText.text = $"레벨: {level}";
            basketStatsText.text = $"용량: {capacity}개";
        }

        // 착유기 정보
        var milkerUpgradeData = allShopItems.FirstOrDefault(i => i.upgradeData is MilkerUpgradeData)?.upgradeData as MilkerUpgradeData;
        if (milkerLevelText != null && milkerStatsText != null && milkerUpgradeData != null)
        {
            int level = gameManager.gameData.milkerLevel;
            int capacity = milkerUpgradeData.GetCapacity(level);
            int milkingYield = milkerUpgradeData.GetMilkingYield(level);
            milkerLevelText.text = $"레벨: {level}";
            milkerStatsText.text = $"용량: {capacity}L\n착유량: {milkingYield}개";
        }

        // 총기 정보
        var gunUpgradeData = allShopItems.FirstOrDefault(i => i.upgradeData is GunUpgradeData)?.upgradeData as GunUpgradeData;
        if (gunLevelText != null && gunStatsText != null && gunUpgradeData != null)
        {
            int level = gameManager.gameData.gunLevel;
            float damage = gunUpgradeData.GetDamage(level);
            gunLevelText.text = $"레벨: {level}";
            gunStatsText.text = $"데미지: {damage:F1}";
        }
    }

    private void UpdateInventoryInfo()
    {
        if (warehouse != null)
        {
            if (milkCountText != null)
                milkCountText.text = $"{warehouse.GetMilkCount()}개";
            if (eggCountText != null)
                eggCountText.text = $"{warehouse.GetEggCount()}개";
            if (avgFreshnessText != null)
                avgFreshnessText.text = $"{warehouse.GetAverageMilkFreshness():F2}%";
        }
        else
        {
            if (milkCountText != null) milkCountText.text = "0개";
            if (eggCountText != null) eggCountText.text = "0개";
            if (avgFreshnessText != null) avgFreshnessText.text = "0.00%";
        }

        if (gameManager != null)
        {
            if (dailyMilkText != null)
                dailyMilkText.text = $"{gameManager.gameData.dailyMilkProduced}개";
            if (dailyEggText != null)
                dailyEggText.text = $"{gameManager.gameData.dailyEggsProduced}개";

            if (bulletsCountText != null)
            {
                bulletsCountText.text = $"총알:{gameManager.gameData.bulletCount}개";
            }
        }
    }
}