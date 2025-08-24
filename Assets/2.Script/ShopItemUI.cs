using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public Image itemIcon;
    public Button actionButton;

    // ���ſ� ������ ī�� ����
    public void SetupBuyItem(ShopUI shopUI, PurchasableItemData itemData)
    {
        itemNameText.text = itemData.itemName;
        itemPriceText.text = itemData.itemPrice.ToString("C0");
        itemIcon.sprite = itemData.itemIcon;

        // ��ư �ؽ�Ʈ�� "����"�� ��������� ����
        actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "����";
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => shopUI.OnClickBuy(itemData));
    }

    // �Ǹſ� ������ ī�� ����
    public void SetupSellItem(ShopUI shopUI, Animal animalToSell)
    {
        itemNameText.text = animalToSell.animalData.animalName;
        itemPriceText.text = (animalToSell.animalData.animalPrice / 2).ToString("C0");
        itemIcon.sprite = animalToSell.animalData.animalIcon;

        // ��ư �ؽ�Ʈ�� "�Ǹ�"�� ��������� ����
        actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "�Ǹ�";
        actionButton.onClick.RemoveAllListeners();
        // �� �Ǹ� ��ư Ŭ�� �� ShopUI�� OnClickSell �Լ� ȣ��
        actionButton.onClick.AddListener(() => shopUI.OnClickSell(animalToSell));
    }
}