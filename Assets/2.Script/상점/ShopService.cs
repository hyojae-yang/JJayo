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

        if (animalHandler == null) Debug.LogError("AnimalHandler�� �Ҵ���� �ʾҽ��ϴ�.");
        if (buildingHandler == null) Debug.LogError("BuildingHandler�� �Ҵ���� �ʾҽ��ϴ�.");
        if (equipmentHandler == null) Debug.LogError("EquipmentHandler�� �Ҵ���� �ʾҽ��ϴ�.");
        if (upgradeHandler == null) Debug.LogError("UpgradeHandler�� �Ҵ���� �ʾҽ��ϴ�.");
    }

    // �������� ǥ���� ������ ����� �����ɴϴ�.
    // ���� ������ ��� �������� �����մϴ�.
    public List<PurchasableItemData> GetShopItems()
    {
        if (ShopManager.Instance != null)
        {
            // ShopManager���� ��� �������� �����ɴϴ�.
            var allItems = ShopManager.Instance.GetShopItems();

            // GameManager�� �ε���� �ʾ��� ���, ��ü ������ ����� ��ȯ�մϴ�.
            if (GameManager.Instance == null || GameManager.Instance.gameData == null)
            {
                return allItems;
            }

            // ������ ��� �������� ������ ���ο� ����Ʈ�� �����Ͽ� ��ȯ�մϴ�.
            var availableItems = allItems.Where(item =>
            {
                // ������ Ÿ���� ����� ���,
                if (item.itemType == ItemType.Equipment)
                {
                    // �ش� ����� ID�� �̹� ������ ����Ʈ�� ���ԵǾ� �ִ��� Ȯ���մϴ�.
                    // ���ԵǾ� ���� ���� ��쿡�� true�� ��ȯ�Ͽ� ��Ͽ� ����ϴ�.
                    return !GameManager.Instance.gameData.ownedEquipmentIds.Contains(item.equipmentData.id);
                }
                // �ٸ� ������ Ÿ���� �״�� �����մϴ�.
                return true;
            }).ToList();

            return availableItems;
        }

        Debug.LogError("ShopManager�� ã�� �� ���� �� ����Ʈ�� ��ȯ�մϴ�.");
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
                if (itemData.consumableData != null && itemData.itemName == "�Ѿ�(30��)")
                {
                    // �Ѿ��� ���� GameData�� hasGun ���� ��� ownedEquipmentIds ����Ʈ�� Ȯ���մϴ�.
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
                NotificationManager.Instance.ShowNotification(itemToPurchase.itemName + "��(��) �����߽��ϴ�!");
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
                if (itemToPurchase.itemName == "�Ѿ�(30��)")
                {
                    gameData.bulletCount += itemToPurchase.consumableData.amount;
                    NotificationManager.Instance.ShowNotification(itemToPurchase.itemName + "��(��) �����߽��ϴ�.");
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
            NotificationManager.Instance.ShowNotification("�Ǹ��� ���� �����ϴ�.");
        }
    }

    public int GetChickenSellPrice()
    {
        var chickenSellData = GetShopItems().FirstOrDefault(item => item.animalData != null && item.animalData.animalType == AnimalType.Chicken);
        return chickenSellData != null ? chickenSellData.animalData.animalPrice / 2 : 0;
    }
}