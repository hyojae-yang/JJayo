using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    // �̱���
    public static ShopManager Instance { get; private set; }

    [Header("�г� UI")]
    public GameObject shopPanel;
    public GameObject buyPanel;
    public GameObject sellPanel;
    public GameObject confirmationPanel;

    [Header("������ UI")]
    public Transform buyContentPanel;
    public Transform sellContentPanel;

    [Header("������ �� ������")]
    public List<PurchasableItemData> shopItems;
    public GameObject uiItemCardPrefab;

    [Header("���� ����Ʈ")]
    public List<Transform> cowSpawnPoints;
    public List<Transform> buildingSpawnPoints;

    [Header("�˸�â UI ���")]
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
                    // ������ �κ�: consumableType ��� itemName�� ����մϴ�.
                    if (data.consumableData != null && data.itemName == "�Ѿ�" && !playerInventory.hasGun)
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
                itemUI.itemNameText.text = chickenSellData.animalData.animalName + $" (���� {chickenCoop.numberOfChickens}����)";
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
                    NotificationManager.Instance.ShowNotification("�� �������� �̹� �ִ� �����Դϴ�!");
                    return;
                }
            }
            else if (upgradeData is MilkerUpgradeData milkerData)
            {
                if (currentLevel >= milkerData.upgradeLevels.Count)
                {
                    NotificationManager.Instance.ShowNotification("�� �������� �̹� �ִ� �����Դϴ�!");
                    return;
                }
            }
            else if (upgradeData is GunUpgradeData gunData)
            {
                if (currentLevel >= gunData.upgradeLevels.Count)
                {
                    NotificationManager.Instance.ShowNotification("�� �������� �̹� �ִ� �����Դϴ�!");
                    return;
                }
            }
            else
            {
                NotificationManager.Instance.ShowNotification("�� �� ���� ���׷��̵� �������Դϴ�.");
                return;
            }
        }

        if (gameManager.CurrentMoney < itemData.itemPrice)
        {
            NotificationManager.Instance.ShowNotification("���� �����մϴ�.");
            return;
        }

        itemToPurchase = itemData;
        confirmationPanel.SetActive(true);
        confirmText.text = $"{itemData.itemName}��(��) {itemData.itemPrice}���� �����Ͻðڽ��ϱ�?";
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
                                NotificationManager.Instance.ShowNotification("���Ҹ� �����߽��ϴ�!");
                            }
                            else
                            {
                                NotificationManager.Instance.ShowNotification("���Ҹ� ���� �ڸ��� �����ϴ�.");
                                gameManager.AddMoney(itemToPurchase.itemPrice);
                            }
                            break;
                        case AnimalType.Chicken:
                            if (ChickenCoop.Instance != null)
                            {
                                ChickenCoop.Instance.AddChicken();
                                NotificationManager.Instance.ShowNotification("���� �����߽��ϴ�. ���� ���� �þ�ϴ�.");
                            }
                            else
                            {
                                NotificationManager.Instance.ShowNotification("������ �����ϴ�. ���� ������ �����ϼ���!");
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
                        NotificationManager.Instance.ShowNotification(itemToPurchase.itemName + "��(��) �����߽��ϴ�. ���忡 ��ġ�Ǿ����ϴ�!");
                    }
                    else
                    {
                        NotificationManager.Instance.ShowNotification("�ǹ��� ���� �ڸ��� �����ϴ�.");
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
                                NotificationManager.Instance.ShowNotification("���� �����߽��ϴ�!");
                                break;
                            case EquipmentType.Basket:
                                playerInventory.basketLevel = 1;
                                NotificationManager.Instance.ShowNotification("�ٱ��ϸ� �����߽��ϴ�!");
                                break;
                            case EquipmentType.Milker:
                                playerInventory.milkerLevel = 1;
                                NotificationManager.Instance.ShowNotification("�����⸦ �����߽��ϴ�!");
                                break;
                        }
                    }
                    break;
                case ItemType.Consumable:
                    playerInventory.AddBullets(itemToPurchase.consumableData.amount);
                    NotificationManager.Instance.ShowNotification(itemToPurchase.itemName + "��(��) �����߽��ϴ�.");
                    break;
                case ItemType.Upgrade:
                    if (itemToPurchase.upgradeData is BasketUpgradeData)
                    {
                        BasketUpgradeData data = itemToPurchase.upgradeData as BasketUpgradeData;
                        playerInventory.basketLevel++;
                        playerInventory.basketCapacity = data.upgradeLevels[playerInventory.basketLevel - 1].capacity;
                        NotificationManager.Instance.ShowNotification("�ٱ��ϰ� ���� " + playerInventory.basketLevel + "�� ���׷��̵� �Ǿ����ϴ�!");
                    }
                    else if (itemToPurchase.upgradeData is MilkerUpgradeData)
                    {
                        MilkerUpgradeData data = itemToPurchase.upgradeData as MilkerUpgradeData;
                        playerInventory.milkerLevel++;
                        playerInventory.milkerCapacity = data.upgradeLevels[playerInventory.milkerLevel - 1].capacity;
                        playerInventory.milkerSpeed = data.upgradeLevels[playerInventory.milkerLevel - 1].speed;
                        NotificationManager.Instance.ShowNotification("�����Ⱑ ���� " + playerInventory.milkerLevel + "�� ���׷��̵� �Ǿ����ϴ�!");
                    }
                    else if (itemToPurchase.upgradeData is GunUpgradeData)
                    {
                        GunUpgradeData data = itemToPurchase.upgradeData as GunUpgradeData;
                        playerInventory.gunLevel++;
                        playerInventory.gunDamage = data.upgradeLevels[playerInventory.gunLevel - 1].damageIncrease;
                        playerInventory.gunFireRate = data.upgradeLevels[playerInventory.gunLevel - 1].fireRateIncrease;
                        NotificationManager.Instance.ShowNotification("���� ���� " + playerInventory.gunLevel + "�� ���׷��̵� �Ǿ����ϴ�!");
                    }
                    break;
            }
        }
        else
        {
            NotificationManager.Instance.ShowNotification("���� �ݾ��� �����մϴ�.");
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
        NotificationManager.Instance.ShowNotification(animalToSell.animalData.animalName + "��(��) " + sellPrice + "���� �Ǹ��߽��ϴ�!");
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
                NotificationManager.Instance.ShowNotification($"�� 1������ {sellPrice}���� �Ǹ��߽��ϴ�. ���� ��: {chickenCoop.numberOfChickens}����");
            }
        }
        PopulateSellItems();
    }
}