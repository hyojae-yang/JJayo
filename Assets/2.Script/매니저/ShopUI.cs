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

    private void Awake()
    {
        // �ڡڡ� ����� �κ�: DontDestroyOnLoad �ڵ带 �����ϰ� ���� �������� �̱��� �������� ���� �ڡڡ�
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // �̹� �����ϴ� �ν��Ͻ��� �ִٸ�, ���� ������ �ڽ��� �ı��մϴ�.
            Destroy(gameObject);
        }
    }

    void Start()
    {
        shopPanel.SetActive(false);
        buyPanel.SetActive(true);
        sellPanel.SetActive(false);
        confirmationPanel.SetActive(false);

        // Ȯ�� �� ��� ��ư�� �̺�Ʈ �����ʸ� �̸� �߰��Ͽ� �ߺ��� �����մϴ�.
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

        foreach (var data in ShopService.Instance.shopItems)
        {
            if (data == null) continue;

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
            var chickenSellData = ShopService.Instance.shopItems.FirstOrDefault(item => item.animalData != null && item.animalData.animalType == AnimalType.Chicken);
            if (chickenSellData != null)
            {
                GameObject itemCard = Instantiate(uiItemCardPrefab, sellContentPanel);
                ShopItemUI itemUI = itemCard.GetComponent<ShopItemUI>();
                itemUI.itemNameText.text = chickenSellData.animalData.animalName + $" (���� {chickenCoop.numberOfChickens}����)";
                itemUI.itemPriceText.text = (chickenSellData.animalData.animalPrice / 2).ToString("C0");
                itemUI.itemIcon.sprite = chickenSellData.animalData.animalIcon;

                itemUI.actionButton.onClick.RemoveAllListeners();
                itemUI.actionButton.onClick.AddListener(() => OnClickSellChicken());
            }
        }
    }

    public void OnClickBuy(PurchasableItemData itemData)
    {
        itemToPurchase = itemData;
        animalToSell = null;
        confirmationPanel.SetActive(true);
        confirmText.text = $"{itemData.itemName}��(��) {itemData.itemPrice}���� �����Ͻðڽ��ϱ�?";
    }

    public void OnClickSell(Animal animalToSell)
    {
        this.animalToSell = animalToSell;
        itemToPurchase = null;
        confirmationPanel.SetActive(true);
        int sellPrice = animalToSell.animalData.animalPrice / 2;
        confirmText.text = $"{animalToSell.animalData.animalName}��(��) {sellPrice}���� �Ǹ��Ͻðڽ��ϱ�?";
    }

    public void OnClickSellChicken()
    {
        itemToPurchase = null;
        animalToSell = null;
        confirmationPanel.SetActive(true);
        var chickenData = ShopService.Instance.shopItems.FirstOrDefault(item => item.animalData != null && item.animalData.animalType == AnimalType.Chicken);
        if (chickenData != null)
        {
            int sellPrice = chickenData.animalData.animalPrice / 2;
            confirmText.text = $"�� 1������ {sellPrice}���� �Ǹ��Ͻðڽ��ϱ�?";
        }
    }

    public void OnClickConfirm()
    {
        if (itemToPurchase != null)
        {
            ShopService.Instance.PurchaseItem(itemToPurchase);
            PopulateBuyItems();
        }
        else if (animalToSell != null)
        {
            ShopService.Instance.SellItem(animalToSell);
            PopulateSellItems();
        }
        else
        {
            ShopService.Instance.SellChicken();
            PopulateSellItems();
        }

        confirmationPanel.SetActive(false);
    }

    public void OnClickCancel()
    {
        itemToPurchase = null;
        animalToSell = null;
        confirmationPanel.SetActive(false);
    }
}