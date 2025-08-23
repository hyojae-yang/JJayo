using UnityEngine;
using TMPro;

public class EquipmentButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public PlayerInventory playerInventory; // PlayerInventory�� ������ ����

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
        for (int i = 0; i < equipmentOrder.Length; i++)
        {
            if (equipmentOrder[i] == EquipmentManager.Instance.currentEquipment)
            {
                currentIndex = i;
                return;
            }
        }
        currentIndex = 0; // ��Ī�Ǵ� ��� ���� ��� �⺻������ ����
    }

    public void OnClickChangeEquipment()
    {
        do
        {
            // ���� ���� �ε��� �̵�
            currentIndex = (currentIndex + 1) % equipmentOrder.Length;

            EquipmentType nextEquipment = equipmentOrder[currentIndex];

            // �ٱ����ε� �÷��̾ �ٱ��ϸ� �����ϰ� ���� �ʴٸ� �ǳʶݴϴ�.
            if (nextEquipment == EquipmentType.Basket && playerInventory.basketLevel == 0)
            {
                continue;
            }

            // �������ε� �÷��̾ �����⸦ �����ϰ� ���� �ʴٸ� �ǳʶݴϴ�.
            if (nextEquipment == EquipmentType.Milker && playerInventory.milkerLevel == 0)
            {
                continue;
            }

            // �� ����ε� �÷��̾ ���� ���� ���ٸ� �ǳʶݴϴ�.
            if (nextEquipment == EquipmentType.Gun && !playerInventory.hasGun)
            {
                continue;
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

        // ���� ����� �̸��� �ѱ۷� ������ ��ư �ؽ�Ʈ�� ����
        buttonText.text = EquipmentManager.Instance.GetEquipmentName(EquipmentManager.Instance.currentEquipment);
    }
}