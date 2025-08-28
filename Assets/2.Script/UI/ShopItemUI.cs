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

    private ShopUI shopUI;
    private PurchasableItemData _currentItemData;
    private Animal _animalToSell;

    public void SetupBuyItem(PurchasableItemData itemData)
    {
        shopUI = ShopUI.Instance;
        _currentItemData = itemData;

        itemNameText.text = itemData.itemName;
        itemIcon.sprite = itemData.itemIcon;

        // 아이템이 업그레이드 아이템일 경우
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
        // 아이템이 건물일 경우
        else if (itemData.itemType == ItemType.Building)
        {
            // GameManager를 통해 건물의 보유 여부를 확인합니다.
            bool isOwned = GameManager.Instance.gameData.IsBuildingOwned(itemData.buildingData.buildingId);

            if (isOwned)
            {
                itemPriceText.text = "보유중";
                actionButton.interactable = false;
            }
            else
            {
                // 가격 정보를 이제 buildingData에서 가져옵니다.
                itemPriceText.text = itemData.buildingData.buildingPrice.ToString("C0");
                actionButton.interactable = true;
            }
        }
        // 그 외 일반 아이템일 경우 (장비 아이템은 이제 이 로직에 포함됩니다.)
        else
        {
            itemPriceText.text = itemData.itemPrice.ToString("C0");
            actionButton.interactable = true;
        }

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => shopUI.ShowConfirmationPanelForBuy(_currentItemData));
    }

    public void SetupSellItem(Animal animalToSell)
    {
        shopUI = ShopUI.Instance;
        _animalToSell = animalToSell;

        itemNameText.text = animalToSell.animalData.animalName;
        itemPriceText.text = (animalToSell.animalData.animalPrice / 2).ToString("C0");
        itemIcon.sprite = animalToSell.animalData.animalIcon;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => shopUI.ShowConfirmationPanelForSell(_animalToSell));
    }

    public void SetupSellChicken(int price)
    {
        shopUI = ShopUI.Instance;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => shopUI.ShowConfirmationPanelForSellChicken(price));
    }

    private int GetCurrentUpgradeLevel()
    {
        if (_currentItemData.upgradeData is BasketUpgradeData) return GameManager.Instance.gameData.basketLevel;
        if (_currentItemData.upgradeData is MilkerUpgradeData) return GameManager.Instance.gameData.milkerLevel;
        if (_currentItemData.upgradeData is GunUpgradeData) return GameManager.Instance.gameData.gunLevel;
        if (_currentItemData.upgradeData is PastureUpgradeData) return GameManager.Instance.gameData.pastureLevel;
        return 0;
    }
}