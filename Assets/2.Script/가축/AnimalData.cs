using UnityEngine;

public enum AnimalType
{
    Cow,
    Chicken,
    // 다른 동물 타입도 여기에 추가 가능
}

[CreateAssetMenu(fileName = "New Animal", menuName = "Tycoon Game/Animal Data")]

public class AnimalData : ScriptableObject
{
    public string animalName;
    public int animalPrice;
    public Sprite animalIcon;
    public GameObject animalPrefab;
    public float productionInterval;
    public ProductData productData;
    public int maxProductionCount;

    public string animalId;
    public AnimalType animalType;

    // ★★★ 추가된 변수: 이 젖소를 해금하는 데 필요한 명성도 값 ★★★
    public int unlockReputation;

    // ★★★ 추가된 변수: 이 젖소의 기본 신선도 값 (0-50) ★★★
    public float baseFreshness;

    // ★★★ 추가된 변수: 이 동물의 최대 체력 값 ★★★
    [Tooltip("이 동물의 최대 체력입니다.")]
    public float maxHealth;
}