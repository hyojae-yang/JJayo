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
        foreach (Transform child in buyContentPanel)
        {
            Destroy(child.gameObject);
        }

        if (ShopService.Instance == null) return;

        foreach (var data in ShopService.Instance.GetShopItems())
        {
            if (ShopService.Instance.CanBuy(data))
            {
                GameObject itemCard = Instantiate(uiItemCardPrefab, buyContentPanel);
                ShopItemUI itemUI = itemCard.GetComponent<ShopItemUI>();
                itemUI.SetupBuyItem(this, data);
            }
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

        ChickenCoop chickenCoop = FindAnyObjectByType<ChickenCoop>();
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

            itemUI.actionButton.onClick.RemoveAllListeners();
            itemUI.actionButton.onClick.AddListener(() => OnClickSellChicken(sellPrice));
        }
    }

    public void OnClickBuy(PurchasableItemData itemData)
    {
        itemToPurchase = itemData;
        animalToSell = null;
        isSellingChicken = false;
        confirmationPanel.SetActive(true);
        confirmText.text = $"{itemData.itemName}을(를) {itemData.itemPrice}원에 구매하시겠습니까?";
    }

    public void OnClickSell(Animal animalToSell)
    {
        this.animalToSell = animalToSell;
        itemToPurchase = null;
        isSellingChicken = false;
        confirmationPanel.SetActive(true);
        int sellPrice = animalToSell.animalData.animalPrice / 2;
        confirmText.text = $"{animalToSell.animalData.animalName}을(를) {sellPrice}원에 판매하시겠습니까?";
    }

    public void OnClickSellChicken(int price)
    {
        itemToPurchase = null;
        animalToSell = null;
        isSellingChicken = true;
        confirmationPanel.SetActive(true);
        confirmText.text = $"닭 1마리를 {price}원에 판매하시겠습니까?";
    }

    public void OnClickConfirm()
    {
        if (itemToPurchase != null)
        {
            ShopService.Instance.PurchaseItem(itemToPurchase);
            PopulateBuyItems();
        }
        else if (isSellingChicken)
        {
            ShopService.Instance.SellChicken();
            PopulateSellItems();
        }
        else if (animalToSell != null)
        {
            ShopService.Instance.SellItem(animalToSell);
            PopulateSellItems();
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
}