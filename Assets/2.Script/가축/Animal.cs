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
                        NotificationManager.Instance.ShowNotification(animalData.animalName + "의 우유를 수거했습니다.");
                    }
                }
            }
        }
    }

    public void TakeDamage(float amount, GameObject attacker)
    {
        health -= amount;
        Debug.Log($"{animalData.animalName}이(가) {amount}만큼 피해를 입었습니다. 현재 체력: {health}");

        if (health <= 0)
        {
            Die(attacker);
        }
    }

    private void Die(GameObject lastHitter)
    {
        Debug.Log($"{animalData.animalName}이(가) 죽었습니다.");

        // ★★★ 수정된 부분: AnimalManager 리스트에서 스스로를 제거합니다. ★★★
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