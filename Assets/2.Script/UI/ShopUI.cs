using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }

    [Header("Dependencies - 인스펙터로 연결")]
    public ShopService shopService;

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

    void Awake()
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

    private void OnEnable()
    {
        if (shopService == null)
        {
            shopService = ShopService.Instance;
        }
    }

    public void Initialize()
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
        SoundManager.Instance.PlaySFX(SFXType.Button_Click);
        if (shopService == null)
        {
            shopService = ShopService.Instance;
        }
        shopPanel.SetActive(true);
        GameManager.Instance.IsMenuOn = true;
        ShowBuyPanel();
        Time.timeScale = 0f;
    }

    public void HideShop()
    {
        SoundManager.Instance.PlaySFX(SFXType.Button_Click);
        shopPanel.SetActive(false);
        GameManager.Instance.IsMenuOn = false;
        Time.timeScale = 1f;
    }

    public void ShowBuyPanel()
    {
        SoundManager.Instance.PlaySFX(SFXType.Button_Click);
        buyPanel.SetActive(true);
        sellPanel.SetActive(false);
        PopulateBuyItems();
    }

    public void ShowSellPanel()
    {
        SoundManager.Instance.PlaySFX(SFXType.Button_Click);
        buyPanel.SetActive(false);
        sellPanel.SetActive(true);
        PopulateSellItems();
    }

    private void PopulateBuyItems()
    {
        foreach (Transform child in buyContentPanel)
        {
            Destroy(child.gameObject);
        }

        if (shopService == null)
        {
            Debug.LogError("ShopService가 할당되지 않았습니다. 인스펙터에서 연결해주세요.");
            return;
        }

        foreach (var data in shopService.GetShopItems())
        {
            GameObject itemCard = Instantiate(uiItemCardPrefab, buyContentPanel);
            ShopItemUI itemUI = itemCard.GetComponent<ShopItemUI>();
            itemUI.SetupBuyItem(this, data);
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
                itemUI.SetupSellItem(this, animal);
            }
        }

        if (GameManager.Instance != null && GameManager.Instance.CurrentGameData != null && GameManager.Instance.CurrentGameData.chickenCount > 0)
        {
            int sellPrice = shopService.GetChickenSellPrice();

            var chickenData = shopService.GetShopItems().FirstOrDefault(item => item.animalData != null && item.animalData.animalType == AnimalType.Chicken);

            if (chickenData != null)
            {
                GameObject itemCard = Instantiate(uiItemCardPrefab, sellContentPanel);
                ShopItemUI itemUI = itemCard.GetComponent<ShopItemUI>();

                itemUI.SetupSellChicken(this, sellPrice, chickenData.itemIcon);
            }
        }
    }

    public void OnClickConfirm()
    {
        SoundManager.Instance.PlaySFX(SFXType.Button_Click);
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

            if (shopService.CanBuy(itemToPurchase))
            {
                shopService.PurchaseItem(itemToPurchase);
                // ★★★ 구매 성공 효과음 추가 ★★★
                SoundManager.Instance.PlaySFX(SFXType.Item_Purchase);
            }
            else
            {
                NotificationManager.Instance.ShowNotification("돈이 부족하거나 이미 소유한 아이템입니다!");
            }
        }
        else if (isSellingChicken)
        {
            shopService.SellChicken();
            // ★★★ 판매 성공 효과음 추가 ★★★
            SoundManager.Instance.PlaySFX(SFXType.Item_Sell);
        }
        else if (animalToSell != null)
        {
            shopService.SellItem(animalToSell);
            // ★★★ 판매 성공 효과음 추가 ★★★
            SoundManager.Instance.PlaySFX(SFXType.Item_Sell);
        }

        RefreshShopItems();
        confirmationPanel.SetActive(false);
    }

    public void OnClickCancel()
    {
        SoundManager.Instance.PlaySFX(SFXType.Button_Click);
        itemToPurchase = null;
        animalToSell = null;
        isSellingChicken = false;
        confirmationPanel.SetActive(false);
    }

    private int GetCurrentUpgradeLevelForConfirmation(UpgradeData upgradeData)
    {
        if (shopService.gameManager.CurrentGameData == null) return 0;
        if (upgradeData is BasketUpgradeData) return shopService.gameManager.CurrentGameData.basketLevel;
        if (upgradeData is MilkerUpgradeData) return shopService.gameManager.CurrentGameData.milkerLevel;
        if (upgradeData is GunUpgradeData) return shopService.gameManager.CurrentGameData.gunLevel;
        if (upgradeData is PastureUpgradeData) return shopService.gameManager.CurrentGameData.pastureLevel;
        if (upgradeData is WarehouseUpgradeData) return shopService.gameManager.CurrentGameData.warehouseLevel; // ★★★ 새로 추가된 부분 ★★★
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