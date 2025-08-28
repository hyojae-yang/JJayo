using UnityEngine;
using System.Collections.Generic;

public class EquipmentHandler : MonoBehaviour
{
    private GameData gameData;
    public UpgradeHandler upgradeHandler; // UpgradeHandler ���� �߰�

    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            gameData = GameManager.Instance.gameData;
        }
        // UpgradeHandler�� ã�� ����
        upgradeHandler = FindObjectOfType<UpgradeHandler>();
    }

    // ��� ������ ���� ���� ���θ� Ȯ���մϴ�.
    public bool CanBuy(EquipmentData equipmentData)
    {
        if (gameData == null)
        {
            Debug.LogError("GameData�� �ε���� �ʾҽ��ϴ�.");
            return false;
        }

        return !gameData.ownedEquipmentIds.Contains(equipmentData.id);
    }

    // ��� ������ ���Ÿ� ó���մϴ�.
    public void Purchase(EquipmentData equipmentData)
    {
        if (gameData == null)
        {
            Debug.LogError("GameData�� �ε���� �ʾҽ��ϴ�.");
            return;
        }

        gameData.ownedEquipmentIds.Add(equipmentData.id);

        // �ڡڡ� ������ ��� ���� ���׷��̵� ������ �ʱ�ȭ�ϴ� ���� �ڡڡ�
        if (equipmentData.id == "Gun")
        {
            upgradeHandler.InitializeLevel(UpgradeType.Gun);
        }
        else if (equipmentData.id == "Basket")
        {
            upgradeHandler.InitializeLevel(UpgradeType.Basket);
        }
        else if (equipmentData.id == "Milker")
        {
            upgradeHandler.InitializeLevel(UpgradeType.Milker);
        }

        NotificationManager.Instance.ShowNotification(equipmentData.equipmentType + "��(��) �����߽��ϴ�!");
    }
}