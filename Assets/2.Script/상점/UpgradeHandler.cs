using UnityEngine;
using System.Collections.Generic;

// 업그레이드 타입을 명시하기 위한 Enum
public enum UpgradeType { Gun, Basket, Milker, Pasture, Warehouse } // ★★★ Warehouse 추가 ★★★

public class UpgradeHandler : MonoBehaviour
{
    private static UpgradeHandler m_instance;
    public static UpgradeHandler Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindFirstObjectByType<UpgradeHandler>();
            }
            return m_instance;
        }
    }

    private GameData gameData;
    private PastureManager pastureManager;


    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        if (GameManager.Instance != null)
        {
            gameData = GameManager.Instance.CurrentGameData;
        }
        pastureManager = PastureManager.Instance;
    }

    public void InitializeLevel(UpgradeType type)
    {
        if (gameData == null) return;

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
                break;
            case UpgradeType.Warehouse: // ★★★ 새로 추가된 부분 ★★★
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
        else if (upgradeData is WarehouseUpgradeData) currentLevel = gameData.warehouseLevel; // ★★★ 새로 추가된 부분 ★★★

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
            if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("바구니가 업그레이드 되었습니다!");
        }
        else if (upgradeData is MilkerUpgradeData)
        {
            gameData.milkerLevel++;
            if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("착유기가 업그레이드 되었습니다!");
        }
        else if (upgradeData is GunUpgradeData)
        {
            gameData.gunLevel++;
            if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("총이 업그레이드 되었습니다!");
        }
        else if (upgradeData is PastureUpgradeData)
        {
            gameData.pastureLevel++;
            if (pastureManager != null)
            {
                pastureManager.UpdateVisuals();
            }
            if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("목초가 레벨 " + gameData.pastureLevel + "로 업그레이드 되었습니다!");
        }
        else if (upgradeData is WarehouseUpgradeData) // ★★★ 새로 추가된 부분 ★★★
        {
            gameData.warehouseLevel++;
            if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("창고가 업그레이드 되었습니다!");
        }
    }
}