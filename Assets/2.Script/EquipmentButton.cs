using UnityEngine;
using TMPro;
using System.Linq;

public class EquipmentButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;

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
        // GameManager.Instance.gameData.currentEquipment 대신
        // EquipmentManager.Instance.GetCurrentEquipment()를 사용합니다.
        EquipmentType current = EquipmentManager.Instance.GetCurrentEquipment();
        for (int i = 0; i < equipmentOrder.Length; i++)
        {
            if (equipmentOrder[i] == current)
            {
                currentIndex = i;
                return;
            }
        }
        currentIndex = 0; // 매칭되는 장비가 없을 경우 기본값으로 설정
    }

    public void OnClickChangeEquipment()
    {
        // 현재 레벨 데이터를 가져옵니다.
        var gameData = GameManager.Instance.gameData;
        int initialIndex = currentIndex; // 무한 루프 방지용

        do
        {
            // 다음 장비로 인덱스 이동
            currentIndex = (currentIndex + 1) % equipmentOrder.Length;

            EquipmentType nextEquipment = equipmentOrder[currentIndex];
            if (nextEquipment == EquipmentType.Gun && gameData.gunLevel == 0)
            {
                continue;
            }

            // 모든 장비를 다 돌아도 유효한 장비가 없으면 종료
            if (currentIndex == initialIndex)
            {
                Debug.LogWarning("장착할 수 있는 장비가 없습니다. 튜토리얼을 진행하거나 상점을 확인하세요.");
                return;
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
        buttonText.text = EquipmentManager.Instance.GetEquipmentName(EquipmentManager.Instance.GetCurrentEquipment());
    }
}