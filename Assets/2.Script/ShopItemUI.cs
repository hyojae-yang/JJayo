using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI ���")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public Button actionButton;

    // �� ������ �����ؾ� �մϴ�.
    private ShopManager shopManager;
    private PurchasableItemData itemData;

    // ���ſ� ������ ī�� ����
    public void SetupBuyItem(ShopManager manager, PurchasableItemData data)
    {
        shopManager = manager;
        // ���޹��� �����͸� Ŭ���� ������ ����
        itemData = data;

        itemNameText.text = data.itemName;
        itemPriceText.text = data.itemPrice.ToString("C0");
        itemIcon.sprite = data.itemIcon;

        // OnClickBuy �Լ� ȣ��
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => shopManager.OnClickBuy(itemData));
    }

    // �Ǹſ� ������ ī�� ����
    public void SetupSellItem(ShopManager manager, Animal animalToSell)
    {
        shopManager = manager;

        itemNameText.text = animalToSell.animalData.animalName;
        itemPriceText.text = (animalToSell.animalData.animalPrice / 2).ToString("C0");
        itemIcon.sprite = animalToSell.animalData.animalIcon;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => shopManager.SellItem(animalToSell));
    }
}