using UnityEngine;
using System.Collections.Generic;

public class Animal : MonoBehaviour
{
    // ★★★ 추가된 코드: 늑대 공격 이벤트를 위한 이벤트 선언 ★★★
    public static event System.Action<Transform> OnCowAttackedByWolf;

    private float health;
    public AnimalData animalData;
    private Production production;
    private AnimalUI animalUI;

    public void Initialize(AnimalData data)
    {
        this.animalData = data;
        health = this.animalData.maxHealth;
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

    // ★★★ 수정된 메서드: 공격자가 늑대일 경우 이벤트를 발생시킵니다. ★★★
    public void TakeDamage(float amount, GameObject attacker)
    {
        if (attacker != null && attacker.CompareTag("Wolf"))
        {
            // 감시탑에 공격당하고 있다고 알립니다.
            OnCowAttackedByWolf?.Invoke(attacker.transform);

            health -= amount;

            if (health <= 0)
            {
                Die(attacker);
            }
        }
    }

    private void Die(GameObject lastHitter)
    {
        NotificationManager.Instance.ShowNotification($"{animalData.animalName}이(가) 죽었습니다.");

        if (AnimalManager.Instance != null)
        {
            AnimalManager.Instance.RemoveAnimal(this);
        }

        if (lastHitter != null)
        {
            Wolf wolfComponent = lastHitter.GetComponent<Wolf>();
            if (wolfComponent != null)
            {
                wolfComponent.OnKillTarget();
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