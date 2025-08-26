using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ShopService : MonoBehaviour
{
    public static ShopService Instance { get; private set; }

    [Header("���� ����Ʈ")]
    public List<Transform> cowSpawnPoints;
    public List<Transform> buildingSpawnPoints;

    [Header("������Ʈ Ǯ")]
    public ObjectPool cowObjectPool;

    // GameData�� �ʿ��� ������ GameManager���� ���� �������Ƿ� �� �̻� ��� ������ ���� �ʽ��ϴ�.
    // private GameData gameData; 
    // shopItems�� �� �̻� ��� ������ ���� �ʽ��ϴ�.
    // private List<PurchasableItemData> shopItems;

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
    }

    // Start() �Լ��� ���� �ʿ� �����Ƿ� �����մϴ�.
    // void Start()
    // {
    //     gameData = GameManager.Instance.gameData;
    //     if (gameData == null)
    //     {
    //         Debug.LogError("GameData�� ã�� �� �����ϴ�.");
    //     }
    // }

    public List<PurchasableItemData> GetShopItems()
    {
        if (ShopManager.Instance != null)
        {
            return ShopManager.Instance.GetShopItems();
        }
        Debug.LogError("ShopManager�� ã�� �� ���� �� ����Ʈ�� ��ȯ�մϴ�.");
        return new List<PurchasableItemData>();
    }

    /// <summary>
    /// �������� ������ �� �ִ��� Ȯ���մϴ�.
    /// </summary>
    public bool CanBuy(PurchasableItemData itemData)
    {
        // �ڡڡ� ���� �κ� �ڡڡ�
        // GameData�� �ʿ��� ������ GameManager���� ���������� ����
        GameData gameData = GameManager.Instance.gameData;
        if (gameData == null)
        {
            Debug.LogError("GameData�� ã�� �� ���� ���� ���� ���θ� Ȯ���� �� �����ϴ�.");
            return false;
        }

        if (gameData.money < itemData.itemPrice)
        {
            NotificationManager.Instance.ShowNotification("���� �����մϴ�.");
            return false;
        }

        switch (itemData.itemType)
        {
            case ItemType.Building:
                if (itemData.itemPrefab != null)
                {
                    if (FindObjectsByType<Transform>(FindObjectsSortMode.None).Any(t => t.CompareTag("Building") && t.name.Contains(itemData.itemName)))
                    {
                        NotificationManager.Instance.ShowNotification("�̹� �ǹ��� ������ �ֽ��ϴ�.");
                        return false;
                    }
                }
                break;
            case ItemType.Equipment:
                if (itemData.equipmentData == null) return false;
                switch (itemData.equipmentData.equipmentType)
                {
                    case EquipmentType.Gun:
                        if (gameData.hasGun)
                        {
                            NotificationManager.Instance.ShowNotification("�̹� ���� ������ �ֽ��ϴ�.");
                            return false;
                        }
                        break;
                    case EquipmentType.Basket:
                        if (gameData.basketLevel > 0)
                        {
                            NotificationManager.Instance.ShowNotification("�̹� �ٱ��ϸ� ������ �ֽ��ϴ�.");
                            return false;
                        }
                        break;
                    case EquipmentType.Milker:
                        if (gameData.milkerLevel > 0)
                        {
                            NotificationManager.Instance.ShowNotification("�̹� �����⸦ ������ �ֽ��ϴ�.");
                            return false;
                        }
                        break;
                }
                break;
            case ItemType.Upgrade:
                if (itemData.upgradeData == null) return false;
                int currentLevel = 0;

                if (itemData.upgradeData is BasketUpgradeData)
                    currentLevel = gameData.basketLevel;
                else if (itemData.upgradeData is MilkerUpgradeData)
                    currentLevel = gameData.milkerLevel;
                else if (itemData.upgradeData is GunUpgradeData)
                    currentLevel = gameData.gunLevel;
                else if (itemData.upgradeData is PastureUpgradeData)
                    currentLevel = gameData.pastureLevel;

                if (itemData.upgradeData is PastureUpgradeData pastureData)
                {
                    if (currentLevel >= pastureData.upgradeLevels.Count - 1)
                    {
                        NotificationManager.Instance.ShowNotification("���ʰ� �ִ� �����Դϴ�.");
                        return false;
                    }
                    itemData.itemPrice = pastureData.upgradeLevels[currentLevel + 1].upgradePrice;
                }
                else
                {
                    if (currentLevel == 0)
                    {
                        NotificationManager.Instance.ShowNotification("��� ���� �����ؾ� ���׷��̵��� �� �ֽ��ϴ�.");
                        return false;
                    }

                    if (itemData.upgradeData is BasketUpgradeData basketData)
                    {
                        if (currentLevel >= basketData.upgradeLevels.Count) return false;
                        itemData.itemPrice = basketData.upgradeLevels[currentLevel].upgradePrice;
                    }
                    else if (itemData.upgradeData is MilkerUpgradeData milkerData)
                    {
                        if (currentLevel >= milkerData.upgradeLevels.Count) return false;
                        itemData.itemPrice = milkerData.upgradeLevels[currentLevel].upgradePrice;
                    }
                    else if (itemData.upgradeData is GunUpgradeData gunData)
                    {
                        if (currentLevel >= gunData.upgradeLevels.Count) return false;
                        itemData.itemPrice = gunData.upgradeLevels[currentLevel].upgradePrice;
                    }
                }
                break;
            case ItemType.Animal:
                if (itemData.animalData != null && itemData.animalData.animalType == AnimalType.Chicken)
                {
                    if (FindAnyObjectByType<ChickenCoop>() == null)
                    {
                        NotificationManager.Instance.ShowNotification("������ �����ϴ�. ���� ������ �����ϼ���.");
                        return false;
                    }
                }
                break;
            case ItemType.Consumable:
                if (itemData.consumableData != null && itemData.itemName == "�Ѿ�(30��)" && !gameData.hasGun)
                {
                    NotificationManager.Instance.ShowNotification("���� �־�� �Ѿ��� ������ �� �ֽ��ϴ�.");
                    return false;
                }
                break;
        }
        return true;
    }

    /// <summary>
    /// ������ ���Ÿ� ó���մϴ�.
    /// </summary>
    public void PurchaseItem(PurchasableItemData itemToPurchase)
    {
        // �ڡڡ� ���� �κ� �ڡڡ�
        GameData gameData = GameManager.Instance.gameData;
        if (gameData == null)
        {
            Debug.LogError("GameData�� ã�� �� ���� �������� ������ �� �����ϴ�.");
            return;
        }

        gameData.money -= itemToPurchase.itemPrice;

        switch (itemToPurchase.itemType)
        {
            case ItemType.Animal:
                switch (itemToPurchase.animalData.animalType)
                {
                    case AnimalType.Cow:
                        if (cowSpawnPoints.Count > 0)
                        {
                            GameObject newCow = cowObjectPool.GetFromPool();
                            if (newCow != null)
                            {
                                newCow.transform.position = cowSpawnPoints[0].position;
                                cowSpawnPoints.RemoveAt(0);
                                NotificationManager.Instance.ShowNotification("���Ҹ� �����߽��ϴ�!");
                            }
                            else
                            {
                                NotificationManager.Instance.ShowNotification("���Ҹ� ���� �ڸ��� �����ϴ�. ���� �ǵ����帳�ϴ�.");
                                gameData.money += itemToPurchase.itemPrice;
                            }
                        }
                        else
                        {
                            NotificationManager.Instance.ShowNotification("���Ҹ� ���� �ڸ��� �����ϴ�. ���� �ǵ����帳�ϴ�.");
                            gameData.money += itemToPurchase.itemPrice;
                        }
                        break;
                    case AnimalType.Chicken:
                        if (ChickenCoop.Instance != null)
                        {
                            ChickenCoop.Instance.AddChicken();
                            NotificationManager.Instance.ShowNotification("���� �����߽��ϴ�.");
                        }
                        else
                        {
                            NotificationManager.Instance.ShowNotification("������ �����ϴ�. ���� ������ �����ϼ���! ���� �ǵ����帳�ϴ�.");
                            gameData.money += itemToPurchase.itemPrice;
                        }
                        break;
                }
                break;
            case ItemType.Building:
                if (buildingSpawnPoints.Count > 0)
                {
                    Instantiate(itemToPurchase.itemPrefab, buildingSpawnPoints[0].position, Quaternion.identity);
                    buildingSpawnPoints.RemoveAt(0);
                    NotificationManager.Instance.ShowNotification(itemToPurchase.itemName + "��(��) �����߽��ϴ�. ���忡 ��ġ�Ǿ����ϴ�!");
                }
                else
                {
                    NotificationManager.Instance.ShowNotification("�ǹ��� ���� �ڸ��� �����ϴ�. ���� �ǵ����帳�ϴ�.");
                    gameData.money += itemToPurchase.itemPrice;
                }
                break;
            case ItemType.Equipment:
                if (itemToPurchase.equipmentData != null)
                {
                    switch (itemToPurchase.equipmentData.equipmentType)
                    {
                        case EquipmentType.Gun:
                            gameData.hasGun = true;
                            gameData.gunLevel = 1;
                            NotificationManager.Instance.ShowNotification("���� �����߽��ϴ�!");
                            break;
                        case EquipmentType.Basket:
                            gameData.basketLevel = 1;
                            NotificationManager.Instance.ShowNotification("�ٱ��ϸ� �����߽��ϴ�!");
                            break;
                        case EquipmentType.Milker:
                            gameData.milkerLevel = 1;
                            NotificationManager.Instance.ShowNotification("�����⸦ �����߽��ϴ�!");
                            break;
                    }
                }
                break;
            case ItemType.Consumable:
                if (itemToPurchase.itemName == "�Ѿ�(30��)")
                {
                    gameData.bulletCount += itemToPurchase.consumableData.amount;
                    NotificationManager.Instance.ShowNotification(itemToPurchase.itemName + "��(��) �����߽��ϴ�.");
                }
                break;
            case ItemType.Upgrade:
                if (itemToPurchase.upgradeData is BasketUpgradeData)
                {
                    gameData.basketLevel++;
                    NotificationManager.Instance.ShowNotification("�ٱ��ϰ� ���׷��̵� �Ǿ����ϴ�!");
                }
                else if (itemToPurchase.upgradeData is MilkerUpgradeData)
                {
                    gameData.milkerLevel++;
                    NotificationManager.Instance.ShowNotification("�����Ⱑ ���׷��̵� �Ǿ����ϴ�!");
                }
                else if (itemToPurchase.upgradeData is GunUpgradeData)
                {
                    gameData.gunLevel++;
                    NotificationManager.Instance.ShowNotification("���� ���׷��̵� �Ǿ����ϴ�!");
                }
                else if (itemToPurchase.upgradeData is PastureUpgradeData)
                {
                    gameData.pastureLevel++;
                    if (PastureManager.Instance != null)
                    {
                        PastureManager.Instance.UpdateVisuals();
                    }
                    NotificationManager.Instance.ShowNotification("���ʰ� ���� " + gameData.pastureLevel + "�� ���׷��̵� �Ǿ����ϴ�!");
                }
                break;
        }
    }

    /// <summary>
    /// ������ �Ǹ��մϴ�.
    /// </summary>
    public void SellItem(Animal animalToSell)
    {
        // �ڡڡ� ���� �κ� �ڡڡ�
        GameData gameData = GameManager.Instance.gameData;
        if (gameData == null)
        {
            Debug.LogError("GameData�� ã�� �� ���� �������� �Ǹ��� �� �����ϴ�.");
            return;
        }

        int sellPrice = animalToSell.animalData.animalPrice / 2;
        gameData.money += sellPrice;
        NotificationManager.Instance.ShowNotification(animalToSell.animalData.animalName + "��(��) " + sellPrice + "���� �Ǹ��߽��ϴ�!");

        if (cowObjectPool != null)
        {
            cowObjectPool.ReturnToPool(animalToSell.gameObject);
        }
        else
        {
            Destroy(animalToSell.gameObject);
        }
    }

    /// <summary>
    /// ���� �Ǹ��մϴ�.
    /// </summary>
    public void SellChicken()
    {
        // �ڡڡ� ���� �κ� �ڡڡ�
        GameData gameData = GameManager.Instance.gameData;
        if (gameData == null)
        {
            Debug.LogError("GameData�� ã�� �� ���� ���� �Ǹ��� �� �����ϴ�.");
            return;
        }

        ChickenCoop chickenCoop = FindAnyObjectByType<ChickenCoop>();
        if (chickenCoop != null && chickenCoop.numberOfChickens > 0)
        {
            // shopItems ��� ������ ������� �ʰ� GetShopItems()�� ���� ȣ��
            var chickenSellData = GetShopItems().FirstOrDefault(item => item.animalData != null && item.animalData.animalType == AnimalType.Chicken);
            if (chickenSellData != null)
            {
                int sellPrice = chickenSellData.animalData.animalPrice / 2;
                gameData.money += sellPrice;
                chickenCoop.RemoveChicken();
                NotificationManager.Instance.ShowNotification($"�� 1������ {sellPrice}���� �Ǹ��߽��ϴ�. ���� ��: {chickenCoop.numberOfChickens}����");
            }
        }
    }

    /// <summary>
    /// �� �Ǹ� ������ ��ȯ�ϴ� �޼��带 �߰��մϴ�.
    /// </summary>
    public int GetChickenSellPrice()
    {
        // shopItems ��� ������ ������� �ʰ� GetShopItems()�� ���� ȣ��
        var chickenSellData = GetShopItems().FirstOrDefault(item => item.animalData != null && item.animalData.animalType == AnimalType.Chicken);
        if (chickenSellData != null)
        {
            return chickenSellData.animalData.animalPrice / 2;
        }
        return 0;
    }
}