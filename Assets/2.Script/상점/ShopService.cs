using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ShopService : MonoBehaviour
{
    public static ShopService Instance { get; private set; }
    public AnimalHandler animalHandler;
    public BuildingHandler buildingHandler;
    public EquipmentHandler equipmentHandler;
    public UpgradeHandler upgradeHandler;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (animalHandler == null) Debug.LogError("AnimalHandler가 할당되지 않았습니다.");
        if (buildingHandler == null) Debug.LogError("BuildingHandler가 할당되지 않았습니다.");
        if (equipmentHandler == null) Debug.LogError("EquipmentHandler가 할당되지 않았습니다.");
        if (upgradeHandler == null) Debug.LogError("UpgradeHandler가 할당되지 않았습니다.");
    }

    // 상점에서 표시할 아이템 목록을 가져옵니다.
    // 이제 구매한 장비 아이템을 제외합니다.
    public List<PurchasableItemData> GetShopItems()
    {
        if (ShopManager.Instance != null)
        {
            // ShopManager에서 모든 아이템을 가져옵니다.
            var allItems = ShopManager.Instance.GetShopItems();

            // GameManager가 로드되지 않았을 경우, 전체 아이템 목록을 반환합니다.
            if (GameManager.Instance == null || GameManager.Instance.gameData == null)
            {
                return allItems;
            }

            // 구매한 장비 아이템을 제외한 새로운 리스트를 생성하여 반환합니다.
            var availableItems = allItems.Where(item =>
            {
                // 아이템 타입이 장비일 경우,
                if (item.itemType == ItemType.Equipment)
                {
                    // 해당 장비의 ID가 이미 구매한 리스트에 포함되어 있는지 확인합니다.
                    // 포함되어 있지 않은 경우에만 true를 반환하여 목록에 남깁니다.
                    return !GameManager.Instance.gameData.ownedEquipmentIds.Contains(item.equipmentData.id);
                }
                // 다른 아이템 타입은 그대로 유지합니다.
                return true;
            }).ToList();

            return availableItems;
        }

        Debug.LogError("ShopManager를 찾을 수 없어 빈 리스트를 반환합니다.");
        return new List<PurchasableItemData>();
    }

    public bool CanBuy(PurchasableItemData itemData)
    {
        GameData gameData = GameManager.Instance.gameData;
        if (gameData == null) return false;

        switch (itemData.itemType)
        {
            case ItemType.Building: return buildingHandler.CanBuy();
            case ItemType.Animal: return animalHandler.CanBuy(itemData.animalData);
            case ItemType.Equipment: return equipmentHandler.CanBuy(itemData.equipmentData);
            case ItemType.Upgrade: return upgradeHandler.CanBuy(itemData.upgradeData);
            case ItemType.Consumable:
                if (itemData.consumableData != null && itemData.itemName == "총알(30개)")
                {
                    // 총알은 이제 GameData의 hasGun 변수 대신 ownedEquipmentIds 리스트를 확인합니다.
                    return gameData.ownedEquipmentIds.Contains("Gun");
                }
                break;
        }
        return true;
    }

    public void PurchaseItem(PurchasableItemData itemToPurchase)
    {
        GameData gameData = GameManager.Instance.gameData;
        if (gameData == null) return;

        int finalPrice = 0;
        if (itemToPurchase.itemType == ItemType.Upgrade)
        {
            int currentLevel = GetCurrentUpgradeLevel(itemToPurchase.upgradeData);
            finalPrice = itemToPurchase.upgradeData.GetUpgradePrice(currentLevel);
        }
        else if (itemToPurchase.itemType == ItemType.Building)
        {
            finalPrice = itemToPurchase.buildingData.buildingPrice;
        }
        else
        {
            finalPrice = itemToPurchase.itemPrice;
        }

        gameData.money -= finalPrice;

        switch (itemToPurchase.itemType)
        {
            case ItemType.Animal:
                animalHandler.Purchase(itemToPurchase.animalData);
                NotificationManager.Instance.ShowNotification(itemToPurchase.itemName + "을(를) 구매했습니다!");
                break;
            case ItemType.Building:
                buildingHandler.Purchase(itemToPurchase.buildingData);
                break;
            case ItemType.Equipment:
                equipmentHandler.Purchase(itemToPurchase.equipmentData);
                break;
            case ItemType.Upgrade:
                upgradeHandler.Purchase(itemToPurchase.upgradeData);
                break;
            case ItemType.Consumable:
                if (itemToPurchase.itemName == "총알(30개)")
                {
                    gameData.bulletCount += itemToPurchase.consumableData.amount;
                    NotificationManager.Instance.ShowNotification(itemToPurchase.itemName + "을(를) 구매했습니다.");
                }
                break;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateUI();
        }

        if (itemToPurchase.itemType == ItemType.Upgrade || itemToPurchase.itemType == ItemType.Building)
        {
            ShopUI.Instance.RefreshShopItems();
        }
    }

    private int GetCurrentUpgradeLevel(UpgradeData upgradeData)
    {
        if (upgradeData is BasketUpgradeData) return GameManager.Instance.gameData.basketLevel;
        if (upgradeData is MilkerUpgradeData) return GameManager.Instance.gameData.milkerLevel;
        if (upgradeData is GunUpgradeData) return GameManager.Instance.gameData.gunLevel;
        if (upgradeData is PastureUpgradeData) return GameManager.Instance.gameData.pastureLevel;
        return 0;
    }

    public void SellItem(Animal animalToSell)
    {
        GameData gameData = GameManager.Instance.gameData;
        if (gameData == null) return;
        int sellPrice = animalToSell.animalData.animalPrice / 2;
        animalHandler.Sell(animalToSell, sellPrice);
    }

    public void SellChicken()
    {
        if (animalHandler != null && animalHandler.CanSellChicken())
        {
            GameManager.Instance.gameData.money += GetChickenSellPrice();
            animalHandler.RemoveChicken();
        }
        else
        {
            NotificationManager.Instance.ShowNotification("판매할 닭이 없습니다.");
        }
    }

    public int GetChickenSellPrice()
    {
        var chickenSellData = GetShopItems().FirstOrDefault(item => item.animalData != null && item.animalData.animalType == AnimalType.Chicken);
        return chickenSellData != null ? chickenSellData.animalData.animalPrice / 2 : 0;
    }
}