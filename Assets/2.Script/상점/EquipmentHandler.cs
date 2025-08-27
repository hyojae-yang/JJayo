using UnityEngine;

public class EquipmentHandler : MonoBehaviour
{
    private GameData gameData;

    private void Awake()
    {
        // GameManager에서 GameData를 가져와 연결합니다.
        if (GameManager.Instance != null)
        {
            gameData = GameManager.Instance.gameData;
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

        switch (equipmentData.equipmentType)
        {
            case EquipmentType.Gun:
                // 총은 하나만 가질 수 있습니다.
                return !gameData.hasGun;
            case EquipmentType.Basket:
                // 바구니는 하나만 가질 수 있습니다.
                return gameData.basketLevel == 0;
            case EquipmentType.Milker:
                // 착유기는 하나만 가질 수 있습니다.
                return gameData.milkerLevel == 0;
            default:
                return false;
        }
    }

    // 장비 아이템 구매를 처리합니다.
    public void Purchase(EquipmentData equipmentData)
    {
        if (gameData == null)
        {
            Debug.LogError("GameData가 로드되지 않았습니다.");
            return;
        }

        switch (equipmentData.equipmentType)
        {
            case EquipmentType.Gun:
                gameData.hasGun = true;
                gameData.gunLevel = 1;
                NotificationManager.Instance.ShowNotification("총을 구매했습니다!");
                break;
            case EquipmentType.Basket:
                gameData.basketLevel = 1;
                NotificationManager.Instance.ShowNotification("바구니를 구매했습니다!");
                break;
            case EquipmentType.Milker:
                gameData.milkerLevel = 1;
                NotificationManager.Instance.ShowNotification("착유기를 구매했습니다!");
                break;
        }
    }
}