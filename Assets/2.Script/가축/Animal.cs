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
    }

    void Update()
    {
        if (production != null && animalUI != null)
        {
            animalUI.UpdateProductionGauge(production.currentProductionCount, production.productionMax);
        }
    }
    // 수정된 OnMouseDown() 함수
    void OnMouseDown()
    {
        // 착유기를 착용했는지 확인
        if (EquipmentManager.Instance.currentEquipment == EquipmentType.Milker)
        {
            if (production.currentProductionCount > 0)
            {
                // 'freshness' 객체 안의 'currentFreshness' 값을 전달
                int milkTransferred = PlayerInventory.Instance.AddMilk(freshness.currentFreshness);

                // 실제로 우유가 옮겨진 경우에만 젖소의 우유 생산량을 1 감소
                if (milkTransferred > 0)
                {
                    production.currentProductionCount--;
                    NotificationManager.Instance.ShowNotification(gameObject.name + "의 우유를 수거했습니다. 남은 양: " + production.currentProductionCount);
                }
            }
        }
    }
}