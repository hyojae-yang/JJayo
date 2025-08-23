using UnityEngine;

public class ChickenCoop : MonoBehaviour
{
    // 싱글턴 인스턴스 (게임 내에서 유일한 닭장 오브젝트를 참조)
    public static ChickenCoop Instance { get; private set; }

    [Header("닭장 고유 데이터")]
    public ChickenCoopData chickenCoopData;

    [Header("현재 닭장 상태")]
    public int currentEggCount = 0;
    public int numberOfChickens;

    // 알 생산을 위한 타이머
    private float productionTimer = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Start에서는 닭의 수를 0으로 초기화하지 않도록 수정했습니다.
        // numberOfChickens = 0;
    }

    void Update()
    {
        if (numberOfChickens <= 0)
        {
            return;
        }

        productionTimer += Time.deltaTime * numberOfChickens;

        if (productionTimer >= chickenCoopData.eggProductionInterval)
        {
            currentEggCount++;
            NotificationManager.Instance.ShowNotification($"닭장에서 알을 {currentEggCount}개 생산했습니다!");
            productionTimer = 0f;
        }
    }

    // 마우스를 클릭하면 호출되는 함수 (알 수거)
    void OnMouseDown()
    {
        // 바구니를 착용했는지 확인하고 알을 수거합니다.
        if (EquipmentManager.Instance.currentEquipment == EquipmentType.Basket)
        {
            if (currentEggCount > 0)
            {
                int eggsTransferred = PlayerInventory.Instance.AddEggs(currentEggCount);
                currentEggCount -= eggsTransferred;
                NotificationManager.Instance.ShowNotification($"바구니에 알 {eggsTransferred}개를 담았습니다. 닭장에 남은 알: {currentEggCount}");
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
        NotificationManager.Instance.ShowNotification("새로운 닭이 닭장에 추가되었습니다. 현재 닭의 수: " + numberOfChickens);
    }

    /// <summary>
    /// 닭을 제거하고 수를 감소시킵니다.
    /// </summary>
    public void RemoveChicken()
    {
        if (numberOfChickens > 0)
        {
            numberOfChickens--;
            NotificationManager.Instance.ShowNotification("닭 한 마리가 판매되었습니다. 현재 닭의 수: " + numberOfChickens);
        }
    }
}