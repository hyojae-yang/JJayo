using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ShopService : MonoBehaviour
{
    public static ShopService Instance { get; private set; }

    [Header("Dependencies - 인스펙터로 연결")]
    public MoneyManager moneyManager;
    public ShopManager shopManager;

    // 이 변수는 인스펙터로 연결하지 않고, 스크립트에서 직접 찾습니다.
    public GameManager gameManager;

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
            return;
        }
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public List<PurchasableItemData> GetShopItems()
    {
        if (shopManager == null)
        {
            Debug.LogError("ShopManager가 할당되지 않아 빈 리스트를 반환합니다.");
            return new List<PurchasableItemData>();
        }

        var allItems = shopManager.GetShopItems();

        if (gameManager == null || gameManager.CurrentGameData == null)
        {
            return allItems;
        }

        var availableItems = allItems.Where(item =>
        {
            if (item.itemType == ItemType.Equipment)
            {
                return !gameManager.CurrentGameData.ownedEquipmentIds.Contains(item.equipmentData.id);
            }
            return true;
        }).ToList();

        return availableItems;
    }

    public bool CanBuy(PurchasableItemData itemData)
    {
        GameData gameData = gameManager.CurrentGameData;
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
            case ItemType.Building: return BuildingHandler.Instance.CanBuy();
            case ItemType.Animal: return AnimalHandler.Instance.CanBuy(itemData.animalData);
            case ItemType.Equipment: return EquipmentHandler.Instance.CanBuy(itemData.equipmentData);
            case ItemType.Upgrade: return UpgradeHandler.Instance.CanBuy(itemData.upgradeData);
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
        GameData gameData = gameManager.CurrentGameData;
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
            if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("돈이 부족합니다.");
            return;
        }

        switch (itemToPurchase.itemType)
        {
            case ItemType.Animal:
                AnimalHandler.Instance.Purchase(itemToPurchase.animalData);
                if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification(itemToPurchase.itemName + "을(를) 구매했습니다!");
                break;
            case ItemType.Building:
                BuildingHandler.Instance.Purchase(itemToPurchase.buildingData);
                break;
            case ItemType.Equipment:
                EquipmentHandler.Instance.Purchase(itemToPurchase.equipmentData);
                break;
            case ItemType.Upgrade:
                UpgradeHandler.Instance.Purchase(itemToPurchase.upgradeData);
                break;
            case ItemType.Consumable:
                if (itemToPurchase.itemName == "총알(30개)")
                {
                    gameData.bulletCount += itemToPurchase.consumableData.amount;
                    if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification(itemToPurchase.itemName + "을(를) 구매했습니다.");
                }
                break;
        }
    }

    private int GetCurrentUpgradeLevel(UpgradeData upgradeData)
    {
        if (gameManager == null || gameManager.CurrentGameData == null) return 0;
        if (upgradeData is BasketUpgradeData) return gameManager.CurrentGameData.basketLevel;
        if (upgradeData is MilkerUpgradeData) return gameManager.CurrentGameData.milkerLevel;
        if (upgradeData is GunUpgradeData) return gameManager.CurrentGameData.gunLevel;
        if (upgradeData is PastureUpgradeData) return gameManager.CurrentGameData.pastureLevel;
        return 0;
    }

    public void SellItem(Animal animalToSell)
    {
        GameData gameData = gameManager.CurrentGameData;
        if (gameData == null || AnimalHandler.Instance == null) return;

        int sellPrice = animalToSell.animalData.animalPrice / 2;
        AnimalHandler.Instance.Sell(animalToSell, sellPrice);
    }

    public void SellChicken()
    {
        if (AnimalHandler.Instance != null && AnimalHandler.Instance.CanSellChicken())
        {
            moneyManager.AddMoney(GetChickenSellPrice());
            AnimalHandler.Instance.RemoveChicken();
        }
        else
        {
            if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("판매할 닭이 없습니다.");
        }
    }

    public int GetChickenSellPrice()
    {
        var chickenSellData = GetShopItems().FirstOrDefault(item => item.animalData != null && item.animalData.animalType == AnimalType.Chicken);
        return chickenSellData != null ? chickenSellData.animalData.animalPrice / 2 : 0;
    }
}