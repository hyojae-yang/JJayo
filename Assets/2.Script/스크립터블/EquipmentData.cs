using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Data", menuName = "Item Data/Equipment")]
public class EquipmentData : ScriptableObject
{
    // ����� ���� ID�� ������ ����
    public string id;

    public EquipmentType equipmentType;
}