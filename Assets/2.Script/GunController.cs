using UnityEngine;

public class GunController : MonoBehaviour
{
    void Update()
    {
        if (EquipmentManager.Instance.GetCurrentEquipment() == EquipmentType.Gun)
        {
            // 이제 마우스 클릭 로직은 Wolf.cs의 OnMouseDown에서 처리되므로, 이 스크립트의 역할은 단순히 총을 장착했는지 확인하는 것으로 축소됩니다.
            // 총기 사운드나 시각 효과 로직은 여기에 추가할 수 있습니다.
        }
    }
}