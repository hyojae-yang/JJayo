using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }

    [Header("�г� UI")]
    public GameObject shopPanel;
    public GameObject buyPanel;
    public GameObject sellPanel;
    public GameObject confirmationPanel;

    [Header("������ UI")]
    public Transform buyContentPanel;
    public Transform sellContentPanel;

    [Header("������ �� ������")]
    public GameObject uiItemCardPrefab;

    [Header("�˸�â UI ���")]
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
        // UI �г��� �ʱ�ȭ�մϴ�.
        foreach (Transform child in buyContentPanel)
        {
            Destroy(child.gameObject);
        }

        if (ShopService.Instance == null) return;

        // ShopService���� �̹� ������ ��� �������� ������ ����Ʈ�� �����ͼ� �ݺ����� �����ϴ�.
        // ��� �������� �� ����Ʈ�� ���Ե��� �����Ƿ�, ī�尡 �������� �ʽ��ϴ�.
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
                itemUI.itemNameText.text = chickenData.animalData.animalName + $" (���� {chickenCoop.numberOfChickens}����)";
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
                NotificationManager.Instance.ShowNotification(itemToPurchase.itemName + "��(��) �����߽��ϴ�!");
            }
            else
            {
                NotificationManager.Instance.ShowNotification("���� �����ϰų� �̹� ������ �������Դϴ�!");
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

        confirmText.text = $"{itemData.itemName}��(��) {priceForConfirmation}���� �����Ͻðڽ��ϱ�?";
    }

    public void ShowConfirmationPanelForSell(Animal animalToSell)
    {
        this.animalToSell = animalToSell;
        itemToPurchase = null;
        isSellingChicken = false;
        confirmationPanel.SetActive(true);
        int sellPrice = animalToSell.animalData.animalPrice / 2;
        confirmText.text = $"{animalToSell.animalData.animalName}��(��) {sellPrice}���� �Ǹ��Ͻðڽ��ϱ�?";
    }

    public void ShowConfirmationPanelForSellChicken(int price)
    {
        itemToPurchase = null;
        animalToSell = null;
        isSellingChicken = true;
        confirmationPanel.SetActive(true);
        confirmText.text = $"�� 1������ {price}���� �Ǹ��Ͻðڽ��ϱ�?";
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