using UnityEngine;

public class ChickenCoop : MonoBehaviour
{
    public static ChickenCoop Instance { get; private set; }

    [Header("닭장 고유 데이터")]
    public ChickenCoopData chickenCoopData;

    [Header("현재 닭장 상태")]
    public int currentEggCount = 0;
    public int numberOfChickens;

    private float productionTimer = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            AnimalHandler handler = FindFirstObjectByType<AnimalHandler>();
            if (handler != null)
            {
                handler.RegisterChickenCoop(this);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
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
            productionTimer = 0f;
            // ★★★ 추가된 부분: 달걀 생산량 기록 ★★★
            if (GameManager.Instance != null && GameManager.Instance.gameData != null)
            {
                GameManager.Instance.gameData.dailyEggsProduced++;
            }
        }
    }

    void OnMouseDown()
    {
        if (EquipmentManager.Instance.GetCurrentEquipment() == EquipmentType.Basket)
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

    public void AddChicken()
    {
        numberOfChickens++;
        NotificationManager.Instance.ShowNotification("새로운 닭이 닭장에 추가되었습니다. 현재 닭의 수: " + numberOfChickens);
    }

    public void RemoveChicken()
    {
        if (numberOfChickens > 0)
        {
            numberOfChickens--;
            NotificationManager.Instance.ShowNotification("닭 한 마리가 판매되었습니다. 현재 닭의 수: " + numberOfChickens);
        }
    }
}