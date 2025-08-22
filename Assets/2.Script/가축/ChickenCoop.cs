using UnityEngine;

public class ChickenCoop : MonoBehaviour
{
    // ���� ���� ������ (�ν����Ϳ��� �Ҵ�)
    public ChickenCoopData chickenCoopData;

    // ���� ���� ����
    public int currentEggCount = 0;
    public int numberOfChickens;

    // �� ������ ���� Ÿ�̸�
    private float productionTimer = 0f;

    void Start()
    {
        // ���� ���� �� ���� ���� 0���� �ʱ�ȭ
        numberOfChickens = 0;
    }

    void Update()
    {
        // ���� ������ �������� �ʽ��ϴ�.
        if (numberOfChickens <= 0)
        {
            return;
        }

        // ���� ���� ����Ͽ� ���� �ӵ� ����
        productionTimer += Time.deltaTime * numberOfChickens;

        // �� ���� �ֱⰡ �Ǹ� ���� �߰�
        if (productionTimer >= chickenCoopData.eggProductionInterval)
        {
            currentEggCount++;
            Debug.Log($"���忡�� ���� {currentEggCount}�� �����߽��ϴ�!");
            productionTimer = 0f;
        }
    }

    // ���콺�� Ŭ���ϸ� ȣ��Ǵ� �Լ� (�� ����)
    void OnMouseDown()
    {
        // �ٱ��ϸ� �����ߴ��� Ȯ��
        if (EquipmentManager.Instance.currentEquipment == EquipmentType.Basket)
        {
            if (currentEggCount > 0)
            {
                int eggsTransferred = PlayerInventory.Instance.AddEggs(currentEggCount);
                currentEggCount -= eggsTransferred;
                Debug.Log($"�ٱ��Ͽ� �� {eggsTransferred}���� ��ҽ��ϴ�. ���忡 ���� ��: {currentEggCount}");
                productionTimer = 0f;
            }
        }
    }

    /// <summary>
    /// ���� �߰��ϰ� ���� ������ŵ�ϴ�.
    /// </summary>
    public void AddChicken()
    {
        numberOfChickens++;
        Debug.Log("���ο� ���� ���忡 �߰��Ǿ����ϴ�. ���� ���� ��: " + numberOfChickens);
    }
}