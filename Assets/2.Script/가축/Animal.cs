using UnityEngine;

public class Animal : MonoBehaviour
{
    // ★★★ 새로 추가된 부분: 동물의 체력입니다. ★★★
    public float health = 100f;

    // === 다른 스크립트 컴포넌트들 ===
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
                    // ★★★ 수정된 부분: PlayerInventory의 AddMilk 함수에 맞게 인자를 하나만 전달합니다. ★★★
                    PlayerInventory.Instance.AddMilk(freshness.currentFreshness);

                    // ★★★ 추가: 이제 AddMilk 함수가 리턴 값이 없으므로, 생산 개수 감소는 직접 처리합니다. ★★★
                    production.currentProductionCount--;
                    NotificationManager.Instance.ShowNotification(animalData.animalName + "의 우유를 수거했습니다.");
                }
            }
        }
    }

    // ★★★ 수정: attacker 매개변수를 추가합니다. ★★★
    public void TakeDamage(float amount, GameObject attacker)
    {
        health -= amount;
        Debug.Log($"{animalData.animalName}이(가) {amount}만큼 피해를 입었습니다. 현재 체력: {health}");

        if (health <= 0)
        {
            // ★★★ 수정: 죽을 때 막타를 친 attacker를 Die 메서드에 전달합니다. ★★★
            Die(attacker);
        }
    }

    // ★★★ 수정: 막타를 친 늑대 오브젝트를 매개변수로 받습니다. ★★★
    private void Die(GameObject lastHitter)
    {
        Debug.Log($"{animalData.animalName}이(가) 죽었습니다.");

        // ★★★ 막타를 친 늑대만 풀로 돌려보냅니다. ★★★
        if (lastHitter != null)
        {
            Wolf wolfComponent = lastHitter.GetComponent<Wolf>();
            if (wolfComponent != null && wolfComponent.isReturning == false)
            {
                wolfComponent.isReturning = true; // 늑대에게 이제 돌아가라고 명령
            }
        }

        // ★★★ 젖소는 풀로 돌아갑니다. ★★★
        // 이 오브젝트의 부모(Object Pool)를 찾아서 반환 메서드를 호출합니다.
        if (transform.parent != null && transform.parent.GetComponent<ObjectPool>() != null)
        {
            transform.parent.GetComponent<ObjectPool>().ReturnToPool(gameObject);
        }
        else
        {
            // 부모가 오브젝트 풀이 아닐 경우 그냥 파괴
            Destroy(gameObject);
        }
    }
}