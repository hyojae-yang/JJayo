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

    public List<PurchasableItemData> GetShopItems()
    {
        if (ShopManager.Instance != null)
        {
            return ShopManager.Instance.GetShopItems();
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
            // ★★★ BuildingHandler.CanBuy는 이제 인자를 받지 않습니다.
            case ItemType.Building: return buildingHandler.CanBuy();
            case ItemType.Animal: return animalHandler.CanBuy(itemData.animalData);
            case ItemType.Equipment: return equipmentHandler.CanBuy(itemData.equipmentData);
            case ItemType.Upgrade: return upgradeHandler.CanBuy(itemData.upgradeData);
            case ItemType.Consumable:
                if (itemData.consumableData != null && itemData.itemName == "총알(30개)")
                {
                    return gameData.hasGun;
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
            finalPrice = itemToPurchase.upgradeData.GetUpgradePrice(currentLevel); // 수정: GetUpgradePrice가 다음 레벨 가격을 반환하도록 수정했기 때문에 currentLevel만 넘겨줍니다.
        }
        else if (itemToPurchase.itemType == ItemType.Building) // 건물 가격을 buildingData에서 가져옵니다.
        {
            finalPrice = itemToPurchase.buildingData.buildingPrice;
        }
        else // 기타 아이템 가격
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
            // ★★★ BuildingHandler.Purchase에 BuildingData를 넘겨줍니다.
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

        // 업그레이드 아이템 구매 후 UI를 갱신합니다.
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