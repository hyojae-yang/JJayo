using UnityEngine;


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
        if (GameManager.Instance != null && GameManager.Instance.CurrentGameData != null)
        {
            return GameManager.Instance.CurrentGameData.currentEquipment;
        }
        return EquipmentType.None;
    }

    public void Equip(EquipmentType newEquipment)
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentGameData != null)
        {
            // 수정: GameData만 업데이트하고 저장 로직은 제거합니다.
            GameManager.Instance.CurrentGameData.currentEquipment = newEquipment;
        }

        string equipmentName = GetEquipmentName(newEquipment);
        NotificationManager.Instance.ShowNotification($"장비를 {equipmentName}로 교체했습니다.");
    }

    public string GetEquipmentName(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.Basket:
                return "바구니";
            case EquipmentType.Milker:
                return "착유기";
            case EquipmentType.Gun:
                return "총";
            default:
                return "맨손";
        }
    }
}