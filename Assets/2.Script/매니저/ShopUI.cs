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

    private void Awake()
    {
        // ★★★ 변경된 부분: DontDestroyOnLoad 코드를 제거하고 씬에 종속적인 싱글톤 패턴으로 변경 ★★★
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // 이미 존재하는 인스턴스가 있다면, 새로 생성된 자신을 파괴합니다.
            Destroy(gameObject);
        }
    }

    void Start()
    {
        shopPanel.SetActive(false);
        buyPanel.SetActive(true);
        sellPanel.SetActive(false);
        confirmationPanel.SetActive(false);

        // 확인 및 취소 버튼에 이벤트 리스너를 미리 추가하여 중복을 방지합니다.
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
                itemUI.itemNameText.text = chickenSellData.animalData.animalName + $" (현재 {chickenCoop.numberOfChickens}마리)";
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
        confirmText.text = $"{itemData.itemName}을(를) {itemData.itemPrice}원에 구매하시겠습니까?";
    }

    public void OnClickSell(Animal animalToSell)
    {
        this.animalToSell = animalToSell;
        itemToPurchase = null;
        confirmationPanel.SetActive(true);
        int sellPrice = animalToSell.animalData.animalPrice / 2;
        confirmText.text = $"{animalToSell.animalData.animalName}을(를) {sellPrice}원에 판매하시겠습니까?";
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
            confirmText.text = $"닭 1마리를 {sellPrice}원에 판매하시겠습니까?";
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