using UnityEngine;
public enum AnimalType
{
    Cow,
    Chicken,
    // �ٸ� ���� Ÿ�Ե� ���⿡ �߰� ����
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

    // �� ������ �߰��Ǿ����ϴ�.
    public AnimalType animalType;
}