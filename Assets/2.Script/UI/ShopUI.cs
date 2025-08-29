using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }

    [Header("패널 UI")]
    public GameObject shopPanel;
    public GameObject buyPanel;
    public GameObject sellPanel;
    public GameObject confirmationPanel;

    [Header("콘텐츠 UI")]
    public Transform buyContentPanel;
    public Transform sellContentPanel;

    [Header("데이터 및 프리팹")]
    public GameObject uiItemCardPrefab;

    [Header("알림창 UI 요소")]
    public TextMeshProUGUI confirmText;
    public Button confirmButton;
    public Button cancelButton;

    private PurchasableItemData itemToPurchase;
    private Animal animalToSell;
    private bool isSellingChicken = false;

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
        shopPanel.SetActive(false);
        buyPanel.SetActive(true);
        sellPanel.SetActive(false);
        confirmationPanel.SetActive(false);

        confirmButton.onClick.AddListener(OnClickConfirm);
        cancelButton.onClick.AddListener(OnClickCancel);

        ShowBuyPanel();
    }

    public void ShowShop()
    {
        shopPanel.SetActive(true);
        ShowBuyPanel();
        Time.timeScale = 0f;
    }

    public void HideShop()
    {
        shopPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ShowBuyPanel()
    {
        buyPanel.SetActive(true);
        sellPanel.SetActive(false);
        PopulateBuyItems();
    }

    public void ShowSellPanel()
    {
        buyPanel.SetActive(false);
        sellPanel.SetActive(true);
        PopulateSellItems();
    }

    private void PopulateBuyItems()
    {
        // UI 패널을 초기화합니다.
        foreach (Transform child in buyContentPanel)
        {
            Destroy(child.gameObject);
        }

        if (ShopService.Instance == null) return;

        // ShopService에서 이미 구매한 장비 아이템을 제외한 리스트를 가져와서 반복문을 돌립니다.
        // 장비 아이템은 이 리스트에 포함되지 않으므로, 카드가 생성되지 않습니다.
        foreach (var data in ShopService.Instance.GetShopItems())
        {
            GameObject itemCard = Instantiate(uiItemCardPrefab, buyContentPanel);
            ShopItemUI itemUI = itemCard.GetComponent<ShopItemUI>();
            itemUI.SetupBuyItem(data);
        }
    }

    private void PopulateSellItems()
    {
        foreach (Transform child in sellContentPanel)
        {
            Destroy(child.gameObject);
        }

        Animal[] allCows = FindObjectsByType<Animal>(FindObjectsSortMode.None);
        foreach (Animal animal in allCows)
        {
            if (animal.animalData.animalType == AnimalType.Cow)
            {
                GameObject itemCard = Instantiate(uiItemCardPrefab, sellContentPanel);
                ShopItemUI itemUI = itemCard.GetComponent<ShopItemUI>();
                itemUI.SetupSellItem(animal);
            }
        }

        ChickenCoop chickenCoop = FindFirstObjectByType<ChickenCoop>();
        if (chickenCoop != null && chickenCoop.numberOfChickens > 0)
        {
            int sellPrice = ShopService.Instance.GetChickenSellPrice();

            GameObject itemCard = Instantiate(uiItemCardPrefab, sellContentPanel);
            ShopItemUI itemUI = itemCard.GetComponent<ShopItemUI>();

            var chickenData = ShopService.Instance.GetShopItems().FirstOrDefault(item => item.animalData != null && item.animalData.animalType == AnimalType.Chicken);
            if (chickenData != null)
            {
                itemUI.itemNameText.text = chickenData.animalData.animalName + $" (현재 {chickenCoop.numberOfChickens}마리)";
                itemUI.itemPriceText.text = sellPrice.ToString("C0");
                itemUI.itemIcon.sprite = chickenData.animalData.animalIcon;
            }
            itemUI.SetupSellChicken(sellPrice);
        }
    }

    public void OnClickConfirm()
    {
        if (itemToPurchase != null)
        {
            int finalPrice = 0;
            if (itemToPurchase.itemType == ItemType.Upgrade)
            {
                int currentLevel = GetCurrentUpgradeLevelForConfirmation(itemToPurchase.upgradeData);
                finalPrice = itemToPurchase.upgradeData.GetUpgradePrice(currentLevel);
            }
            else
            {
                finalPrice = itemToPurchase.itemPrice;
            }

            if (ShopService.Instance.CanBuy(itemToPurchase) && GameManager.Instance.gameData.money >= finalPrice)
            {
                ShopService.Instance.PurchaseItem(itemToPurchase);
                NotificationManager.Instance.ShowNotification(itemToPurchase.itemName + "을(를) 구매했습니다!");
            }
            else
            {
                NotificationManager.Instance.ShowNotification("돈이 부족하거나 이미 소유한 아이템입니다!");
            }
        }
        else if (isSellingChicken)
        {
            ShopService.Instance.SellChicken();
            GameManager.Instance.UpdateUI();
        }
        else if (animalToSell != null)
        {
            ShopService.Instance.SellItem(animalToSell);
            GameManager.Instance.UpdateUI();
        }

        confirmationPanel.SetActive(false);
    }

    public void OnClickCancel()
    {
        itemToPurchase = null;
        animalToSell = null;
        isSellingChicken = false;
        confirmationPanel.SetActive(false);
    }

    private int GetCurrentUpgradeLevelForConfirmation(UpgradeData upgradeData)
    {
        if (upgradeData is BasketUpgradeData) return GameManager.Instance.gameData.basketLevel;
        if (upgradeData is MilkerUpgradeData) return GameManager.Instance.gameData.milkerLevel;
        if (upgradeData is GunUpgradeData) return GameManager.Instance.gameData.gunLevel;
        if (upgradeData is PastureUpgradeData) return GameManager.Instance.gameData.pastureLevel;
        return 0;
    }

    public void ShowConfirmationPanelForBuy(PurchasableItemData itemData)
    {
        itemToPurchase = itemData;
        animalToSell = null;
        isSellingChicken = false;
        confirmationPanel.SetActive(true);

        int priceForConfirmation = 0;
        if (itemData.itemType == ItemType.Upgrade)
        {
            int currentLevel = GetCurrentUpgradeLevelForConfirmation(itemData.upgradeData);
            priceForConfirmation = itemData.upgradeData.GetUpgradePrice(currentLevel);
        }
        else
        {
            priceForConfirmation = itemData.itemPrice;
        }

        confirmText.text = $"{itemData.itemName}을(를) {priceForConfirmation}원에 구매하시겠습니까?";
    }

    public void ShowConfirmationPanelForSell(Animal animalToSell)
    {
        this.animalToSell = animalToSell;
        itemToPurchase = null;
        isSellingChicken = false;
        confirmationPanel.SetActive(true);
        int sellPrice = animalToSell.animalData.animalPrice / 2;
        confirmText.text = $"{animalToSell.animalData.animalName}을(를) {sellPrice}원에 판매하시겠습니까?";
    }

    public void ShowConfirmationPanelForSellChicken(int price)
    {
        itemToPurchase = null;
        animalToSell = null;
        isSellingChicken = true;
        confirmationPanel.SetActive(true);
        confirmText.text = $"닭 1마리를 {price}원에 판매하시겠습니까?";
    }

    public void RefreshShopItems()
    {
        if (buyPanel.activeSelf)
        {
            PopulateBuyItems();
        }
        else if (sellPanel.activeSelf)
        {
            PopulateSellItems();
        }
    }
}