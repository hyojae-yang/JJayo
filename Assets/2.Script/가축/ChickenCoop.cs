using UnityEngine;

public class ChickenCoop : MonoBehaviour
{
    // 닭장 고유 데이터 (인스펙터에서 할당)
    public ChickenCoopData chickenCoopData;

    // 현재 닭장 상태
    public int currentEggCount = 0;
    public int numberOfChickens;

    // 알 생산을 위한 타이머
    private float productionTimer = 0f;

    void Start()
    {
        // 게임 시작 시 닭의 수를 0으로 초기화
        numberOfChickens = 0;
    }

    void Update()
    {
        // 닭이 없으면 생산하지 않습니다.
        if (numberOfChickens <= 0)
        {
            return;
        }

        // 닭의 수에 비례하여 생산 속도 증가
        productionTimer += Time.deltaTime * numberOfChickens;

        // 알 생산 주기가 되면 알을 추가
        if (productionTimer >= chickenCoopData.eggProductionInterval)
        {
            currentEggCount++;
            Debug.Log($"닭장에서 알을 {currentEggCount}개 생산했습니다!");
            productionTimer = 0f;
        }
    }

    // 마우스를 클릭하면 호출되는 함수 (알 수거)
    void OnMouseDown()
    {
        // 바구니를 착용했는지 확인
        if (EquipmentManager.Instance.currentEquipment == EquipmentType.Basket)
        {
            if (currentEggCount > 0)
            {
                int eggsTransferred = PlayerInventory.Instance.AddEggs(currentEggCount);
                currentEggCount -= eggsTransferred;
                Debug.Log($"바구니에 알 {eggsTransferred}개를 담았습니다. 닭장에 남은 알: {currentEggCount}");
                productionTimer = 0f;
            }
        }
    }

    /// <summary>
    /// 닭을 추가하고 수를 증가시킵니다.
    /// </summary>
    public void AddChicken()
    {
        numberOfChickens++;
        Debug.Log("새로운 닭이 닭장에 추가되었습니다. 현재 닭의 수: " + numberOfChickens);
    }
}