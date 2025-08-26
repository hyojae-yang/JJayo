// ShopService.cs

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ShopService : MonoBehaviour
{
    public static ShopService Instance { get; private set; }

    [Header("데이터 및 스폰 포인트")]
    public List<PurchasableItemData> shopItems;
    public List<Transform> cowSpawnPoints;
    public List<Transform> buildingSpawnPoints;

    // ★★★ 젖소 오브젝트 풀 변수 추가 ★★★
    [Header("오브젝트 풀")]
    public ObjectPool cowObjectPool;

    private PlayerInventory playerInventory;
    private GameManager gameManager;

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

    void Start()
    {
        playerInventory = PlayerInventory.Instance;
        gameManager = GameManager.Instance;
        shopItems.RemoveAll(item => item == null);
    }

    public bool CanBuy(PurchasableItemData itemData)
    {
        if (gameManager.CurrentMoney < itemData.itemPrice)
        {
            NotificationManager.Instance.ShowNotification("돈이 부족합니다.");
            return false;
        }

        switch (itemData.itemType)
        {
            case ItemType.Building:
                if (itemData.itemPrefab != null)
                {
                    if (FindObjectsOfType<Transform>().Any(t => t.CompareTag("Building") && t.name.Contains(itemData.itemName)))
                    {
                        return false;
                    }
                }
                break;
            case ItemType.Equipment:
                if (itemData.equipmentData == null) return false;
                switch (itemData.equipmentData.equipmentType)
                {
                    case EquipmentType.Gun:
                        if (playerInventory.hasGun) return false;
                        break;
                    case EquipmentType.Basket:
                        if (playerInventory.basketLevel > 0) return false;
                        break;
                    case EquipmentType.Milker:
                        if (playerInventory.milkerLevel > 0) return false;
                        break;
                }
                break;
            case ItemType.Upgrade:
                if (itemData.upgradeData == null) return false;
                int currentLevel = 0;

                if (itemData.upgradeData is BasketUpgradeData)
                    currentLevel = playerInventory.basketLevel;
                else if (itemData.upgradeData is MilkerUpgradeData)
                    currentLevel = playerInventory.milkerLevel;
                else if (itemData.upgradeData is GunUpgradeData)
                    currentLevel = playerInventory.gunLevel;
                else if (itemData.upgradeData is PastureUpgradeData)
                    currentLevel = gameManager.CurrentPastureLevel;

                if (itemData.upgradeData is PastureUpgradeData pastureData)
                {
                    if (currentLevel >= pastureData.upgradeLevels.Count - 1)
                    {
                        NotificationManager.Instance.ShowNotification("목초가 최대 레벨입니다.");
                        return false;
                    }
                    itemData.itemPrice = pastureData.upgradeLevels[currentLevel + 1].upgradePrice;
                }
                else
                {
                    if (currentLevel == 0) return false;
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
                break;
            case ItemType.Animal:
                if (itemData.animalData != null && itemData.animalData.animalType == AnimalType.Chicken)
                {
                    if (FindAnyObjectByType<ChickenCoop>() == null) return false;
                }
                break;
            case ItemType.Consumable:
                if (itemData.consumableData != null && itemData.itemName == "총알(30개)" && !playerInventory.hasGun)
                {
                    return false;
                }
                break;
        }

        return true;
    }

    public void PurchaseItem(PurchasableItemData itemToPurchase)
    {
        if (gameManager.SpendMoney(itemToPurchase.itemPrice))
        {
            switch (itemToPurchase.itemType)
            {
                case ItemType.Animal:
                    switch (itemToPurchase.animalData.animalType)
                    {
                        case AnimalType.Cow:
                            if (cowSpawnPoints.Count > 0)
                            {
                                // ★★★ 수정: Instantiate 대신 오브젝트 풀에서 젖소를 가져옵니다. ★★★
                                GameObject newCow = cowObjectPool.GetFromPool();
                                if (newCow != null)
                                {
                                    newCow.transform.position = cowSpawnPoints[0].position;
                                    cowSpawnPoints.RemoveAt(0);
                                    NotificationManager.Instance.ShowNotification("젖소를 구매했습니다!");
                                }
                                else
                                {
                                    // 풀에 여유가 없는 경우 (선택 사항)
                                    NotificationManager.Instance.ShowNotification("젖소를 놓을 자리가 없습니다.");
                                    gameManager.AddMoney(itemToPurchase.itemPrice);
                                }
                            }
                            else
                            {
                                NotificationManager.Instance.ShowNotification("젖소를 놓을 자리가 없습니다.");
                                gameManager.AddMoney(itemToPurchase.itemPrice);
                            }
                            break;
                        case AnimalType.Chicken:
                            if (ChickenCoop.Instance != null)
                            {
                                ChickenCoop.Instance.AddChicken();
                                NotificationManager.Instance.ShowNotification("닭을 구매했습니다.");
                            }
                            else
                            {
                                NotificationManager.Instance.ShowNotification("닭장이 없습니다. 먼저 닭장을 구매하세요!");
                                gameManager.AddMoney(itemToPurchase.itemPrice);
                            }
                            break;
                    }
                    break;
                case ItemType.Building:
                    if (buildingSpawnPoints.Count > 0)
                    {
                        Instantiate(itemToPurchase.itemPrefab, buildingSpawnPoints[0].position, Quaternion.identity);
                        buildingSpawnPoints.RemoveAt(0);
                        NotificationManager.Instance.ShowNotification(itemToPurchase.itemName + "을(를) 구매했습니다. 목장에 설치되었습니다!");
                    }
                    else
                    {
                        NotificationManager.Instance.ShowNotification("건물을 놓을 자리가 없습니다.");
                        gameManager.AddMoney(itemToPurchase.itemPrice);
                    }
                    break;
                case ItemType.Equipment:
                    if (itemToPurchase.equipmentData != null)
                    {
                        switch (itemToPurchase.equipmentData.equipmentType)
                        {
                            case EquipmentType.Gun:
                                playerInventory.hasGun = true;
                                playerInventory.gunLevel = 1;
                                NotificationManager.Instance.ShowNotification("총을 구매했습니다!");
                                break;
                            case EquipmentType.Basket:
                                playerInventory.basketLevel = 1;
                                NotificationManager.Instance.ShowNotification("바구니를 구매했습니다!");
                                break;
                            case EquipmentType.Milker:
                                playerInventory.milkerLevel = 1;
                                NotificationManager.Instance.ShowNotification("착유기를 구매했습니다!");
                                break;
                        }
                    }
                    break;
                case ItemType.Consumable:
                    playerInventory.AddBullets(itemToPurchase.consumableData.amount);
                    NotificationManager.Instance.ShowNotification(itemToPurchase.itemName + "을(를) 구매했습니다.");
                    break;
                case ItemType.Upgrade:
                    if (itemToPurchase.upgradeData is BasketUpgradeData)
                    {
                        playerInventory.UpgradeBasket();
                    }
                    else if (itemToPurchase.upgradeData is MilkerUpgradeData)
                    {
                        playerInventory.UpgradeMilker();
                    }
                    else if (itemToPurchase.upgradeData is GunUpgradeData)
                    {
                        playerInventory.UpgradeGun();
                    }
                    else if (itemToPurchase.upgradeData is PastureUpgradeData)
                    {
                        gameManager.UpgradePasture();
                        NotificationManager.Instance.ShowNotification("목초가 레벨 " + gameManager.CurrentPastureLevel + "로 업그레이드 되었습니다!");
                    }
                    break;
            }
        }
        else
        {
            NotificationManager.Instance.ShowNotification("소지 금액이 부족합니다.");
        }
    }

    public void SellItem(Animal animalToSell)
    {
        int sellPrice = animalToSell.animalData.animalPrice / 2;
        gameManager.AddMoney(sellPrice);
        NotificationManager.Instance.ShowNotification(animalToSell.animalData.animalName + "을(를) " + sellPrice + "원에 판매했습니다!");

        // ★★★ 수정: Destroy 대신 오브젝트를 풀로 반환합니다. ★★★
        if (cowObjectPool != null)
        {
            cowObjectPool.ReturnToPool(animalToSell.gameObject);
        }
        else
        {
            Destroy(animalToSell.gameObject);
        }
    }

    public void SellChicken()
    {
        ChickenCoop chickenCoop = FindAnyObjectByType<ChickenCoop>();
        if (chickenCoop != null && chickenCoop.numberOfChickens > 0)
        {
            var chickenSellData = shopItems.FirstOrDefault(item => item.animalData != null && item.animalData.animalType == AnimalType.Chicken);
            if (chickenSellData != null)
            {
                int sellPrice = chickenSellData.animalData.animalPrice / 2;
                gameManager.AddMoney(sellPrice);
                chickenCoop.RemoveChicken();
                NotificationManager.Instance.ShowNotification($"닭 1마리를 {sellPrice}원에 판매했습니다. 남은 닭: {chickenCoop.numberOfChickens}마리");
            }
        }
    }
}