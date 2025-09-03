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

    // 수정된 부분: actionButton의 텍스트를 제어할 변수 추가
    private TextMeshProUGUI _actionButtonText;

    private ShopUI _shopUI;
    private PurchasableItemData _currentItemData;
    private Animal _animalToSell;

    private void Awake()
    {
        // Awake 메서드에서 actionButton의 자식으로 있는 TextMeshProUGUI 컴포넌트를 찾습니다.
        _actionButtonText = actionButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetupBuyItem(ShopUI shopUI, PurchasableItemData itemData)
    {
        this._shopUI = shopUI;
        _currentItemData = itemData;

        itemNameText.text = itemData.itemName;
        itemIcon.sprite = itemData.itemIcon;

        // 수정된 부분: 버튼 텍스트를 "구매"로 변경
        if (_actionButtonText != null)
        {
            _actionButtonText.text = "구매";
        }

        if (itemData.itemType == ItemType.Upgrade)
        {
            int currentLevel = GetCurrentUpgradeLevel();
            int maxLevel = itemData.upgradeData.GetMaxLevel();

            if (currentLevel >= maxLevel)
            {
                itemPriceText.text = "최대 레벨";
                if (_actionButtonText != null) _actionButtonText.text = "최대"; // 버튼 텍스트도 변경
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
                if (_actionButtonText != null) _actionButtonText.text = "보유중"; // 버튼 텍스트도 변경
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

        // 수정된 부분: 버튼 텍스트를 "판매"로 변경
        if (_actionButtonText != null)
        {
            _actionButtonText.text = "판매";
        }

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => _shopUI.ShowConfirmationPanelForSell(_animalToSell));
    }

    public void SetupSellChicken(ShopUI shopUI, int price)
    {
        this._shopUI = shopUI;

        // 수정된 부분: 닭 판매 버튼 텍스트를 "판매"로 변경
        if (_actionButtonText != null)
        {
            _actionButtonText.text = "판매";
        }

        itemPriceText.text = price.ToString("C0");
        itemIcon.sprite = null; // 닭 아이콘이 없으므로 null로 설정
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

        if (_currentItemData.upgradeData is BasketUpgradeData) return _shopUI.shopService.gameManager.CurrentGameData.basketLevel;
        if (_currentItemData.upgradeData is MilkerUpgradeData) return _shopUI.shopService.gameManager.CurrentGameData.milkerLevel;
        if (_currentItemData.upgradeData is GunUpgradeData) return _shopUI.shopService.gameManager.CurrentGameData.gunLevel;
        if (_currentItemData.upgradeData is PastureUpgradeData) return _shopUI.shopService.gameManager.CurrentGameData.pastureLevel;
        return 0;
    }
}