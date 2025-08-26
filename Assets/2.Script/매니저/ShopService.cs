using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ShopService : MonoBehaviour
{
    public static ShopService Instance { get; private set; }

    [Header("스폰 포인트")]
    public List<Transform> cowSpawnPoints;
    public List<Transform> buildingSpawnPoints;

    [Header("오브젝트 풀")]
    public ObjectPool cowObjectPool;

    // GameData를 필요할 때마다 GameManager에서 직접 가져오므로 더 이상 멤버 변수로 두지 않습니다.
    // private GameData gameData; 
    // shopItems도 더 이상 멤버 변수로 두지 않습니다.
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

    // Start() 함수는 이제 필요 없으므로 삭제합니다.
    // void Start()
    // {
    //     gameData = GameManager.Instance.gameData;
    //     if (gameData == null)
    //     {
    //         Debug.LogError("GameData를 찾을 수 없습니다.");
    //     }
    // }

    public List<PurchasableItemData> GetShopItems()
    {
        if (ShopManager.Instance != null)
        {
            return ShopManager.Instance.GetShopItems();
        }
        Debug.LogError("ShopManager를 찾을 수 없어 빈 리스트를 반환합니다.");
        return new List<PurchasableItemData>();
    }

    /// <summary>
    /// 아이템을 구매할 수 있는지 확인합니다.
    /// </summary>
    public bool CanBuy(PurchasableItemData itemData)
    {
        // ★★★ 수정 부분 ★★★
        // GameData를 필요할 때마다 GameManager에서 가져오도록 변경
        GameData gameData = GameManager.Instance.gameData;
        if (gameData == null)
        {
            Debug.LogError("GameData를 찾을 수 없어 구매 가능 여부를 확인할 수 없습니다.");
            return false;
        }

        if (gameData.money < itemData.itemPrice)
        {
            NotificationManager.Instance.ShowNotification("돈이 부족합니다.");
            return false;
        }

        switch (itemData.itemType)
        {
            case ItemType.Building:
                if (itemData.itemPrefab != null)
                {
                    if (FindObjectsByType<Transform>(FindObjectsSortMode.None).Any(t => t.CompareTag("Building") && t.name.Contains(itemData.itemName)))
                    {
                        NotificationManager.Instance.ShowNotification("이미 건물을 가지고 있습니다.");
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
                            NotificationManager.Instance.ShowNotification("이미 총을 가지고 있습니다.");
                            return false;
                        }
                        break;
                    case EquipmentType.Basket:
                        if (gameData.basketLevel > 0)
                        {
                            NotificationManager.Instance.ShowNotification("이미 바구니를 가지고 있습니다.");
                            return false;
                        }
                        break;
                    case EquipmentType.Milker:
                        if (gameData.milkerLevel > 0)
                        {
                            NotificationManager.Instance.ShowNotification("이미 착유기를 가지고 있습니다.");
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
                        NotificationManager.Instance.ShowNotification("목초가 최대 레벨입니다.");
                        return false;
                    }
                    itemData.itemPrice = pastureData.upgradeLevels[currentLevel + 1].upgradePrice;
                }
                else
                {
                    if (currentLevel == 0)
                    {
                        NotificationManager.Instance.ShowNotification("장비를 먼저 구매해야 업그레이드할 수 있습니다.");
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
                        NotificationManager.Instance.ShowNotification("닭장이 없습니다. 먼저 닭장을 구매하세요.");
                        return false;
                    }
                }
                break;
            case ItemType.Consumable:
                if (itemData.consumableData != null && itemData.itemName == "총알(30개)" && !gameData.hasGun)
                {
                    NotificationManager.Instance.ShowNotification("총이 있어야 총알을 구매할 수 있습니다.");
                    return false;
                }
                break;
        }
        return true;
    }

    /// <summary>
    /// 아이템 구매를 처리합니다.
    /// </summary>
    public void PurchaseItem(PurchasableItemData itemToPurchase)
    {
        // ★★★ 수정 부분 ★★★
        GameData gameData = GameManager.Instance.gameData;
        if (gameData == null)
        {
            Debug.LogError("GameData를 찾을 수 없어 아이템을 구매할 수 없습니다.");
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
                                NotificationManager.Instance.ShowNotification("젖소를 구매했습니다!");
                            }
                            else
                            {
                                NotificationManager.Instance.ShowNotification("젖소를 놓을 자리가 없습니다. 돈을 되돌려드립니다.");
                                gameData.money += itemToPurchase.itemPrice;
                            }
                        }
                        else
                        {
                            NotificationManager.Instance.ShowNotification("젖소를 놓을 자리가 없습니다. 돈을 되돌려드립니다.");
                            gameData.money += itemToPurchase.itemPrice;
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
                            NotificationManager.Instance.ShowNotification("닭장이 없습니다. 먼저 닭장을 구매하세요! 돈을 되돌려드립니다.");
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
                    NotificationManager.Instance.ShowNotification(itemToPurchase.itemName + "을(를) 구매했습니다. 목장에 설치되었습니다!");
                }
                else
                {
                    NotificationManager.Instance.ShowNotification("건물을 놓을 자리가 없습니다. 돈을 되돌려드립니다.");
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
                            NotificationManager.Instance.ShowNotification("총을 구매했습니다!");
                            break;
                        case EquipmentType.Basket:
                            gameData.basketLevel = 1;
                            NotificationManager.Instance.ShowNotification("바구니를 구매했습니다!");
                            break;
                        case EquipmentType.Milker:
                            gameData.milkerLevel = 1;
                            NotificationManager.Instance.ShowNotification("착유기를 구매했습니다!");
                            break;
                    }
                }
                break;
            case ItemType.Consumable:
                if (itemToPurchase.itemName == "총알(30개)")
                {
                    gameData.bulletCount += itemToPurchase.consumableData.amount;
                    NotificationManager.Instance.ShowNotification(itemToPurchase.itemName + "을(를) 구매했습니다.");
                }
                break;
            case ItemType.Upgrade:
                if (itemToPurchase.upgradeData is BasketUpgradeData)
                {
                    gameData.basketLevel++;
                    NotificationManager.Instance.ShowNotification("바구니가 업그레이드 되었습니다!");
                }
                else if (itemToPurchase.upgradeData is MilkerUpgradeData)
                {
                    gameData.milkerLevel++;
                    NotificationManager.Instance.ShowNotification("착유기가 업그레이드 되었습니다!");
                }
                else if (itemToPurchase.upgradeData is GunUpgradeData)
                {
                    gameData.gunLevel++;
                    NotificationManager.Instance.ShowNotification("총이 업그레이드 되었습니다!");
                }
                else if (itemToPurchase.upgradeData is PastureUpgradeData)
                {
                    gameData.pastureLevel++;
                    if (PastureManager.Instance != null)
                    {
                        PastureManager.Instance.UpdateVisuals();
                    }
                    NotificationManager.Instance.ShowNotification("목초가 레벨 " + gameData.pastureLevel + "로 업그레이드 되었습니다!");
                }
                break;
        }
    }

    /// <summary>
    /// 동물을 판매합니다.
    /// </summary>
    public void SellItem(Animal animalToSell)
    {
        // ★★★ 수정 부분 ★★★
        GameData gameData = GameManager.Instance.gameData;
        if (gameData == null)
        {
            Debug.LogError("GameData를 찾을 수 없어 아이템을 판매할 수 없습니다.");
            return;
        }

        int sellPrice = animalToSell.animalData.animalPrice / 2;
        gameData.money += sellPrice;
        NotificationManager.Instance.ShowNotification(animalToSell.animalData.animalName + "을(를) " + sellPrice + "원에 판매했습니다!");

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
    /// 닭을 판매합니다.
    /// </summary>
    public void SellChicken()
    {
        // ★★★ 수정 부분 ★★★
        GameData gameData = GameManager.Instance.gameData;
        if (gameData == null)
        {
            Debug.LogError("GameData를 찾을 수 없어 닭을 판매할 수 없습니다.");
            return;
        }

        ChickenCoop chickenCoop = FindAnyObjectByType<ChickenCoop>();
        if (chickenCoop != null && chickenCoop.numberOfChickens > 0)
        {
            // shopItems 멤버 변수를 사용하지 않고 GetShopItems()를 직접 호출
            var chickenSellData = GetShopItems().FirstOrDefault(item => item.animalData != null && item.animalData.animalType == AnimalType.Chicken);
            if (chickenSellData != null)
            {
                int sellPrice = chickenSellData.animalData.animalPrice / 2;
                gameData.money += sellPrice;
                chickenCoop.RemoveChicken();
                NotificationManager.Instance.ShowNotification($"닭 1마리를 {sellPrice}원에 판매했습니다. 남은 닭: {chickenCoop.numberOfChickens}마리");
            }
        }
    }

    /// <summary>
    /// 닭 판매 가격을 반환하는 메서드를 추가합니다.
    /// </summary>
    public int GetChickenSellPrice()
    {
        // shopItems 멤버 변수를 사용하지 않고 GetShopItems()를 직접 호출
        var chickenSellData = GetShopItems().FirstOrDefault(item => item.animalData != null && item.animalData.animalType == AnimalType.Chicken);
        if (chickenSellData != null)
        {
            return chickenSellData.animalData.animalPrice / 2;
        }
        return 0;
    }
}