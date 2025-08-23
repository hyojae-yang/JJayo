using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static PlayerInventory Instance { get; private set; }

    [Header("�ٱ��� ����")]
    [Tooltip("�ٱ����� �ִ� �뷮.")]
    public int basketCapacity = 0; // �ʱ� �뷮�� 0���� ����
    [Tooltip("���� �ٱ��Ͽ� ��� �ް��� ����.")]
    public int currentEggs = 0;
    public int basketLevel = 0; // �ʱ� ������ 0���� ����

    [Header("������ ����")]
    [Tooltip("�������� �ִ� �뷮.")]
    public int milkerCapacity = 0; // �ʱ� �뷮�� 0���� ����
    [Tooltip("���� �����⿡ ��� �������� �ż��� ���.")]
    public List<float> currentMilkFreshness = new List<float>();
    public float milkerSpeed = 0f; // �ʱ� �ӵ��� 0���� ����
    public int milkerLevel = 0; // �ʱ� ������ 0���� ����

    [Header("�ѱ� ����")]
    public bool hasGun = false;
    public int currentBullets = 0;
    public int gunLevel = 0;
    public float gunDamage = 10f;
    public float gunFireRate = 1.0f;

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

    public int AddEggs(int amount)
    {
        if (basketLevel == 0) // �ٱ��ϰ� ���� ���
        {
            NotificationManager.Instance.ShowNotification("�ٱ��ϸ� ���� �����ؾ� �մϴ�!");
            return 0;
        }

        int spaceLeft = basketCapacity - currentEggs;
        int eggsToAdd = Mathf.Min(amount, spaceLeft);
        currentEggs += eggsToAdd;
        NotificationManager.Instance.ShowNotification($"�ٱ��Ͽ� �ް� {eggsToAdd}���� ��ҽ��ϴ�. ����: {currentEggs}/{basketCapacity}");
        return eggsToAdd;
    }

    public void RemoveEggs(int amount)
    {
        currentEggs = Mathf.Max(0, currentEggs - amount);
        NotificationManager.Instance.ShowNotification($"�ٱ��Ͽ��� �ް� {amount}���� ���½��ϴ�. ����: {currentEggs}/{basketCapacity}");
    }

    public int AddMilk(float freshness)
    {
        if (milkerLevel == 0) // �����Ⱑ ���� ���
        {
            NotificationManager.Instance.ShowNotification("�����⸦ ���� �����ؾ� �մϴ�!");
            return 0;
        }

        if (currentMilkFreshness.Count < milkerCapacity)
        {
            currentMilkFreshness.Add(freshness);
            NotificationManager.Instance.ShowNotification($"�����⿡ �ż��� {freshness:F2}�� ������ ��ҽ��ϴ�. ����: {currentMilkFreshness.Count}/{milkerCapacity}");
            return 1;
        }
        NotificationManager.Instance.ShowNotification("�����Ⱑ �� á���ϴ�!");
        return 0;
    }

    public void TransferToWarehouse()
    {
        NotificationManager.Instance.ShowNotification($"TransferToWarehouse �Լ� ȣ���. ���� ��: {currentEggs}��, ���� ����: {currentMilkFreshness.Count}��");
        if (currentEggs > 0)
        {
            // �ӽ÷� ���� �ż��� 100f�� ����
            // Warehouse.Instance.AddEggs(currentEggs);
            currentEggs = 0;
            NotificationManager.Instance.ShowNotification("�ٱ����� ���� ��� â��� �Ű���ϴ�!");
        }

        if (currentMilkFreshness.Count > 0)
        {
            // Warehouse.Instance.AddMilk(currentMilkFreshness);
            currentMilkFreshness.Clear();
            NotificationManager.Instance.ShowNotification("�������� ������ ��� â��� �Ű���ϴ�!");
        }
    }

    public void AddGun()
    {
        if (!hasGun)
        {
            hasGun = true;
            gunLevel = 1;
            NotificationManager.Instance.ShowNotification("���� ȹ���߽��ϴ�!");
        }
        else
        {
            NotificationManager.Instance.ShowNotification("�̹� ���� �ֽ��ϴ�.");
        }
    }

    public void AddBullets(int amount)
    {
        currentBullets += amount;
        NotificationManager.Instance.ShowNotification($"�Ѿ� {amount}���� ȹ���߽��ϴ�! ���� �Ѿ�: {currentBullets}");
    }
}