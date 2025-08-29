using UnityEngine;
using System.Collections.Generic;

public class Animal : MonoBehaviour
{
    public float health = 100f;

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
        if (EquipmentManager.Instance.GetCurrentEquipment() == EquipmentType.Milker)
        {
            if (production.currentProductionCount > 0)
            {
                if (freshness != null)
                {
                    int milkToCollect = Mathf.Min(production.currentProductionCount, PlayerInventory.Instance.MilkingYield);
                    int collectedCount = PlayerInventory.Instance.AddMilk(milkToCollect, freshness.currentFreshness);
                    production.currentProductionCount -= collectedCount;

                    if (collectedCount > 0)
                    {
                        NotificationManager.Instance.ShowNotification(animalData.animalName + "�� ������ �����߽��ϴ�.");
                    }
                }
            }
        }
    }

    public void TakeDamage(float amount, GameObject attacker)
    {
        health -= amount;
        Debug.Log($"{animalData.animalName}��(��) {amount}��ŭ ���ظ� �Ծ����ϴ�. ���� ü��: {health}");

        if (health <= 0)
        {
            Die(attacker);
        }
    }

    private void Die(GameObject lastHitter)
    {
        Debug.Log($"{animalData.animalName}��(��) �׾����ϴ�.");

        // �ڡڡ� ������ �κ�: AnimalManager ����Ʈ���� �����θ� �����մϴ�. �ڡڡ�
        if (AnimalManager.Instance != null)
        {
            AnimalManager.Instance.RemoveAnimal(this);
        }

        if (lastHitter != null)
        {
            Wolf wolfComponent = lastHitter.GetComponent<Wolf>();
            if (wolfComponent != null && wolfComponent.isReturning == false)
            {
                wolfComponent.isReturning = true;
            }
        }

        if (transform.parent != null && transform.parent.GetComponent<ObjectPool>() != null)
        {
            transform.parent.GetComponent<ObjectPool>().ReturnToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}