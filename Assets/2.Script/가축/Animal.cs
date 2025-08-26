using UnityEngine;

public class Animal : MonoBehaviour
{
    // �ڡڡ� ���� �߰��� �κ�: ������ ü���Դϴ�. �ڡڡ�
    public float health = 100f;

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
        if (EquipmentManager.Instance.currentEquipment == EquipmentType.Milker)
        {
            if (production.currentProductionCount > 0)
            {
                if (freshness != null)
                {
                    // �ڡڡ� ������ �κ�: PlayerInventory�� AddMilk �Լ��� �°� ���ڸ� �ϳ��� �����մϴ�. �ڡڡ�
                    PlayerInventory.Instance.AddMilk(freshness.currentFreshness);

                    // �ڡڡ� �߰�: ���� AddMilk �Լ��� ���� ���� �����Ƿ�, ���� ���� ���Ҵ� ���� ó���մϴ�. �ڡڡ�
                    production.currentProductionCount--;
                    NotificationManager.Instance.ShowNotification(animalData.animalName + "�� ������ �����߽��ϴ�.");
                }
            }
        }
    }

    // �ڡڡ� ����: attacker �Ű������� �߰��մϴ�. �ڡڡ�
    public void TakeDamage(float amount, GameObject attacker)
    {
        health -= amount;
        Debug.Log($"{animalData.animalName}��(��) {amount}��ŭ ���ظ� �Ծ����ϴ�. ���� ü��: {health}");

        if (health <= 0)
        {
            // �ڡڡ� ����: ���� �� ��Ÿ�� ģ attacker�� Die �޼��忡 �����մϴ�. �ڡڡ�
            Die(attacker);
        }
    }

    // �ڡڡ� ����: ��Ÿ�� ģ ���� ������Ʈ�� �Ű������� �޽��ϴ�. �ڡڡ�
    private void Die(GameObject lastHitter)
    {
        Debug.Log($"{animalData.animalName}��(��) �׾����ϴ�.");

        // �ڡڡ� ��Ÿ�� ģ ���븸 Ǯ�� ���������ϴ�. �ڡڡ�
        if (lastHitter != null)
        {
            Wolf wolfComponent = lastHitter.GetComponent<Wolf>();
            if (wolfComponent != null && wolfComponent.isReturning == false)
            {
                wolfComponent.isReturning = true; // ���뿡�� ���� ���ư���� ���
            }
        }

        // �ڡڡ� ���Ҵ� Ǯ�� ���ư��ϴ�. �ڡڡ�
        // �� ������Ʈ�� �θ�(Object Pool)�� ã�Ƽ� ��ȯ �޼��带 ȣ���մϴ�.
        if (transform.parent != null && transform.parent.GetComponent<ObjectPool>() != null)
        {
            transform.parent.GetComponent<ObjectPool>().ReturnToPool(gameObject);
        }
        else
        {
            // �θ� ������Ʈ Ǯ�� �ƴ� ��� �׳� �ı�
            Destroy(gameObject);
        }
    }
}