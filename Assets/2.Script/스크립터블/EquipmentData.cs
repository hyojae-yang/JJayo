using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Data", menuName = "Item Data/Equipment")]
public class EquipmentData : ScriptableObject
{
    // 장비의 고유 ID를 저장할 변수
    public string id;

    public EquipmentType equipmentType;
}