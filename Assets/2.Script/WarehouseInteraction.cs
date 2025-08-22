using UnityEngine;

public class WarehouseInteraction : MonoBehaviour
{
    /// <summary>
    /// 창고 오브젝트를 클릭하면 인벤토리의 모든 생산물을 창고로 옮깁니다.
    /// 이 함수는 추후 UI 버튼 클릭 시에도 사용할 수 있습니다.
    /// </summary>
    void OnMouseDown()
    {
        // PlayerInventory의 싱글톤 인스턴스를 통해 직접 접근합니다.
        // 이 방법은 PlayerInventory 스크립트가 어디에 붙어있든 상관없이 작동합니다.
        if (PlayerInventory.Instance != null)
        {
            Debug.Log("창고로 옮기는 중...");
            PlayerInventory.Instance.TransferToWarehouse();
        }
        else
        {
            Debug.LogError("PlayerInventory 인스턴스를 찾을 수 없습니다!");
        }
    }
}