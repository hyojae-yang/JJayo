using UnityEngine;

[CreateAssetMenu(fileName = "New Building Data", menuName = "Tycoon Game/Building Data")]
public class BuildingData : ScriptableObject
{
    // 건물을 식별할 고유 ID (코드에서 건물을 찾을 때 사용됩니다)
    public string buildingId;

    // 상점에 표시될 건물의 이름
    public string buildingName;

    // 건물의 구매 가격
    public int buildingPrice;

    // 상점에 표시될 건물의 아이콘
    public Sprite buildingIcon;

    // 실제로 게임에 배치될 건물의 프리팹
    public GameObject buildingPrefab;
}