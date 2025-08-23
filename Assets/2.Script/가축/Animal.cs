using UnityEngine;

public class Animal : MonoBehaviour
{
    // === �ٸ� ��ũ��Ʈ ������Ʈ�� ===
    public AnimalData animalData;
    private Production production;
    private Freshness freshness;
    private AnimalUI animalUI;

    void Awake()
    {
        production = GetComponent<Production>();
        freshness = GetComponent<Freshness>();
        animalUI = GetComponent<AnimalUI>();
    }

    void Update()
    {
        if (production != null && animalUI != null)
        {
            animalUI.UpdateProductionGauge(production.currentProductionCount, production.productionMax);
        }
    }
    // ������ OnMouseDown() �Լ�
    void OnMouseDown()
    {
        // �����⸦ �����ߴ��� Ȯ��
        if (EquipmentManager.Instance.currentEquipment == EquipmentType.Milker)
        {
            if (production.currentProductionCount > 0)
            {
                // 'freshness' ��ü ���� 'currentFreshness' ���� ����
                int milkTransferred = PlayerInventory.Instance.AddMilk(freshness.currentFreshness);

                // ������ ������ �Ű��� ��쿡�� ������ ���� ���귮�� 1 ����
                if (milkTransferred > 0)
                {
                    production.currentProductionCount--;
                    NotificationManager.Instance.ShowNotification(gameObject.name + "�� ������ �����߽��ϴ�. ���� ��: " + production.currentProductionCount);
                }
            }
        }
    }
}