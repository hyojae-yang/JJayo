using UnityEngine;

public enum EquipmentType
{
    None,
    Basket,
    Milker,
    Gun
}

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance { get; private set; }

    private void Awake()
    {
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

    public EquipmentType GetCurrentEquipment()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameData != null)
        {
            return GameManager.Instance.gameData.currentEquipment;
        }
        return EquipmentType.None;
    }

    public void Equip(EquipmentType newEquipment)
    {
        if (GameManager.Instance != null && GameManager.Instance.gameData != null)
        {
            GameManager.Instance.gameData.currentEquipment = newEquipment;
            GameManager.Instance.SaveGame();
        }

        string equipmentName = GetEquipmentName(newEquipment);
        NotificationManager.Instance.ShowNotification($"��� {equipmentName}�� ��ü�߽��ϴ�.");
    }

    public string GetEquipmentName(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.Basket:
                return "�ٱ���";
            case EquipmentType.Milker:
                return "������";
            case EquipmentType.Gun:
                return "��";
            default:
                return "�Ǽ�";
        }
    }
}