using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public Image itemIcon;
    public Button actionButton;

    // 구매용 아이템 카드 설정
    public void SetupBuyItem(ShopManager shopManager, PurchasableItemData itemData)
    {
        itemNameText.text = itemData.itemName;
        itemPriceText.text = itemData.itemPrice.ToString("C0");
        itemIcon.sprite = itemData.itemIcon;

        // 버튼 텍스트를 "구매"로 명시적으로 설정
        actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "구매";
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => shopManager.OnClickBuy(itemData));
    }

    // 판매용 아이템 카드 설정
    public void SetupSellItem(ShopManager shopManager, Animal animalToSell)
    {
        itemNameText.text = animalToSell.animalData.animalName;
        itemPriceText.text = (animalToSell.animalData.animalPrice / 2).ToString("C0");
        itemIcon.sprite = animalToSell.animalData.animalIcon;

        // 버튼 텍스트를 "판매"로 명시적으로 설정
        actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "판매";
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => shopManager.SellItem(animalToSell));
    }
}