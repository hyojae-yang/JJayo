using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("�г� UI")]
    public GameObject shopPanel;
    public GameObject buyPanel;
    public GameObject sellPanel;
    public GameObject confirmationPanel;

    [Header("������ UI")]
    public Transform buyContentPanel;
    public Transform sellContentPanel;

    [Header("������ �� ������")]
    public PurchasableItemData[] shopItems;
    public GameObject uiItemCardPrefab;

    [Header("���� ����Ʈ")]
    public List<Transform> cowSpawnPoints;
    public List<Transform> buildingSpawnPoints;

    [Header("���� ����")]
    public ChickenCoop chickenCoop;

    [Header("�˸�â UI ���")]
    public TextMeshProUGUI confirmText;
    private PurchasableItemData itemToPurchase;

    private PlayerInventory playerInventory;
    private GameManager gameManager;

    void Start()
    {
        shopPanel.SetActive(false);
        buyPanel.SetActive(true);
        sellPanel.SetActive(false);
        confirmationPanel.SetActive(false);

        playerInventory = PlayerInventory.Instance;
        gameManager = GameManager.Instance;

        PopulateBuyItems();
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
            GameObject itemCard = Instantiate(uiItemCardPrefab, buyContentPanel);
            ShopItemUI itemUI = itemCard.GetComponent<ShopItemUI>();
            itemUI.SetupBuyItem(this, data);
        }
    }

    private void PopulateSellItems()
    {
        foreach (Transform child in sellContentPanel)
        {
            Destroy(child.gameObject);
        }

        Animal[] allAnimals = FindObjectsOfType<Animal>();

        foreach (Animal animal in allAnimals)
        {
            GameObject itemCard = Instantiate(uiItemCardPrefab, sellContentPanel);
            ShopItemUI itemUI = itemCard.GetComponent<ShopItemUI>();
            itemUI.SetupSellItem(this, animal);
        }
    }

    public void OnClickBuy(PurchasableItemData itemData)
    {
        if (gameManager.CurrentMoney < itemData.itemPrice)
        {
            Debug.Log("���� �����մϴ�.");
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
            // itemType�� ���� �б� ó��
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
                            }
                            else
                            {
                                Debug.Log("���Ҹ� ���� �ڸ��� �����ϴ�.");
                                gameManager.AddMoney(itemToPurchase.itemPrice);
                            }
                            break;

                        case AnimalType.Chicken:
                            // ���� ������Ʈ�� �ִ��� ���� Ȯ���մϴ�.
                            if (chickenCoop != null)
                            {
                                chickenCoop.AddChicken();
                                Debug.Log("���� �����߽��ϴ�. ���� ���� �þ�ϴ�.");
                            }
                            else
                            {
                                Debug.Log("������ �����ϴ�. ���� ������ �����ϼ���!");
                                // ���� �ٽ� �����ݴϴ�.
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
                        Debug.Log(itemToPurchase.itemName + "��(��) �����߽��ϴ�. ���忡 ��ġ�Ǿ����ϴ�!");
                    }
                    else
                    {
                        Debug.Log("�ǹ��� ���� �ڸ��� �����ϴ�.");
                        gameManager.AddMoney(itemToPurchase.itemPrice);
                    }
                    break;

                case ItemType.Equipment:
                    // TODO: PlayerInventory ��ũ��Ʈ�� ��� �߰� �Լ��� ����� ����
                    Debug.Log(itemToPurchase.itemName + "��(��) �����߽��ϴ�. �κ��丮�� Ȯ���ϼ���!");
                    break;

                case ItemType.Consumable:
                    // TODO: PlayerInventory ��ũ��Ʈ�� �Ҹ�ǰ �߰� �Լ��� ����� ����
                    Debug.Log(itemToPurchase.itemName + "��(��) �����߽��ϴ�.");
                    break;
            }
        }
        else
        {
            Debug.Log("���� �ݾ��� �����մϴ�.");
        }

        confirmationPanel.SetActive(false);
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
        Debug.Log(animalToSell.animalData.animalName + "��(��) " + sellPrice + "���� �Ǹ��߽��ϴ�!");

        Destroy(animalToSell.gameObject);

        PopulateSellItems();
    }
}