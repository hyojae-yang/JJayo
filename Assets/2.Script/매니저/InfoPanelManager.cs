using UnityEngine;
using TMPro;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class InfoPanelManager : MonoBehaviour
{
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
    // ★★★ 창고 업그레이드 UI 텍스트 변수 추가 ★★★
    public TextMeshProUGUI warehouseLevelText;
    public TextMeshProUGUI warehouseStatsText;

    [Header("Panel 2: Inventory UI Texts")]
    public TextMeshProUGUI milkCountText;
    public TextMeshProUGUI eggCountText;
    public TextMeshProUGUI avgFreshnessText;
    public TextMeshProUGUI dailyMilkText;
    public TextMeshProUGUI dailyEggText;
    public TextMeshProUGUI bulletsCountText;

    [Header("General UI")]
    public TextMeshProUGUI reputationText;

    [Header("Panel 3: Cow Info UI")]
    public TextMeshProUGUI chickenCountText;
    public Transform cowCardContentParent;
    public GameObject cowInfoCardPrefab;

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

        UpdateReputationUI();
        UpdateBulletCountUI();
    }

    public void UpdateReputationUI()
    {
        if (reputationText != null && gameManager != null && gameManager.CurrentGameData != null)
        {
            reputationText.text = $"명성도: {gameManager.CurrentGameData.reputation}";
        }
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
                UpdateCowInfoPanel();
                break;
            case 4:
                panel4_CowPlacementPanel.SetActive(true);
                break;
        }
    }

    private void UpdateUpgradeInfo()
    {
        var allShopItems = ShopService.Instance.GetShopItems();

        if (pastureManager != null && pastureManager.pastureUpgradeData != null)
        {
            int level = gameManager.CurrentGameData.pastureLevel;
            pastureLevelText.text = $"레벨: {level}";
            var freshnessRange = pastureManager.pastureUpgradeData.GetFreshnessRange(level);
            pastureStatsText.text = $"신선도 범위: {freshnessRange.min}% ~ {freshnessRange.max}%";
        }
        else
        {
            if (pastureLevelText != null) pastureLevelText.text = "레벨: 0";
            if (pastureStatsText != null) pastureStatsText.text = "능력치 정보 없음";
        }

        var basketUpgradeData = allShopItems.FirstOrDefault(i => i.upgradeData is BasketUpgradeData)?.upgradeData as BasketUpgradeData;
        if (basketLevelText != null && basketStatsText != null && basketUpgradeData != null)
        {
            int level = gameManager.CurrentGameData.basketLevel;
            int capacity = basketUpgradeData.GetCapacity(level);
            basketLevelText.text = $"레벨: {level}";
            basketStatsText.text = $"용량: {capacity}개";
        }

        var milkerUpgradeData = allShopItems.FirstOrDefault(i => i.upgradeData is MilkerUpgradeData)?.upgradeData as MilkerUpgradeData;
        if (milkerLevelText != null && milkerStatsText != null && milkerUpgradeData != null)
        {
            int level = gameManager.CurrentGameData.milkerLevel;
            int capacity = milkerUpgradeData.GetCapacity(level);
            int milkingYield = milkerUpgradeData.GetMilkingYield(level);
            milkerLevelText.text = $"레벨: {level}";
            milkerStatsText.text = $"용량: {capacity}L\n착유량: {milkingYield}개";
        }

        var gunUpgradeData = allShopItems.FirstOrDefault(i => i.upgradeData is GunUpgradeData)?.upgradeData as GunUpgradeData;
        if (gunLevelText != null && gunStatsText != null && gunUpgradeData != null)
        {
            int level = gameManager.CurrentGameData.gunLevel;
            float damage = gunUpgradeData.GetDamage(level);
            gunLevelText.text = $"레벨: {level}";
            gunStatsText.text = $"데미지: {damage:F1}";
        }

        // ★★★ 새로 추가된 창고 업그레이드 정보 UI 업데이트 로직 ★★★
        var warehouseUpgradeData = allShopItems.FirstOrDefault(i => i.upgradeData is WarehouseUpgradeData)?.upgradeData as WarehouseUpgradeData;
        if (warehouseLevelText != null && warehouseStatsText != null && warehouseUpgradeData != null)
        {
            int level = gameManager.CurrentGameData.warehouseLevel;
            float freshnessMultiplier = warehouseUpgradeData.GetFreshnessDecayMultiplier(level);
            // 1.0f - multiplier를 계산하여 몇 퍼센트가 '감소'하는지 표시합니다.
            string percentage = $"{((1 - freshnessMultiplier) * 100):F0}";
            warehouseLevelText.text = $"레벨: {level}";
            warehouseStatsText.text = $"신선도 감소 속도: {percentage}% 감소";
        }
        else
        {
            if (warehouseLevelText != null) warehouseLevelText.text = "레벨: 0";
            if (warehouseStatsText != null) warehouseStatsText.text = "능력치 정보 없음";
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
                dailyMilkText.text = $"{gameManager.CurrentGameData.dailyMilkProduced}개";
            if (dailyEggText != null)
                dailyEggText.text = $"{gameManager.CurrentGameData.dailyEggsProduced}개";
        }

        UpdateBulletCountUI();
    }

    public void UpdateBulletCountUI()
    {
        if (bulletsCountText != null && gameManager != null && gameManager.CurrentGameData != null)
        {
            bulletsCountText.text = $"총알: {gameManager.CurrentGameData.bulletCount}개";
        }
    }

    private void UpdateCowInfoPanel()
    {
        if (gameManager == null || gameManager.CurrentGameData == null) return;

        if (chickenCountText != null)
        {
            chickenCountText.text = $"닭 마릿수: {gameManager.CurrentGameData.chickenCount}마리";
        }

        foreach (Transform child in cowCardContentParent)
        {
            Destroy(child.gameObject);
        }

        if (AnimalManager.Instance != null && AnimalManager.Instance.activeAnimals != null)
        {
            foreach (Animal animal in AnimalManager.Instance.activeAnimals)
            {
                if (animal.animalData.animalType == AnimalType.Cow)
                {
                    GameObject newCard = Instantiate(cowInfoCardPrefab, cowCardContentParent);
                    CowInfoCard cardScript = newCard.GetComponent<CowInfoCard>();
                    if (cardScript != null)
                    {
                        cardScript.SetupCowInfo(animal);
                    }
                }
            }
        }
    }

    public void GoToTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
        GameManager.Instance.ResetGameData();
    }

    public void OnSaveButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.SaveGame();
            NotificationManager.Instance.ShowNotification($"게임이 저장되었습니다!");
        }
    }
}