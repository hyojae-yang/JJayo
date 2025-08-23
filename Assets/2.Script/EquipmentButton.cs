using UnityEngine;
using TMPro;

public class EquipmentButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public PlayerInventory playerInventory; // PlayerInventory를 참조할 변수

    // 장비 순서를 정의합니다.
    private EquipmentType[] equipmentOrder = { EquipmentType.Basket, EquipmentType.Milker, EquipmentType.Gun };
    private int currentIndex = 0;

    void Start()
    {
        // 게임 시작 시 현재 장비에 따라 인덱스 및 버튼 텍스트 초기화
        UpdateCurrentIndex();
        UpdateButtonText();
    }

    // 장비 매니저의 현재 장비를 기준으로 인덱스를 설정합니다.
    private void UpdateCurrentIndex()
    {
        for (int i = 0; i < equipmentOrder.Length; i++)
        {
            if (equipmentOrder[i] == EquipmentManager.Instance.currentEquipment)
            {
                currentIndex = i;
                return;
            }
        }
        currentIndex = 0; // 매칭되는 장비가 없을 경우 기본값으로 설정
    }

    public void OnClickChangeEquipment()
    {
        do
        {
            // 다음 장비로 인덱스 이동
            currentIndex = (currentIndex + 1) % equipmentOrder.Length;

            EquipmentType nextEquipment = equipmentOrder[currentIndex];

            // 바구니인데 플레이어가 바구니를 소유하고 있지 않다면 건너뜁니다.
            if (nextEquipment == EquipmentType.Basket && playerInventory.basketLevel == 0)
            {
                continue;
            }

            // 착유기인데 플레이어가 착유기를 소유하고 있지 않다면 건너뜁니다.
            if (nextEquipment == EquipmentType.Milker && playerInventory.milkerLevel == 0)
            {
                continue;
            }

            // 총 장비인데 플레이어가 아직 총이 없다면 건너뜁니다.
            if (nextEquipment == EquipmentType.Gun && !playerInventory.hasGun)
            {
                continue;
            }

            // 유효한 장비를 찾았으면 장착합니다.
            EquipmentManager.Instance.Equip(nextEquipment);
            UpdateButtonText();
            return;

        } while (true);
    }

    private void UpdateButtonText()
    {
        if (buttonText == null)
        {
            Debug.LogWarning("Button text (TextMeshProUGUI) is not assigned!");
            return;
        }

        // 현재 장비의 이름을 한글로 가져와 버튼 텍스트를 변경
        buttonText.text = EquipmentManager.Instance.GetEquipmentName(EquipmentManager.Instance.currentEquipment);
    }
}