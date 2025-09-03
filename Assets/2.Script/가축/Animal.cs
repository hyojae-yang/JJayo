using UnityEngine;
using System.Collections.Generic;

public class Animal : MonoBehaviour
{
    public float health = 100f;

    public AnimalData animalData;
    private Production production;
    // Freshness 스크립트가 제거되었으므로 변수도 삭제
    // private Freshness freshness;
    private AnimalUI animalUI;

    // 기존 Awake() 메서드는 삭제

    // AnimalHandler가 호출하는 초기화 메서드
    public void Initialize(AnimalData data)
    {
        this.animalData = data;

        production = GetComponent<Production>();
        animalUI = GetComponent<AnimalUI>();

        if (production != null && this.animalData != null)
        {
            production.productionTime = this.animalData.productionInterval;
            production.productionMax = this.animalData.maxProductionCount;
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
                // Production 스크립트에 Freshness 기능이 통합되었으므로
                // production.currentFreshness를 직접 사용합니다.
                if (production != null)
                {
                    int milkToCollect = Mathf.Min(production.currentProductionCount, PlayerInventory.Instance.MilkingYield);
                    int collectedCount = PlayerInventory.Instance.AddMilk(milkToCollect, production.currentFreshness);
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