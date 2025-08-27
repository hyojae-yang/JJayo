using UnityEngine;

// 아이템 종류를 정의하는 enum
public enum ItemType
{
    Animal,
    Building,
    Equipment,
    Consumable,
    Upgrade // 업그레이드 아이템 종류 추가
}

[CreateAssetMenu(fileName = "New Purchasable Item", menuName = "Tycoon Game/Purchasable Item Data")]
public class PurchasableItemData : ScriptableObject
{
    public string itemName;
    public int itemPrice;
    public Sprite itemIcon;
    public GameObject itemPrefab;

    // 아이템 종류를 구분하는 변수
    public ItemType itemType;

    // 동물인 경우 추가 데이터 (Animal 타입에만 사용)
    public AnimalData animalData;

    // 건물인 경우 추가 데이터 (Building 타입에만 사용)
    public BuildingData buildingData; // ★★★ 추가된 부분

    // 장비인 경우 추가 데이터 (Equipment 타입에만 사용)
    public EquipmentData equipmentData;

    // 소모품인 경우 추가 데이터 (Consumable 타입에만 사용)
    public ConsumableData consumableData;

    // 업그레이드 아이템인 경우 추가 데이터
    public UpgradeData upgradeData;
}