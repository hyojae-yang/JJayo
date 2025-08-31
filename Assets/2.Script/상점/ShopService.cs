using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ShopService : MonoBehaviour
{
    public static ShopService Instance { get; private set; }

    [Header("Dependencies - 인스펙터로 연결")]
    // 씬 전환 시 파괴되지 않는 매니저들은 여기에 인스펙터로 연결합니다.
    public AnimalHandler animalHandler;
    public BuildingHandler buildingHandler;
    public EquipmentHandler equipmentHandler;
    public UpgradeHandler upgradeHandler;
    public MoneyManager moneyManager;
    public ShopManager shopManager; // ShopManager도 DontDestroyOnLoad일 경우 여기에 연결합니다.
    public GameManager gameManager; // GameManager도 DontDestroyOnLoad이므로 여기에 연결합니다.
    public NotificationManager notificationManager; // 알림 매니저도 여기에 연결합니다.

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

        // Awake에서 Find를 사용하던 코드를 모두 제거합니다.
        // 모든 참조는 이제 인스펙터로 이루어집니다.
    }

    // 상점에서 표시할 아이템 목록을 가져옵니다.
    public List<PurchasableItemData> GetShopItems()
    {
        if (shopManager == null)
        {
            Debug.LogError("ShopManager가 할당되지 않아 빈 리스트를 반환합니다.");
            return new List<PurchasableItemData>();
        }

        var allItems = shopManager.GetShopItems();

        if (gameManager == null || gameManager.gameData == null)
        {
            return allItems;
        }

        var availableItems = allItems.Where(item =>
        {
            if (item.itemType == ItemType.Equipment)
            {
                return !gameManager.gameData.ownedEquipmentIds.Contains(item.equipmentData.id);
            }
            return true;
        }).ToList();

        return availableItems;
    }

    public bool CanBuy(PurchasableItemData itemData)
    {
        GameData gameData = gameManager.gameData;
        if (gameData == null || moneyManager == null) return false;

        int itemPrice = itemData.itemPrice;
        if (itemData.itemType == ItemType.Upgrade)
        {
            int currentLevel = GetCurrentUpgradeLevel(itemData.upgradeData);
            itemPrice = itemData.upgradeData.GetUpgradePrice(currentLevel);
        }
        else if (itemData.itemType == ItemType.Building)
        {
            itemPrice = itemData.buildingData.buildingPrice;
        }

        if (moneyManager.CurrentMoney < itemPrice) return false;

        switch (itemData.itemType)
        {
            case ItemType.Building: return buildingHandler.CanBuy();
            case ItemType.Animal: return animalHandler.CanBuy(itemData.animalData);
            case ItemType.Equipment: return equipmentHandler.CanBuy(itemData.equipmentData);
            case ItemType.Upgrade: return upgradeHandler.CanBuy(itemData.upgradeData);
            case ItemType.Consumable:
                if (itemData.consumableData != null && itemData.itemName == "총알(30개)")
                {
                    return gameData.ownedEquipmentIds.Contains("Gun");
                }
                break;
        }
        return true;
    }

    public void PurchaseItem(PurchasableItemData itemToPurchase)
    {
        GameData gameData = gameManager.gameData;
        if (gameData == null || moneyManager == null) return;

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

        if (!moneyManager.SpendMoney(finalPrice))
        {
            if (notificationManager != null) notificationManager.ShowNotification("돈이 부족합니다.");
            return;
        }

        switch (itemToPurchase.itemType)
        {
            case ItemType.Animal:
                animalHandler.Purchase(itemToPurchase.animalData);
                if (notificationManager != null) notificationManager.ShowNotification(itemToPurchase.itemName + "을(를) 구매했습니다!");
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
                    if (notificationManager != null) notificationManager.ShowNotification(itemToPurchase.itemName + "을(를) 구매했습니다.");
                }
                break;
        }

        // UI 갱신은 UI 스크립트에서 직접 하도록 수정
    }

    private int GetCurrentUpgradeLevel(UpgradeData upgradeData)
    {
        if (gameManager == null || gameManager.gameData == null) return 0;
        if (upgradeData is BasketUpgradeData) return gameManager.gameData.basketLevel;
        if (upgradeData is MilkerUpgradeData) return gameManager.gameData.milkerLevel;
        if (upgradeData is GunUpgradeData) return gameManager.gameData.gunLevel;
        if (upgradeData is PastureUpgradeData) return gameManager.gameData.pastureLevel;
        return 0;
    }

    public void SellItem(Animal animalToSell)
    {
        GameData gameData = gameManager.gameData;
        if (gameData == null || animalHandler == null) return;

        int sellPrice = animalToSell.animalData.animalPrice / 2;
        animalHandler.Sell(animalToSell, sellPrice);
    }

    public void SellChicken()
    {
        if (animalHandler != null && animalHandler.CanSellChicken())
        {
            moneyManager.AddMoney(GetChickenSellPrice());
            animalHandler.RemoveChicken();
        }
        else
        {
            if (notificationManager != null) notificationManager.ShowNotification("판매할 닭이 없습니다.");
        }
    }

    public int GetChickenSellPrice()
    {
        var chickenSellData = GetShopItems().FirstOrDefault(item => item.animalData != null && item.animalData.animalType == AnimalType.Chicken);
        return chickenSellData != null ? chickenSellData.animalData.animalPrice / 2 : 0;
    }
}