using UnityEngine;
using System.Collections.Generic;

public class EquipmentHandler : MonoBehaviour
{
    private GameData gameData;

    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            gameData = GameManager.Instance.gameData;
        }
    }

    // ��� ������ ���� ���� ���θ� Ȯ���մϴ�.
    // ���� GameData�� ����Ʈ�� Ȯ���Ͽ� �̹� ������ ������� �˻��մϴ�.
    public bool CanBuy(EquipmentData equipmentData)
    {
        if (gameData == null)
        {
            Debug.LogError("GameData�� �ε���� �ʾҽ��ϴ�.");
            return false;
        }

        // ��� �������� ID�� GameData�� ownedEquipmentIds ����Ʈ�� ���ԵǾ� �ִ��� Ȯ���մϴ�.
        // ���ԵǾ� �ִٸ� �� �̻� ������ �� �����ϴ�.
        return !gameData.ownedEquipmentIds.Contains(equipmentData.id);
    }

    // ��� ������ ���Ÿ� ó���մϴ�.
    // ���� ���� �� GameData�� ����Ʈ�� ������ ID�� �߰��մϴ�.
    public void Purchase(EquipmentData equipmentData)
    {
        if (gameData == null)
        {
            Debug.LogError("GameData�� �ε���� �ʾҽ��ϴ�.");
            return;
        }

        // ������ ����� ID�� ����Ʈ�� �߰��մϴ�.
        // ���� hasGun, basketLevel ���� ���� ������ ������� �ʽ��ϴ�.
        gameData.ownedEquipmentIds.Add(equipmentData.id);

        NotificationManager.Instance.ShowNotification(equipmentData.equipmentType + "��(��) �����߽��ϴ�!");
    }
}