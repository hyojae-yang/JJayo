using UnityEngine;

public class WarehouseInteraction : MonoBehaviour
{
    /// <summary>
    /// â�� ������Ʈ�� Ŭ���ϸ� �κ��丮�� ��� ���깰�� â��� �ű�ϴ�.
    /// �� �Լ��� ���� UI ��ư Ŭ�� �ÿ��� ����� �� �ֽ��ϴ�.
    /// </summary>
    void OnMouseDown()
    {
        // PlayerInventory�� �̱��� �ν��Ͻ��� ���� ���� �����մϴ�.
        // �� ����� PlayerInventory ��ũ��Ʈ�� ��� �پ��ֵ� ������� �۵��մϴ�.
        if (PlayerInventory.Instance != null)
        {
            Debug.Log("â��� �ű�� ��...");
            PlayerInventory.Instance.TransferToWarehouse();
        }
        else
        {
            Debug.LogError("PlayerInventory �ν��Ͻ��� ã�� �� �����ϴ�!");
        }
    }
}