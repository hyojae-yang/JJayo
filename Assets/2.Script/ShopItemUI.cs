using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI 요소")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public Button actionButton;

    // 이 변수를 선언해야 합니다.
    private ShopManager shopManager;
    private PurchasableItemData itemData;

    // 구매용 아이템 카드 설정
    public void SetupBuyItem(ShopManager manager, PurchasableItemData data)
    {
        shopManager = manager;
        // 전달받은 데이터를 클래스 변수에 저장
        itemData = data;

        itemNameText.text = data.itemName;
        itemPriceText.text = data.itemPrice.ToString("C0");
        itemIcon.sprite = data.itemIcon;

        // OnClickBuy 함수 호출
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => shopManager.OnClickBuy(itemData));
    }

    // 판매용 아이템 카드 설정
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