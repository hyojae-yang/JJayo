using UnityEngine;

public class ChickenCoop : MonoBehaviour
{
    // �̱��� �ν��Ͻ� (���� ������ ������ ���� ������Ʈ�� ����)
    public static ChickenCoop Instance { get; private set; }

    [Header("���� ���� ������")]
    public ChickenCoopData chickenCoopData;

    [Header("���� ���� ����")]
    public int currentEggCount = 0;
    public int numberOfChickens;

    // �� ������ ���� Ÿ�̸�
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
        // Start������ ���� ���� 0���� �ʱ�ȭ���� �ʵ��� �����߽��ϴ�.
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
            NotificationManager.Instance.ShowNotification($"���忡�� ���� {currentEggCount}�� �����߽��ϴ�!");
            productionTimer = 0f;
        }
    }

    // ���콺�� Ŭ���ϸ� ȣ��Ǵ� �Լ� (�� ����)
    void OnMouseDown()
    {
        // �ٱ��ϸ� �����ߴ��� Ȯ���ϰ� ���� �����մϴ�.
        if (EquipmentManager.Instance.currentEquipment == EquipmentType.Basket)
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

    /// <summary>
    /// ���� �߰��ϰ� ���� ������ŵ�ϴ�.
    /// </summary>
    public void AddChicken()
    {
        numberOfChickens++;
        NotificationManager.Instance.ShowNotification("���ο� ���� ���忡 �߰��Ǿ����ϴ�. ���� ���� ��: " + numberOfChickens);
    }

    /// <summary>
    /// ���� �����ϰ� ���� ���ҽ�ŵ�ϴ�.
    /// </summary>
    public void RemoveChicken()
    {
        if (numberOfChickens > 0)
        {
            numberOfChickens--;
            NotificationManager.Instance.ShowNotification("�� �� ������ �ǸŵǾ����ϴ�. ���� ���� ��: " + numberOfChickens);
        }
    }
}