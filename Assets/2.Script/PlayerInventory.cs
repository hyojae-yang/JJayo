using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static PlayerInventory Instance { get; private set; }

    [Header("�ٱ��� ����")]
    [Tooltip("�ٱ����� �ִ� �뷮.")]
    public int basketCapacity = 10;
    [Tooltip("���� �ٱ��Ͽ� ��� �ް��� ����.")]
    public int currentEggs = 0;

    [Header("������ ����")]
    [Tooltip("�������� �ִ� �뷮.")]
    public int milkerCapacity = 10;
    [Tooltip("���� �����⿡ ��� �������� �ż��� ���.")]
    public List<float> currentMilkFreshness = new List<float>();

    private void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �ٱ��Ͽ� �ް��� �߰��մϴ�. �ٱ��� �뷮�� �ʰ��� �� �����ϴ�.
    /// </summary>
    /// <param name="amount">�߰��� �ް��� ����</param>
    /// <returns>������ �߰��� �ް��� ����</returns>
    public int AddEggs(int amount)
    {
        int spaceLeft = basketCapacity - currentEggs;
        int eggsToAdd = Mathf.Min(amount, spaceLeft);

        currentEggs += eggsToAdd;
        Debug.Log($"�ٱ��Ͽ� �ް� {eggsToAdd}���� ��ҽ��ϴ�. ����: {currentEggs}/{basketCapacity}");

        return eggsToAdd;
    }

    /// <summary>
    /// �ٱ��Ͽ��� �ް��� �����մϴ�.
    /// </summary>
    /// <param name="amount">������ �ް��� ����</param>
    public void RemoveEggs(int amount)
    {
        currentEggs = Mathf.Max(0, currentEggs - amount);
        Debug.Log($"�ٱ��Ͽ��� �ް� {amount}���� ���½��ϴ�. ����: {currentEggs}/{basketCapacity}");
    }

    /// <summary>
    /// �����⿡ ������ �߰��մϴ�. ������ �뷮�� �ʰ��� �� �����ϴ�.
    /// </summary>
    /// <param name="freshness">�߰��� ������ �ż���</param>
    /// <returns>������ �߰��� ������ ����</returns>
    public int AddMilk(float freshness)
    {
        if (currentMilkFreshness.Count < milkerCapacity)
        {
            currentMilkFreshness.Add(freshness);
            Debug.Log($"�����⿡ �ż��� {freshness:F2}�� ������ ��ҽ��ϴ�. ����: {currentMilkFreshness.Count}/{milkerCapacity}");
            return 1;
        }

        Debug.Log("�����Ⱑ �� á���ϴ�!");
        return 0;
    }

    /// <summary>
    /// ������� �ٱ����� ��� ���빰�� â��� �ű�ϴ�.
    /// </summary>
    public void TransferToWarehouse()
    {
        // ���� �κ��丮 ���¸� �ֿܼ� ����մϴ�.
        Debug.Log($"TransferToWarehouse �Լ� ȣ���. ���� ��: {currentEggs}��, ���� ����: {currentMilkFreshness.Count}��");

        // �ٱ��Ͽ� ���� ������ â��� �ű�ϴ�.
        if (currentEggs > 0)
        {
            List<float> eggFreshnessList = new List<float>();
            for (int i = 0; i < currentEggs; i++)
            {
                eggFreshnessList.Add(100f);
            }
            Warehouse.Instance.AddEggs(eggFreshnessList);
            currentEggs = 0; // �ٱ��� ����
            Debug.Log("�ٱ����� ���� ��� â��� �Ű���ϴ�!");
        }

        // �����⿡ ������ ������ â��� �ű�ϴ�.
        if (currentMilkFreshness.Count > 0)
        {
            Warehouse.Instance.AddMilk(currentMilkFreshness);
            currentMilkFreshness.Clear(); // ������ ����
            Debug.Log("�������� ������ ��� â��� �Ű���ϴ�!");
        }
    }
}