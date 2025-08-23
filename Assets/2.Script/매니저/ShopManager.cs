using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    // 싱글톤
    public static ShopManager Instance { get; private set; }

    [Header("패널 UI")]
    public GameObject shopPanel;
    public GameObject buyPanel;
    public GameObject sellPanel;
    public GameObject confirmationPanel;

    [Header("콘텐츠 UI")]
    public Transform buyContentPanel;
    public Transform sellContentPanel;

    [Header("데이터 및 프리팹")]
    public List<PurchasableItemData> shopItems;
    public GameObject uiItemCardPrefab;

    [Header("스폰 포인트")]
    public List<Transform> cowSpawnPoints;
    public List<Transform> buildingSpawnPoints;

    [Header("알림창 UI 요소")]
    public TextMeshProUGUI confirmText;
    private PurchasableItemData itemToPurchase;

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
        shopItems.RemoveAll(item => item == null);

        shopPanel.SetActive(false);
        buyPanel.SetActive(true);
        sellPanel.SetActive(false);
        confirmationPanel.SetActive(false);

        playerInventory = PlayerInventory.Instance;
        gameManager = GameManager.Instance;

        ShowBuyPanel();
    }

    public void ShowShop()
    {
        shopPanel.SetActive(true);
        ShowBuyPanel();
    }

    public void HideShop()
    {
        shopPanel.SetActive(false);
    }

    public void ShowBuyPanel()
    {
        buyPanel.SetActive(true);
        sellPanel.SetActive(false);
        PopulateBuyItems();
    }

    public void ShowSellPanel()
    {
        buyPanel.SetActive(false);
        sellPanel.SetActive(true);
        PopulateSellItems();
    }

    private void PopulateBuyItems()
    {
        foreach (Transform child in buyContentPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (PurchasableItemData data in shopItems)
        {
            if (data == null) continue;

            bool canShowItem = true;
            bool isMaxLevel = false;

            switch (data.itemType)
            {
                case ItemType.Building:
                    if (FindAnyObjectByType<ChickenCoop>() != null) canShowItem = false;
                    break;

                case ItemType.Equipment:
                    if (data.equipmentData == null) continue;
                    switch (data.equipmentData.equipmentType)
                    {
                        case EquipmentType.Gun:
                            if (playerInventory.hasGun) canShowItem = false;
                            break;
                        case EquipmentType.Basket:
                            if (playerInventory.basketLevel > 0) canShowItem = false;
                            break;
                        case EquipmentType.Milker:
                            if (playerInventory.milkerLevel > 0) canShowItem = false;
                            break;
                    }
                    break;

                case ItemType.Upgrade:
                    if (data.upgradeData == null) continue;

                    int currentLevel = 0;
                    if (data.upgradeData is BasketUpgradeData basketUpgradeData)
                    {
                        if (playerInventory.basketLevel == 0) canShowItem = false;
                        else
                        {
                            currentLevel = playerInventory.basketLevel;
                            if (currentLevel >= basketUpgradeData.upgradeLevels.Count) isMaxLevel = true;
                        }
                    }
                    else if (data.upgradeData is MilkerUpgradeData milkerUpgradeData)
                    {
                        if (playerInventory.milkerLevel == 0) canShowItem = false;
                        else
                        {
                            currentLevel = playerInventory.milkerLevel;
                            if (currentLevel >= milkerUpgradeData.upgradeLevels.Count) isMaxLevel = true;
                        }
                    }
                    else if (data.upgradeData is GunUpgradeData gunUpgradeData)
                    {
                        if (!playerInventory.hasGun) canShowItem = false;
                        else
                        {
                            currentLevel = playerInventory.gunLevel;
                            if (currentLevel >= gunUpgradeData.upgradeLevels.Count) isMaxLevel = true;
                        }
                    }

                    if (isMaxLevel) canShowItem = false;

                    if (canShowItem)
                    {
                        if (data.upgradeData is BasketUpgradeData basketData)
                            data.itemPrice = basketData.upgradeLevels[currentLevel].upgradePrice;
                        else if (data.upgradeData is MilkerUpgradeData milkerData)
                            data.itemPrice = milkerData.upgradeLevels[currentLevel].upgradePrice;
                        else if (data.upgradeData is GunUpgradeData gunData)
                            data.itemPrice = gunData.upgradeLevels[currentLevel].upgradePrice;
                    }
                    break;

                case ItemType.Animal:
                    if (data.animalData != null && data.animalData.animalType == AnimalType.Chicken)
                    {
                        if (FindAnyObjectByType<ChickenCoop>() == null) canShowItem = false;
                    }
                    break;

                case ItemType.Consumable:
                    // 수정된 부분: consumableType 대신 itemName을 사용합니다.
                    if (data.consumableData != null && data.itemName == "총알" && !playerInventory.hasGun)
                    {
                        canShowItem = false;
                    }
                    break;
            }

            if (canShowItem)
            {
                GameObject itemCard = Instantiate(uiItemCardPrefab, buyContentPanel);
                ShopItemUI itemUI = itemCard.GetComponent<ShopItemUI>();
                itemUI.SetupBuyItem(this, data);
            }
        }
    }

    private void PopulateSellItems()
    {
        foreach (Transform child in sellContentPanel)
        {
            Destroy(child.gameObject);
        }

        Animal[] allCows = FindObjectsByType<Animal>(FindObjectsSortMode.None);
        foreach (Animal animal in allCows)
        {
            if (animal.animalData.animalType == AnimalType.Cow)
            {
                GameObject itemCard = Instantiate(uiItemCardPrefab, sellContentPanel);
                ShopItemUI itemUI = itemCard.GetComponent<ShopItemUI>();
                itemUI.SetupSellItem(this, animal);
            }
        }

        ChickenCoop chickenCoop = FindAnyObjectByType<ChickenCoop>();
        if (chickenCoop != null && chickenCoop.numberOfChickens > 0)
        {
            var chickenSellData = shopItems.FirstOrDefault(item => item.animalData != null && item.animalData.animalType == AnimalType.Chicken);
            if (chickenSellData != null)
            {
                GameObject itemCard = Instantiate(uiItemCardPrefab, sellContentPanel);
                ShopItemUI itemUI = itemCard.GetComponent<ShopItemUI>();
                itemUI.itemNameText.text = chickenSellData.animalData.animalName + $" (현재 {chickenCoop.numberOfChickens}마리)";
                itemUI.itemPriceText.text = (chickenSellData.animalData.animalPrice / 2).ToString("C0");
                itemUI.itemIcon.sprite = chickenSellData.animalData.animalIcon;
                itemUI.actionButton.onClick.RemoveAllListeners();
                itemUI.actionButton.onClick.AddListener(() => SellChicken());
            }
        }
    }

    public void OnClickBuy(PurchasableItemData itemData)
    {
        if (itemData.itemType == ItemType.Upgrade)
        {
            UpgradeData upgradeData = itemData.upgradeData;
            int currentLevel = 0;

            if (upgradeData is BasketUpgradeData)
                currentLevel = playerInventory.basketLevel;
            else if (upgradeData is MilkerUpgradeData)
                currentLevel = playerInventory.milkerLevel;
            else if (upgradeData is GunUpgradeData)
                currentLevel = playerInventory.gunLevel;

            if (upgradeData is BasketUpgradeData basketData)
            {
                if (currentLevel >= basketData.upgradeLevels.Count)
                {
                    NotificationManager.Instance.ShowNotification("이 아이템은 이미 최대 레벨입니다!");
                    return;
                }
            }
            else if (upgradeData is MilkerUpgradeData milkerData)
            {
                if (currentLevel >= milkerData.upgradeLevels.Count)
                {
                    NotificationManager.Instance.ShowNotification("이 아이템은 이미 최대 레벨입니다!");
                    return;
                }
            }
            else if (upgradeData is GunUpgradeData gunData)
            {
                if (currentLevel >= gunData.upgradeLevels.Count)
                {
                    NotificationManager.Instance.ShowNotification("이 아이템은 이미 최대 레벨입니다!");
                    return;
                }
            }
            else
            {
                NotificationManager.Instance.ShowNotification("알 수 없는 업그레이드 데이터입니다.");
                return;
            }
        }

        if (gameManager.CurrentMoney < itemData.itemPrice)
        {
            NotificationManager.Instance.ShowNotification("돈이 부족합니다.");
            return;
        }

        itemToPurchase = itemData;
        confirmationPanel.SetActive(true);
        confirmText.text = $"{itemData.itemName}을(를) {itemData.itemPrice}원에 구매하시겠습니까?";
    }

    public void ConfirmPurchase()
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
                                Instantiate(itemToPurchase.animalData.animalPrefab, cowSpawnPoints[0].position, Quaternion.identity);
                                cowSpawnPoints.RemoveAt(0);
                                NotificationManager.Instance.ShowNotification("젖소를 구매했습니다!");
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
                                NotificationManager.Instance.ShowNotification("닭을 구매했습니다. 닭장 수가 늘어납니다.");
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
                        BasketUpgradeData data = itemToPurchase.upgradeData as BasketUpgradeData;
                        playerInventory.basketLevel++;
                        playerInventory.basketCapacity = data.upgradeLevels[playerInventory.basketLevel - 1].capacity;
                        NotificationManager.Instance.ShowNotification("바구니가 레벨 " + playerInventory.basketLevel + "로 업그레이드 되었습니다!");
                    }
                    else if (itemToPurchase.upgradeData is MilkerUpgradeData)
                    {
                        MilkerUpgradeData data = itemToPurchase.upgradeData as MilkerUpgradeData;
                        playerInventory.milkerLevel++;
                        playerInventory.milkerCapacity = data.upgradeLevels[playerInventory.milkerLevel - 1].capacity;
                        playerInventory.milkerSpeed = data.upgradeLevels[playerInventory.milkerLevel - 1].speed;
                        NotificationManager.Instance.ShowNotification("착유기가 레벨 " + playerInventory.milkerLevel + "로 업그레이드 되었습니다!");
                    }
                    else if (itemToPurchase.upgradeData is GunUpgradeData)
                    {
                        GunUpgradeData data = itemToPurchase.upgradeData as GunUpgradeData;
                        playerInventory.gunLevel++;
                        playerInventory.gunDamage = data.upgradeLevels[playerInventory.gunLevel - 1].damageIncrease;
                        playerInventory.gunFireRate = data.upgradeLevels[playerInventory.gunLevel - 1].fireRateIncrease;
                        NotificationManager.Instance.ShowNotification("총이 레벨 " + playerInventory.gunLevel + "로 업그레이드 되었습니다!");
                    }
                    break;
            }
        }
        else
        {
            NotificationManager.Instance.ShowNotification("소지 금액이 부족합니다.");
        }

        confirmationPanel.SetActive(false);
        PopulateBuyItems();
    }

    public void CancelPurchase()
    {
        confirmationPanel.SetActive(false);
        itemToPurchase = null;
    }

    public void SellItem(Animal animalToSell)
    {
        int sellPrice = animalToSell.animalData.animalPrice / 2;
        gameManager.AddMoney(sellPrice);
        NotificationManager.Instance.ShowNotification(animalToSell.animalData.animalName + "을(를) " + sellPrice + "원에 판매했습니다!");
        Destroy(animalToSell.gameObject);
        PopulateSellItems();
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
        PopulateSellItems();
    }
}