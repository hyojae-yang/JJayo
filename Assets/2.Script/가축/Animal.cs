using UnityEngine;
using System.Collections.Generic;

public class Animal : MonoBehaviour
{
    public float health = 100f; // 현재 체력
    public AnimalData animalData;
    private Production production;
    private AnimalUI animalUI;

    // AnimalHandler가 호출하는 초기화 메서드
    public void Initialize(AnimalData data)
    {
        this.animalData = data;

        // 초기화 시 젖소의 체력도 AnimalData에 기반해 설정할 수 있습니다.
        // 현재는 public float health를 사용하고 있으므로 그대로 둡니다.

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

    public void TakeDamage(float amount, GameObject attacker)
    {
        // 총에 맞는 데미지 로직은 제외하고 늑대 공격만 처리합니다.
        if (attacker != null && attacker.CompareTag("Wolf"))
        {
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

        // AnimalManager 리스트에서 자신을 제거
        if (AnimalManager.Instance != null)
        {
            AnimalManager.Instance.RemoveAnimal(this);
        }

        // 늑대가 마지막으로 공격했는지 확인하고, 늑대가 풀로 돌아가게 처리
        if (lastHitter != null)
        {
            Wolf wolfComponent = lastHitter.GetComponent<Wolf>();
            if (wolfComponent != null)
            {
                // isReturning 변수를 직접 수정하는 대신,
                // OnKillTarget 메서드를 호출하여 늑대의 상태를 관리
                wolfComponent.OnKillTarget();
            }
        }

        // 오브젝트 풀로 돌아가거나 파괴
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