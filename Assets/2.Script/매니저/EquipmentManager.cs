using UnityEngine;

// ��� ������ ��Ÿ���� Enum
public enum EquipmentType
{
    None,       // �ƹ��͵� �������� ����
    Basket,     // �ٱ���
    Milker      // ������
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
        Debug.Log($"��� {newEquipment}�� ��ü�߽��ϴ�.");
    }
}