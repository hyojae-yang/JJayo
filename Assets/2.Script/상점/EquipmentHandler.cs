using UnityEngine;
using System.Collections.Generic;

public class EquipmentHandler : MonoBehaviour
{
    private GameData gameData;
    public UpgradeHandler upgradeHandler; // UpgradeHandler 참조 추가

    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            gameData = GameManager.Instance.gameData;
        }
        // UpgradeHandler를 찾아 연결
        upgradeHandler = FindFirstObjectByType<UpgradeHandler>();

        // ★★★ 추가된 부분: 게임 시작 시 바구니와 착유기를 소유한 상태로 설정합니다. ★★★
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

    // 장비 아이템 구매 가능 여부를 확인합니다.
    public bool CanBuy(EquipmentData equipmentData)
    {
        if (gameData == null)
        {
            Debug.LogError("GameData가 로드되지 않았습니다.");
            return false;
        }

        return !gameData.ownedEquipmentIds.Contains(equipmentData.id);
    }

    // 장비 아이템 구매를 처리합니다.
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

        NotificationManager.Instance.ShowNotification(equipmentData.equipmentType + "을(를) 구매했습니다!");
    }
}