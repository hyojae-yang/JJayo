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
            // === ★★★ 수정된 부분: Building 타입에도 Null 체크 추가 ★★★ ===
            bool isOwned = false;
            if (_shopUI.shopService != null && _shopUI.shopService.gameManager != null && _shopUI.shopService.gameManager.CurrentGameData != null)
            {
                isOwned = _shopUI.shopService.gameManager.CurrentGameData.IsBuildingOwned(itemData.buildingData.buildingId);
            }
            else
            {
                // 데이터 로드 전이므로 일단 보유하지 않은 것으로 간주
                isOwned = false;
            }

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

    public void SetupSellItem(ShopUI shopUI, Animal animalToSell)
    {
        this._shopUI = shopUI;
        _animalToSell = animalToSell;

        itemNameText.text = animalToSell.animalData.animalName;
        itemPriceText.text = (animalToSell.animalData.animalPrice / 2).ToString("C0");
        itemIcon.sprite = animalToSell.animalData.animalIcon;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => _shopUI.ShowConfirmationPanelForSell(_animalToSell));
    }

    public void SetupSellChicken(ShopUI shopUI, int price)
    {
        this._shopUI = shopUI;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => _shopUI.ShowConfirmationPanelForSellChicken(price));
    }

    private int GetCurrentUpgradeLevel()
    {
        // === ★★★ 수정된 부분: Null 체크 추가 ★★★ ===
        if (_shopUI == null || _shopUI.shopService == null || _shopUI.shopService.gameManager == null || _shopUI.shopService.gameManager.CurrentGameData == null)
        {
            return 0; // 데이터가 아직 로드되지 않았으므로 0을 반환합니다.
        }

        if (_currentItemData.upgradeData is BasketUpgradeData) return _shopUI.shopService.gameManager.CurrentGameData.basketLevel;
        if (_currentItemData.upgradeData is MilkerUpgradeData) return _shopUI.shopService.gameManager.CurrentGameData.milkerLevel;
        if (_currentItemData.upgradeData is GunUpgradeData) return _shopUI.shopService.gameManager.CurrentGameData.gunLevel;
        if (_currentItemData.upgradeData is PastureUpgradeData) return _shopUI.shopService.gameManager.CurrentGameData.pastureLevel;
        return 0;
    }
}