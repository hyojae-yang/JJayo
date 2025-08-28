using UnityEngine;
using TMPro;
using System.Linq;

public class EquipmentButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;

    // ��� ������ �����մϴ�.
    private EquipmentType[] equipmentOrder = { EquipmentType.Basket, EquipmentType.Milker, EquipmentType.Gun };
    private int currentIndex = 0;

    void Start()
    {
        // ���� ���� �� ���� ��� ���� �ε��� �� ��ư �ؽ�Ʈ �ʱ�ȭ
        UpdateCurrentIndex();
        UpdateButtonText();
    }

    // ��� �Ŵ����� ���� ��� �������� �ε����� �����մϴ�.
    private void UpdateCurrentIndex()
    {
        // GameManager.Instance.gameData.currentEquipment ���
        // EquipmentManager.Instance.GetCurrentEquipment()�� ����մϴ�.
        EquipmentType current = EquipmentManager.Instance.GetCurrentEquipment();
        for (int i = 0; i < equipmentOrder.Length; i++)
        {
            if (equipmentOrder[i] == current)
            {
                currentIndex = i;
                return;
            }
        }
        currentIndex = 0; // ��Ī�Ǵ� ��� ���� ��� �⺻������ ����
    }

    public void OnClickChangeEquipment()
    {
        // ���� ���� �����͸� �����ɴϴ�.
        var gameData = GameManager.Instance.gameData;
        int initialIndex = currentIndex; // ���� ���� ������

        do
        {
            // ���� ���� �ε��� �̵�
            currentIndex = (currentIndex + 1) % equipmentOrder.Length;

            EquipmentType nextEquipment = equipmentOrder[currentIndex];
            if (nextEquipment == EquipmentType.Gun && gameData.gunLevel == 0)
            {
                continue;
            }

            // ��� ��� �� ���Ƶ� ��ȿ�� ��� ������ ����
            if (currentIndex == initialIndex)
            {
                Debug.LogWarning("������ �� �ִ� ��� �����ϴ�. Ʃ�丮���� �����ϰų� ������ Ȯ���ϼ���.");
                return;
            }

            // ��ȿ�� ��� ã������ �����մϴ�.
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