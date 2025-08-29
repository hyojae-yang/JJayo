using UnityEngine;

public class ChickenCoop : MonoBehaviour
{
    public static ChickenCoop Instance { get; private set; }

    [Header("���� ���� ������")]
    public ChickenCoopData chickenCoopData;

    [Header("���� ���� ����")]
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
            // �ڡڡ� �߰��� �κ�: �ް� ���귮 ��� �ڡڡ�
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
                NotificationManager.Instance.ShowNotification($"�ٱ��Ͽ� �� {eggsTransferred}���� ��ҽ��ϴ�. ���忡 ���� ��: {currentEggCount}");
                productionTimer = 0f;
            }
        }
    }

    public void AddChicken()
    {
        numberOfChickens++;
        NotificationManager.Instance.ShowNotification("���ο� ���� ���忡 �߰��Ǿ����ϴ�. ���� ���� ��: " + numberOfChickens);
    }

    public void RemoveChicken()
    {
        if (numberOfChickens > 0)
        {
            numberOfChickens--;
            NotificationManager.Instance.ShowNotification("�� �� ������ �ǸŵǾ����ϴ�. ���� ���� ��: " + numberOfChickens);
        }
    }
}