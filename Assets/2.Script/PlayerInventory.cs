using UnityEngine;
using System.Collections.Generic;
using System; // Action Ÿ���� ����ϱ� ���� �߰�

public class PlayerInventory : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static PlayerInventory Instance { get; private set; }

    // **********************************************
    // �ڡڡ� ���� �߰��� �κ� �ڡڡ�
    // �뷮�� ����� �� �ٸ� ��ũ��Ʈ(PlayerUI)���� �˸��� �̺�Ʈ
    public event Action OnCapacityChanged;
    // **********************************************

    [Header("�ٱ��� ����")]
    [Tooltip("�ٱ����� �ִ� �뷮.")]
    public int basketCapacity = 0;
    [Tooltip("���� �ٱ��Ͽ� ��� �ް��� ����.")]
    public int currentEggs = 0;
    public int basketLevel = 0;

    [Header("������ ����")]
    [Tooltip("�������� �ִ� �뷮.")]
    public int milkerCapacity = 0;
    [Tooltip("���� �����⿡ ��� �������� �ż��� ���.")]
    public List<float> currentMilkFreshness = new List<float>();
    public float milkerSpeed = 0f;
    public int milkerLevel = 0;

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

    // �������� ���׷��̵� �� �� �Լ����� ȣ���ؾ� �մϴ�.

    public void SetBasketCapacity(int newCapacity, int newLevel)
    {
        basketCapacity = newCapacity;
        basketLevel = newLevel;
        // �뷮�� ����Ǿ����� �̺�Ʈ �߻�! (UI���� �˸�)
        OnCapacityChanged?.Invoke();
    }

    public void SetMilkerCapacity(int newCapacity, int newLevel, float newSpeed)
    {
        milkerCapacity = newCapacity;
        milkerLevel = newLevel;
        milkerSpeed = newSpeed;
        // �뷮�� ����Ǿ����� �̺�Ʈ �߻�! (UI���� �˸�)
        OnCapacityChanged?.Invoke();
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

    // �ڡڡ� �� �Լ��� �����Ǿ����ϴ�. �ڡڡ�
    public void TransferToWarehouse()
    {
        NotificationManager.Instance.ShowNotification($"���� ��: {currentEggs}��, ���� ����: {currentMilkFreshness.Count}��");

        // ���� â��� �ű�� ����
        if (currentEggs > 0)
        {
            List<float> eggsToTransferFreshness = new List<float>();
            for (int i = 0; i < currentEggs; i++)
            {
                eggsToTransferFreshness.Add(100f);
            }

            // ������ â��� ���� �ű�ϴ�.
            if (Warehouse.Instance != null)
            {
                Warehouse.Instance.AddEggs(eggsToTransferFreshness);
            }

            currentEggs = 0;
            NotificationManager.Instance.ShowNotification("�ٱ����� ���� ��� â��� �Ű���ϴ�!");
        }

        // ������ â��� �ű�� ����
        if (currentMilkFreshness.Count > 0)
        {
            // PlayerInventory�� �ִ� ���� �ż��� ����� �״�� â��� �ű�ϴ�.
            if (Warehouse.Instance != null)
            {
                Warehouse.Instance.AddMilk(currentMilkFreshness);
            }

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

    // **********************************************
    // �ڡڡ� ���� �ý��� ���� �Լ� ���� �ڡڡ�
    // **********************************************

    /// <summary>
    /// â�� ����� ������ ������ ��ȯ�մϴ�.
    /// (���� �ǸŴ� â���� ������ �������� �մϴ�.)
    /// </summary>
    public int GetMilkCount()
    {
        if (Warehouse.Instance != null)
        {
            return Warehouse.Instance.GetMilkCount();
        }
        return 0;
    }

    /// <summary>
    /// â�� ����� �������� ��� �ż����� ��ȯ�մϴ�.
    /// </summary>
    public float GetAverageFreshness()
    {
        if (Warehouse.Instance != null)
        {
            return Warehouse.Instance.GetAverageMilkFreshness();
        }
        return 0f;
    }

    /// <summary>
    /// ������ �䱸 ������ �����Ͽ� ������ �Ǹ��� �� �ִ��� Ȯ���մϴ�.
    /// </summary>
    /// <param name="requiredAmount">������ �䱸�ϴ� ���� ����</param>
    /// <param name="requiredFreshness">������ �䱸�ϴ� �ּ� �ż���</param>
    /// <returns>�Ǹ� ���� ����</returns>
    public bool CanSellMilk(int requiredAmount, float requiredFreshness)
    {
        // 1. ���� ���� Ȯ��
        if (GetMilkCount() < requiredAmount)
        {
            NotificationManager.Instance.ShowNotification("â�� ������ �����մϴ�!");
            return false;
        }

        // 2. �ż��� ���� Ȯ�� (��� �ż����� Ȯ��)
        if (GetAverageFreshness() < requiredFreshness)
        {
            NotificationManager.Instance.ShowNotification("������ ��� �ż����� ���� ������ �䱸 ������ �������� ���մϴ�.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// ���ο��� ������ �Ǹ��ϰ� â���� ������ �����մϴ�.
    /// </summary>
    /// <param name="amount">�Ǹ��� ���� ����</param>
    public void SellMilk(int amount)
    {
        if (Warehouse.Instance != null)
        {
            Warehouse.Instance.RemoveMilk(amount);
            NotificationManager.Instance.ShowNotification($"���� {amount}���� ���ο��� �Ǹ��߽��ϴ�!");
        }
    }

    // **********************************************
    // �ڡڡ� ���� �ý��� ���� �Լ� �� �ڡڡ�
    // **********************************************
}