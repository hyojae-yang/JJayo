using UnityEngine;

// ������ ������ �����ϴ� enum
public enum ItemType
{
    Animal,
    Building,
    Equipment,
    Consumable
}

[CreateAssetMenu(fileName = "New Purchasable Item", menuName = "Tycoon Game/Purchasable Item Data")]
public class PurchasableItemData : ScriptableObject
{
    public string itemName;
    public int itemPrice;
    public Sprite itemIcon;
    public GameObject itemPrefab;

    // ������ ������ �����ϴ� ����
    public ItemType itemType;

    // ������ ��� �߰� ������ (Animal Ÿ�Կ��� ���)
    public AnimalData animalData;
}