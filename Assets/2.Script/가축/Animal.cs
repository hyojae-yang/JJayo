using UnityEngine;

public class Animal : MonoBehaviour
{
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

        // ★★★ AnimalData의 생산 관련 값을 Production에 전달합니다.
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
        // 착유기를 착용했는지 확인
        if (EquipmentManager.Instance.currentEquipment == EquipmentType.Milker)
        {
            if (production.currentProductionCount > 0)
            {
                // 'freshness' 객체가 존재한다면 그 안의 값을 전달
                if (freshness != null)
                {
                    int milkTransferred = PlayerInventory.Instance.AddMilk(freshness.currentFreshness);

                    // 실제로 우유가 옮겨진 경우에만 젖소의 우유 생산량을 1 감소
                    if (milkTransferred > 0)
                    {
                        production.currentProductionCount--;
                        NotificationManager.Instance.ShowNotification(animalData.animalName + "의 우유를 수거했습니다.");
                    }
                }
            }
        }
    }
}