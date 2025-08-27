using UnityEngine;
using System.Collections.Generic;

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
        pastureManager = FindObjectOfType<PastureManager>();
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