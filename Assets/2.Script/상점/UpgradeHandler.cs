using UnityEngine;
using System.Collections.Generic;

// 업그레이드 타입을 명시하기 위한 Enum
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

    // ★★★ 요청에 따라 다시 추가된 InitializeLevel 메서드 ★★★
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
                // 목초지 레벨은 장비 구매가 아닌 업그레이드 자체로 관리되므로
                // 이 부분은 비워두거나 다른 로직을 추가할 수 있습니다.
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
            NotificationManager.Instance.ShowNotification("바구니가 업그레이드 되었습니다!");
        }
        else if (upgradeData is MilkerUpgradeData)
        {
            gameData.milkerLevel++;
            NotificationManager.Instance.ShowNotification("착유기가 업그레이드 되었습니다!");
        }
        else if (upgradeData is GunUpgradeData)
        {
            gameData.gunLevel++;
            NotificationManager.Instance.ShowNotification("총이 업그레이드 되었습니다!");
        }
        else if (upgradeData is PastureUpgradeData)
        {
            gameData.pastureLevel++;
            if (pastureManager != null)
            {
                pastureManager.UpdateVisuals();
            }
            NotificationManager.Instance.ShowNotification("목초가 레벨 " + gameData.pastureLevel + "로 업그레이드 되었습니다!");
        }
    }
}