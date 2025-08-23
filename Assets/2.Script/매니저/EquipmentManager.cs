using UnityEngine;

// ��� ������ ��Ÿ���� Enum
public enum EquipmentType
{
    None,       // �ƹ��͵� �������� ����
    Basket,     // �ٱ���
    Milker,     // ������
    Gun         // ���� �߰��մϴ�!
}

public class EquipmentManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static EquipmentManager Instance { get; private set; }

    [Header("���� ��� ����")]
    [Tooltip("���� �÷��̾ �����ϰ� �ִ� ���.")]
    public EquipmentType currentEquipment = EquipmentType.None;

    private void Awake()
    {
        // �̱��� �ν��Ͻ� ����
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
    /// ��� ��ü�մϴ�.
    /// </summary>
    /// <param name="newEquipment">���� ������ ���</param>
    public void Equip(EquipmentType newEquipment)
    {
        currentEquipment = newEquipment;

        // Enum ���� �ѱ۷� ��ȯ�Ͽ� �˸�â�� ǥ��
        string equipmentName = GetEquipmentName(newEquipment);
        NotificationManager.Instance.ShowNotification($"��� {equipmentName}�� ��ü�߽��ϴ�.");
    }

    /// <summary>
    /// ��� Enum ���� �ѱ� �̸����� ��ȯ�մϴ�.
    /// </summary>
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