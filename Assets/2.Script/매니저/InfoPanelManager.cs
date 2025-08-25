using UnityEngine;
using TMPro;
using System;

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
        if (pastureLevelText != null && pastureStatsText != null && GameManager.Instance != null)
        {
            int level = GameManager.Instance.CurrentPastureLevel;
            pastureLevelText.text = $"레벨: {level}";

            if (GameManager.Instance.pastureUpgradeData != null)
            {
                var freshnessRange = GameManager.Instance.pastureUpgradeData.GetFreshnessRange(level);
                pastureStatsText.text = $"신선도 범위: {freshnessRange.min}% ~ {freshnessRange.max}%";
            }
            else
            {
                pastureStatsText.text = "능력치 정보 없음";
            }
        }

        if (PlayerInventory.Instance != null)
        {
            // 바구니 정보
            if (basketLevelText != null && basketStatsText != null)
            {
                int level = PlayerInventory.Instance.basketLevel;
                int capacity = PlayerInventory.Instance.BasketCapacity;
                basketLevelText.text = $"레벨: {level}";
                basketStatsText.text = $"용량: {capacity}개";
            }

            // 착유기 정보
            if (milkerLevelText != null && milkerStatsText != null)
            {
                int level = PlayerInventory.Instance.milkerLevel;
                int capacity = PlayerInventory.Instance.MilkerCapacity;
                // ★★★ 수정된 부분: 이제 착유량을 가져옵니다. ★★★
                int milkingYield = PlayerInventory.Instance.MilkingYield;
                milkerLevelText.text = $"레벨: {level}";
                // ★★★ 수정된 부분: 착유량을 UI에 표시합니다. ★★★
                milkerStatsText.text = $"용량: {capacity}L\n착유량: {milkingYield}개";
            }

            // 총 정보
            if (gunLevelText != null && gunStatsText != null)
            {
                int level = PlayerInventory.Instance.gunLevel;
                float damage = PlayerInventory.Instance.GunDamage;
                float fireRate = PlayerInventory.Instance.GunFireRate;
                gunLevelText.text = $"레벨: {level}";
                gunStatsText.text = $"데미지: {damage:F1}\n연사력: {fireRate:F1}s";
            }
        }
    }
    private void UpdateInventoryInfo()
    {
        if (Warehouse.Instance != null)
        {
            if (milkCountText != null)
                milkCountText.text = $"{Warehouse.Instance.GetMilkCount()}개";
            if (eggCountText != null)
                eggCountText.text = $"{Warehouse.Instance.GetEggCount()}개";
            if (avgFreshnessText != null)
                avgFreshnessText.text = $"{Warehouse.Instance.GetAverageMilkFreshness():F2}%";
        }
        else
        {
            if (milkCountText != null) milkCountText.text = "0개";
            if (eggCountText != null) eggCountText.text = "0개";
            if (avgFreshnessText != null) avgFreshnessText.text = "0.00%";
        }

        if (GameManager.Instance != null)
        {
            if (dailyMilkText != null)
                dailyMilkText.text = $"{GameManager.Instance.dailyMilkProduced}개";
            if (dailyEggText != null)
                dailyEggText.text = $"{GameManager.Instance.dailyEggsProduced}개";
        }
    }
}