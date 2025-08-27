using UnityEngine;

public class EquipmentHandler : MonoBehaviour
{
    private GameData gameData;

    private void Awake()
    {
        // GameManager���� GameData�� ������ �����մϴ�.
        if (GameManager.Instance != null)
        {
            gameData = GameManager.Instance.gameData;
        }
    }

    // ��� ������ ���� ���� ���θ� Ȯ���մϴ�.
    public bool CanBuy(EquipmentData equipmentData)
    {
        if (gameData == null)
        {
            Debug.LogError("GameData�� �ε���� �ʾҽ��ϴ�.");
            return false;
        }

        switch (equipmentData.equipmentType)
        {
            case EquipmentType.Gun:
                // ���� �ϳ��� ���� �� �ֽ��ϴ�.
                return !gameData.hasGun;
            case EquipmentType.Basket:
                // �ٱ��ϴ� �ϳ��� ���� �� �ֽ��ϴ�.
                return gameData.basketLevel == 0;
            case EquipmentType.Milker:
                // ������� �ϳ��� ���� �� �ֽ��ϴ�.
                return gameData.milkerLevel == 0;
            default:
                return false;
        }
    }

    // ��� ������ ���Ÿ� ó���մϴ�.
    public void Purchase(EquipmentData equipmentData)
    {
        if (gameData == null)
        {
            Debug.LogError("GameData�� �ε���� �ʾҽ��ϴ�.");
            return;
        }

        switch (equipmentData.equipmentType)
        {
            case EquipmentType.Gun:
                gameData.hasGun = true;
                gameData.gunLevel = 1;
                NotificationManager.Instance.ShowNotification("���� �����߽��ϴ�!");
                break;
            case EquipmentType.Basket:
                gameData.basketLevel = 1;
                NotificationManager.Instance.ShowNotification("�ٱ��ϸ� �����߽��ϴ�!");
                break;
            case EquipmentType.Milker:
                gameData.milkerLevel = 1;
                NotificationManager.Instance.ShowNotification("�����⸦ �����߽��ϴ�!");
                break;
        }
    }
}