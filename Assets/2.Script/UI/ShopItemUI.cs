// ShopItemUI.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

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

        bool isInteractable = true; // 기본값은 활성화

        if (itemData.itemType == ItemType.Upgrade)
        {
            int currentLevel = GetCurrentUpgradeLevel();
            int maxLevel = itemData.upgradeData.GetMaxLevel();

            if (currentLevel >= maxLevel)
            {
                itemPriceText.text = "최대 레벨";
                if (_actionButtonText != null) _actionButtonText.text = "최대";
                isInteractable = false;
            }
            else
            {
                int nextPrice = itemData.upgradeData.GetUpgradePrice(currentLevel);
                itemPriceText.text = nextPrice.ToString("C0");
            }
        }
        else if (itemData.itemType == ItemType.Building)
        {
            // 닭장 건물 ID는 프로젝트 내에서 "ChickenCoop" 같은 고유한 값으로 설정되어 있어야 합니다.
            if (itemData.buildingData.buildingId == "ChickenCoop")
            {
                bool isOwned = BuildingManager.Instance.IsBuildingOwned(itemData.buildingData.buildingId);
                if (isOwned)
                {
                    itemPriceText.text = "보유중";
                    if (_actionButtonText != null) _actionButtonText.text = "보유중";
                    isInteractable = false;
                }
                else
                {
                    itemPriceText.text = itemData.buildingData.buildingPrice.ToString("C0");
                }
            }
            else
            {
                // 다른 건물은 GameData에서 확인
                bool isOwned = false;
                if (_shopUI.shopService != null && _shopUI.shopService.gameManager != null && _shopUI.shopService.gameManager.CurrentGameData != null)
                {
                    isOwned = _shopUI.shopService.gameManager.CurrentGameData.IsBuildingOwned(itemData.buildingData.buildingId);
                }

                if (isOwned)
                {
                    itemPriceText.text = "보유중";
                    if (_actionButtonText != null) _actionButtonText.text = "보유중";
                    isInteractable = false;
                }
                else
                {
                    itemPriceText.text = itemData.buildingData.buildingPrice.ToString("C0");
                }
            }
        }
        else if (itemData.itemType == ItemType.Animal)
        {
            // 닭(Chicken)일 경우 닭장 유무를 확인
            if (itemData.animalData.animalType == AnimalType.Chicken)
            {
                if (BuildingManager.Instance != null)
                {
                    bool hasChickenCoop = BuildingManager.Instance.IsBuildingOwned("ChickenCoop"); // 닭장 ID로 확인
                    isInteractable = hasChickenCoop;
                    if (!hasChickenCoop)
                    {
                        itemPriceText.text = "닭장 필요";
                    }
                    else
                    {
                        itemPriceText.text = itemData.itemPrice.ToString("C0");
                    }
                }
                else
                {
                    isInteractable = false;
                    itemPriceText.text = "관리자 없음";
                }
            }
            else
            {
                // 젖소(Cow)일 경우 기존 로직 유지
                itemPriceText.text = itemData.itemPrice.ToString("C0");
            }
        }
        else
        {
            itemPriceText.text = itemData.itemPrice.ToString("C0");
        }

        actionButton.interactable = isInteractable;
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySFX(SFXType.Button_Click);
            _shopUI.ShowConfirmationPanelForBuy(_currentItemData);
        });
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
        actionButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySFX(SFXType.Button_Click);
            _shopUI.ShowConfirmationPanelForSell(_animalToSell);
        });
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
        actionButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySFX(SFXType.Button_Click);
            _shopUI.ShowConfirmationPanelForSellChicken(price);
        });
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
        if (_currentItemData.upgradeData is WarehouseUpgradeData) return _shopUI.shopService.gameManager.CurrentGameData.warehouseLevel;

        return 0;
    }
}