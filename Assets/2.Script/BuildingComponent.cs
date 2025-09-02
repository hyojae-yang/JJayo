using UnityEngine;

// 이 스크립트는 BuildingData를 사용하므로,
// 건물을 생성할 때 프리팹에 이 스크립트와 BuildingData를 연결해야 합니다.
public class BuildingComponent : MonoBehaviour
{
    // 유니티 인스펙터 창에서 BuildingData를 할당합니다.
    public BuildingData buildingData;
}