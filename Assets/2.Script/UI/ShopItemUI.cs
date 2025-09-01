using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public Image itemIcon;
    public Button actionButton;

    // 더 이상 private가 아닌 public 또는 protected로 변경
    private ShopUI _shopUI;
    private PurchasableItemData _currentItemData;
    private Animal _animalToSell;

    // ShopUI 인스턴스를 직접 받도록 수정
    public void SetupBuyItem(ShopUI shopUI, PurchasableItemData itemData)
    {
        // 인자로 받은 shopUI를 사용합니다.
        this._shopUI = shopUI;
        _currentItemData = itemData;

        itemNameText.text = itemData.itemName;
        itemIcon.sprite = itemData.itemIcon;

        if (itemData.itemType == ItemType.Upgrade)
        {
            int currentLevel = GetCurrentUpgradeLevel();
            int maxLevel = itemData.upgradeData.GetMaxLevel();

            if (currentLevel >= maxLevel)
            {
                itemPriceText.text = "최대 레벨";
                actionButton.interactable = false;
            }
            else
            {
                int nextPrice = itemData.upgradeData.GetUpgradePrice(currentLevel);
                itemPriceText.text = nextPrice.ToString("C0");
                actionButton.interactable = true;
            }
        }
        else if (itemData.itemType == ItemType.Building)
        {
            bool isOwned = _shopUI.shopService.gameManager.CurrentGameData.IsBuildingOwned(itemData.buildingData.buildingId);

            if (isOwned)
            {
                itemPriceText.text = "보유중";
                actionButton.interactable = false;
            }
            else
            {
                itemPriceText.text = itemData.buildingData.buildingPrice.ToString("C0");
                actionButton.interactable = true;
            }
        }
        else
        {
            itemPriceText.text = itemData.itemPrice.ToString("C0");
            actionButton.interactable = true;
        }

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => _shopUI.ShowConfirmationPanelForBuy(_currentItemData));
    }

    // ShopUI 인스턴스를 직접 받도록 수정
    public void SetupSellItem(ShopUI shopUI, Animal animalToSell)
    {
        // 인자로 받은 shopUI를 사용합니다.
        this._shopUI = shopUI;
        _animalToSell = animalToSell;

        itemNameText.text = animalToSell.animalData.animalName;
        itemPriceText.text = (animalToSell.animalData.animalPrice / 2).ToString("C0");
        itemIcon.sprite = animalToSell.animalData.animalIcon;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => _shopUI.ShowConfirmationPanelForSell(_animalToSell));
    }

    // ShopUI 인스턴스를 직접 받도록 수정
    public void SetupSellChicken(ShopUI shopUI, int price)
    {
        // 인자로 받은 shopUI를 사용합니다.
        this._shopUI = shopUI;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => _shopUI.ShowConfirmationPanelForSellChicken(price));
    }

    private int GetCurrentUpgradeLevel()
    {
        if (_currentItemData.upgradeData is BasketUpgradeData) return _shopUI.shopService.gameManager.CurrentGameData.basketLevel;
        if (_currentItemData.upgradeData is MilkerUpgradeData) return _shopUI.shopService.gameManager.CurrentGameData.milkerLevel;
        if (_currentItemData.upgradeData is GunUpgradeData) return _shopUI.shopService.gameManager.CurrentGameData.gunLevel;
        if (_currentItemData.upgradeData is PastureUpgradeData) return _shopUI.shopService.gameManager.CurrentGameData.pastureLevel;
        return 0;
    }
}