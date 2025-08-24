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

        // �ڡڡ� AnimalData�� ���� ���� ���� Production�� �����մϴ�.
        if (production != null && animalData != null)
        {
            production.productionTime = animalData.productionInterval;
            production.productionMax = animalData.maxProductionCount;
        }
    }

    void Update()
    {
        if (production != null && animalUI != null)
        {
            animalUI.UpdateProductionGauge(production.currentProductionCount, production.productionMax);
        }
    }

    void OnMouseDown()
    {
        // �����⸦ �����ߴ��� Ȯ��
        if (EquipmentManager.Instance.currentEquipment == EquipmentType.Milker)
        {
            if (production.currentProductionCount > 0)
            {
                // 'freshness' ��ü�� �����Ѵٸ� �� ���� ���� ����
                if (freshness != null)
                {
                    int milkTransferred = PlayerInventory.Instance.AddMilk(freshness.currentFreshness);

                    // ������ ������ �Ű��� ��쿡�� ������ ���� ���귮�� 1 ����
                    if (milkTransferred > 0)
                    {
                        production.currentProductionCount--;
                        NotificationManager.Instance.ShowNotification(animalData.animalName + "�� ������ �����߽��ϴ�.");
                    }
                }
            }
        }
    }
}