using UnityEngine;

// 장비 종류를 나타내는 Enum
public enum EquipmentType
{
    None,       // 아무것도 장착하지 않음
    Basket,     // 바구니
    Milker      // 착유기
}

public class EquipmentManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static EquipmentManager Instance { get; private set; }

    [Header("현재 장비 상태")]
    [Tooltip("현재 플레이어가 착용하고 있는 장비.")]
    public EquipmentType currentEquipment = EquipmentType.None;

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 장비를 교체합니다.
    /// </summary>
    /// <param name="newEquipment">새로 착용할 장비</param>
    public void Equip(EquipmentType newEquipment)
    {
        currentEquipment = newEquipment;
        Debug.Log($"장비를 {newEquipment}로 교체했습니다.");
    }
}