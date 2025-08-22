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

    // 이 변수가 추가되었습니다.
    public AnimalType animalType;
}