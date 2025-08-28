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

    // 장비 아이템 구매 가능 여부를 확인합니다.
    // 이제 GameData의 리스트를 확인하여 이미 구매한 장비인지 검사합니다.
    public bool CanBuy(EquipmentData equipmentData)
    {
        if (gameData == null)
        {
            Debug.LogError("GameData가 로드되지 않았습니다.");
            return false;
        }

        // 장비 아이템의 ID가 GameData의 ownedEquipmentIds 리스트에 포함되어 있는지 확인합니다.
        // 포함되어 있다면 더 이상 구매할 수 없습니다.
        return !gameData.ownedEquipmentIds.Contains(equipmentData.id);
    }

    // 장비 아이템 구매를 처리합니다.
    // 구매 성공 시 GameData의 리스트에 아이템 ID를 추가합니다.
    public void Purchase(EquipmentData equipmentData)
    {
        if (gameData == null)
        {
            Debug.LogError("GameData가 로드되지 않았습니다.");
            return;
        }

        // 구매한 장비의 ID를 리스트에 추가합니다.
        // 이제 hasGun, basketLevel 등의 개별 변수는 사용하지 않습니다.
        gameData.ownedEquipmentIds.Add(equipmentData.id);

        NotificationManager.Instance.ShowNotification(equipmentData.equipmentType + "을(를) 구매했습니다!");
    }
}