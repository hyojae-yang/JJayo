using UnityEngine;
using System.Collections.Generic;

// ���׷��̵� Ÿ���� ����ϱ� ���� Enum
public enum UpgradeType { Gun, Basket, Milker, Pasture }

public class UpgradeHandler : MonoBehaviour
{
    private GameData gameData;
    private PastureManager pastureManager;

    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            gameData = GameManager.Instance.gameData;
        }
        pastureManager = FindFirstObjectByType<PastureManager>();
    }

    // �ڡڡ� ��û�� ���� �ٽ� �߰��� InitializeLevel �޼��� �ڡڡ�
    public void InitializeLevel(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Gun:
                gameData.gunLevel = 1;
                break;
            case UpgradeType.Basket:
                gameData.basketLevel = 1;
                break;
            case UpgradeType.Milker:
                gameData.milkerLevel = 1;
                break;
            case UpgradeType.Pasture:
                // ������ ������ ��� ���Ű� �ƴ� ���׷��̵� ��ü�� �����ǹǷ�
                // �� �κ��� ����ΰų� �ٸ� ������ �߰��� �� �ֽ��ϴ�.
                break;
        }
    }

    public bool CanBuy(UpgradeData upgradeData)
    {
        if (gameData == null) return false;

        int currentLevel = 0;

        if (upgradeData is BasketUpgradeData) currentLevel = gameData.basketLevel;
        else if (upgradeData is MilkerUpgradeData) currentLevel = gameData.milkerLevel;
        else if (upgradeData is GunUpgradeData) currentLevel = gameData.gunLevel;
        else if (upgradeData is PastureUpgradeData) currentLevel = gameData.pastureLevel;

        if (currentLevel == upgradeData.GetMaxLevel())
        {
            return false;
        }

        return true;
    }

    public void Purchase(UpgradeData upgradeData)
    {
        if (gameData == null) return;

        if (upgradeData is BasketUpgradeData)
        {
            gameData.basketLevel++;
            NotificationManager.Instance.ShowNotification("�ٱ��ϰ� ���׷��̵� �Ǿ����ϴ�!");
        }
        else if (upgradeData is MilkerUpgradeData)
        {
            gameData.milkerLevel++;
            NotificationManager.Instance.ShowNotification("�����Ⱑ ���׷��̵� �Ǿ����ϴ�!");
        }
        else if (upgradeData is GunUpgradeData)
        {
            gameData.gunLevel++;
            NotificationManager.Instance.ShowNotification("���� ���׷��̵� �Ǿ����ϴ�!");
        }
        else if (upgradeData is PastureUpgradeData)
        {
            gameData.pastureLevel++;
            if (pastureManager != null)
            {
                pastureManager.UpdateVisuals();
            }
            NotificationManager.Instance.ShowNotification("���ʰ� ���� " + gameData.pastureLevel + "�� ���׷��̵� �Ǿ����ϴ�!");
        }
    }
}