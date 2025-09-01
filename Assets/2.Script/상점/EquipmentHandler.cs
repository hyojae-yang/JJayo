using UnityEngine;
using System.Collections.Generic;

public class EquipmentHandler : MonoBehaviour
{
    private static EquipmentHandler m_instance;
    public static EquipmentHandler Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindFirstObjectByType<EquipmentHandler>();
            }
            return m_instance;
        }
    }

    private GameData gameData;

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

        if (gameData != null)
        {
            if (!gameData.ownedEquipmentIds.Contains("Basket"))
            {
                gameData.ownedEquipmentIds.Add("Basket");
            }
            if (!gameData.ownedEquipmentIds.Contains("Milker"))
            {
                gameData.ownedEquipmentIds.Add("Milker");
            }
        }
    }

    public bool CanBuy(EquipmentData equipmentData)
    {
        if (gameData == null)
        {
            Debug.LogError("GameData가 로드되지 않았습니다.");
            return false;
        }

        return !gameData.ownedEquipmentIds.Contains(equipmentData.id);
    }

    public void Purchase(EquipmentData equipmentData)
    {
        if (gameData == null)
        {
            Debug.LogError("GameData가 로드되지 않았습니다.");
            return;
        }

        gameData.ownedEquipmentIds.Add(equipmentData.id);

        if (equipmentData.id == "Gun")
        {
            if (UpgradeHandler.Instance != null) UpgradeHandler.Instance.InitializeLevel(UpgradeType.Gun);
        }
        else if (equipmentData.id == "Basket")
        {
            if (UpgradeHandler.Instance != null) UpgradeHandler.Instance.InitializeLevel(UpgradeType.Basket);
        }
        else if (equipmentData.id == "Milker")
        {
            if (UpgradeHandler.Instance != null) UpgradeHandler.Instance.InitializeLevel(UpgradeType.Milker);
        }

        if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification(equipmentData.equipmentType + "을(를) 구매했습니다!");
    }
}