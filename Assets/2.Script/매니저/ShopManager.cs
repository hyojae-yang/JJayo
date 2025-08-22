using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("패널 UI")]
    public GameObject shopPanel;
    public GameObject buyPanel;
    public GameObject sellPanel;
    public GameObject confirmationPanel;

    [Header("콘텐츠 UI")]
    public Transform buyContentPanel;
    public Transform sellContentPanel;

    [Header("데이터 및 프리팹")]
    public PurchasableItemData[] shopItems;
    public GameObject uiItemCardPrefab;

    [Header("스폰 포인트")]
    public List<Transform> cowSpawnPoints;
    public List<Transform> buildingSpawnPoints;

    [Header("닭장 관리")]
    public ChickenCoop chickenCoop;

    [Header("알림창 UI 요소")]
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
            Debug.Log("돈이 부족합니다.");
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
            // itemType에 따라 분기 처리
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
                                Debug.Log("젖소를 놓을 자리가 없습니다.");
                                gameManager.AddMoney(itemToPurchase.itemPrice);
                            }
                            break;

                        case AnimalType.Chicken:
                            // 닭장 오브젝트가 있는지 먼저 확인합니다.
                            if (chickenCoop != null)
                            {
                                chickenCoop.AddChicken();
                                Debug.Log("닭을 구매했습니다. 닭장 수가 늘어납니다.");
                            }
                            else
                            {
                                Debug.Log("닭장이 없습니다. 먼저 닭장을 구매하세요!");
                                // 돈을 다시 돌려줍니다.
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
                        Debug.Log(itemToPurchase.itemName + "을(를) 구매했습니다. 목장에 설치되었습니다!");
                    }
                    else
                    {
                        Debug.Log("건물을 놓을 자리가 없습니다.");
                        gameManager.AddMoney(itemToPurchase.itemPrice);
                    }
                    break;

                case ItemType.Equipment:
                    // TODO: PlayerInventory 스크립트에 장비 추가 함수를 만들어 연결
                    Debug.Log(itemToPurchase.itemName + "을(를) 구매했습니다. 인벤토리를 확인하세요!");
                    break;

                case ItemType.Consumable:
                    // TODO: PlayerInventory 스크립트에 소모품 추가 함수를 만들어 연결
                    Debug.Log(itemToPurchase.itemName + "을(를) 구매했습니다.");
                    break;
            }
        }
        else
        {
            Debug.Log("소지 금액이 부족합니다.");
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
        Debug.Log(animalToSell.animalData.animalName + "을(를) " + sellPrice + "원에 판매했습니다!");

        Destroy(animalToSell.gameObject);

        PopulateSellItems();
    }
}