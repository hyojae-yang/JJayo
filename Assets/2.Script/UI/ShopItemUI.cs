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

    private TextMeshProUGUI _actionButtonText;

    private ShopUI _shopUI;
    private PurchasableItemData _currentItemData;
    private Animal _animalToSell;

    private void Awake()
    {
        _actionButtonText = actionButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetupBuyItem(ShopUI shopUI, PurchasableItemData itemData)
    {
        this._shopUI = shopUI;
        _currentItemData = itemData;

        itemNameText.text = itemData.itemName;
        itemIcon.sprite = itemData.itemIcon;

        if (_actionButtonText != null)
        {
            _actionButtonText.text = "구매";
        }

        if (itemData.itemType == ItemType.Upgrade)
        {
            // GetCurrentUpgradeLevel() 메서드에서 올바른 레벨을 가져오도록 수정되었습니다.
            int currentLevel = GetCurrentUpgradeLevel();
            int maxLevel = itemData.upgradeData.GetMaxLevel();

            if (currentLevel >= maxLevel)
            {
                itemPriceText.text = "최대 레벨";
                if (_actionButtonText != null) _actionButtonText.text = "최대";
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
            bool isOwned = false;
            if (_shopUI.shopService != null && _shopUI.shopService.gameManager != null && _shopUI.shopService.gameManager.CurrentGameData != null)
            {
                isOwned = _shopUI.shopService.gameManager.CurrentGameData.IsBuildingOwned(itemData.buildingData.buildingId);
            }
            else
            {
                isOwned = false;
            }

            if (isOwned)
            {
                itemPriceText.text = "보유중";
                if (_actionButtonText != null) _actionButtonText.text = "보유중";
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

    public void SetupSellItem(ShopUI shopUI, Animal animalToSell)
    {
        this._shopUI = shopUI;
        _animalToSell = animalToSell;

        itemNameText.text = animalToSell.animalData.animalName;
        itemPriceText.text = (animalToSell.animalData.animalPrice / 2).ToString("C0");
        itemIcon.sprite = animalToSell.animalData.animalIcon;

        if (_actionButtonText != null)
        {
            _actionButtonText.text = "판매";
        }

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => _shopUI.ShowConfirmationPanelForSell(_animalToSell));
    }

    public void SetupSellChicken(ShopUI shopUI, int price, Sprite chickenIcon)
    {
        this._shopUI = shopUI;

        if (_actionButtonText != null)
        {
            _actionButtonText.text = "판매";
        }

        itemPriceText.text = price.ToString("C0");
        itemIcon.sprite = chickenIcon;
        itemNameText.text = "닭 판매";

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => _shopUI.ShowConfirmationPanelForSellChicken(price));
    }

    private int GetCurrentUpgradeLevel()
    {
        if (_shopUI == null || _shopUI.shopService == null || _shopUI.shopService.gameManager == null || _shopUI.shopService.gameManager.CurrentGameData == null)
        {
            return 0;
        }

        // ★★★ 이 부분에 창고 업그레이드 로직이 추가되었습니다. ★★★
        if (_currentItemData.upgradeData is BasketUpgradeData) return _shopUI.shopService.gameManager.CurrentGameData.basketLevel;
        if (_currentItemData.upgradeData is MilkerUpgradeData) return _shopUI.shopService.gameManager.CurrentGameData.milkerLevel;
        if (_currentItemData.upgradeData is GunUpgradeData) return _shopUI.shopService.gameManager.CurrentGameData.gunLevel;
        if (_currentItemData.upgradeData is PastureUpgradeData) return _shopUI.shopService.gameManager.CurrentGameData.pastureLevel;
        if (_currentItemData.upgradeData is WarehouseUpgradeData) return _shopUI.shopService.gameManager.CurrentGameData.warehouseLevel;

        return 0;
    }
}