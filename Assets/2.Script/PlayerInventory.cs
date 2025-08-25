using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class PlayerInventory : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static PlayerInventory Instance { get; private set; }

    public event Action OnCapacityChanged;

    [Header("�ٱ��� ����")]
    [Tooltip("���� �ٱ��Ͽ� ��� �ް��� ����.")]
    public int currentEggs = 0;
    public int basketLevel = 0;
    [Tooltip("�ٱ��� ���׷��̵� ������ ScriptableObject�� �����ϼ���.")]
    public BasketUpgradeData basketUpgradeData;

    [Header("������ ����")]
    [Tooltip("���� �����⿡ ��� �������� �ż��� ���.")]
    public List<float> currentMilkFreshness = new List<float>();
    public int milkerLevel = 0;
    [Tooltip("������ ���׷��̵� ������ ScriptableObject�� �����ϼ���.")]
    public MilkerUpgradeData milkerUpgradeData;

    [Header("�ѱ� ����")]
    public bool hasGun = false;
    public int currentBullets = 0;
    public int gunLevel = 0;
    [Tooltip("�ѱ� ���׷��̵� ������ ScriptableObject�� �����ϼ���.")]
    public GunUpgradeData gunUpgradeData;

    // �ڡڡ� ������ �κ�: �뷮, �ӵ�, �������� �Ӽ����� ���� �ڡڡ�
    public int BasketCapacity => (basketLevel > 0 && basketUpgradeData != null) ? basketUpgradeData.upgradeLevels[basketLevel - 1].capacity : 0;
    public int MilkerCapacity => (milkerLevel > 0 && milkerUpgradeData != null) ? milkerUpgradeData.upgradeLevels[milkerLevel - 1].capacity : 0;
    // �ڡڡ� ������ �κ�: MilkerSpeed �Ӽ��� MilkingYield�� ���� �ڡڡ�
    public int MilkingYield => (milkerLevel > 0 && milkerUpgradeData != null) ? milkerUpgradeData.upgradeLevels[milkerLevel - 1].milkingYield : 1;
    public float GunDamage => (gunLevel > 0 && gunUpgradeData != null) ? gunUpgradeData.upgradeLevels[gunLevel - 1].damageIncrease : 10f;
    public float GunFireRate => (gunLevel > 0 && gunUpgradeData != null) ? gunUpgradeData.upgradeLevels[gunLevel - 1].fireRateIncrease : 1.0f;

    private void Awake()
    {
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

    public void UpgradeBasket()
    {
        if (basketUpgradeData == null) return;
        basketLevel++;
        NotificationManager.Instance.ShowNotification($"�ٱ��ϰ� ���� {basketLevel}�� ���׷��̵� �Ǿ����ϴ�!");
        OnCapacityChanged?.Invoke();
    }

    public void UpgradeMilker()
    {
        if (milkerUpgradeData == null) return;
        milkerLevel++;
        NotificationManager.Instance.ShowNotification($"�����Ⱑ ���� {milkerLevel}�� ���׷��̵� �Ǿ����ϴ�!");
        OnCapacityChanged?.Invoke();
    }

    public void UpgradeGun()
    {
        if (gunUpgradeData == null) return;
        gunLevel++;
        NotificationManager.Instance.ShowNotification($"���� ���� {gunLevel}�� ���׷��̵� �Ǿ����ϴ�!");
    }

    public int AddEggs(int amount)
    {
        if (basketLevel == 0)
        {
            NotificationManager.Instance.ShowNotification("�ٱ��ϸ� ���� �����ؾ� �մϴ�!");
            return 0;
        }

        int spaceLeft = BasketCapacity - currentEggs;
        int eggsToAdd = Mathf.Min(amount, spaceLeft);
        currentEggs += eggsToAdd;
        NotificationManager.Instance.ShowNotification($"�ٱ��Ͽ� �ް� {eggsToAdd}���� ��ҽ��ϴ�. ����: {currentEggs}/{BasketCapacity}");
        return eggsToAdd;
    }

    public void RemoveEggs(int amount)
    {
        currentEggs = Mathf.Max(0, currentEggs - amount);
        NotificationManager.Instance.ShowNotification($"�ٱ��Ͽ��� �ް� {amount}���� ���½��ϴ�. ����: {currentEggs}/{BasketCapacity}");
    }

    // �ڡڡ� ������ �κ�: amount �Ű������� �߰��Ͽ� �� ���� ���� ������ �߰��մϴ�. �ڡڡ�
    public int AddMilk(float freshness, int amount)
    {
        if (milkerLevel == 0)
        {
            NotificationManager.Instance.ShowNotification("�����⸦ ���� �����ؾ� �մϴ�!");
            return 0;
        }

        int spaceLeft = MilkerCapacity - currentMilkFreshness.Count;
        int milkToAdd = Mathf.Min(amount, spaceLeft);

        for (int i = 0; i < milkToAdd; i++)
        {
            currentMilkFreshness.Add(freshness);
        }

        if (milkToAdd > 0)
        {
            NotificationManager.Instance.ShowNotification($"�����⿡ ���� {milkToAdd}���� ��ҽ��ϴ�. ����: {currentMilkFreshness.Count}/{MilkerCapacity}");
        }

        return milkToAdd;
    }

    public void TransferToWarehouse()
    {
        NotificationManager.Instance.ShowNotification($"���� ��: {currentEggs}��, ���� ����: {currentMilkFreshness.Count}��");
        if (currentEggs > 0)
        {
            List<float> eggsToTransferFreshness = new List<float>();
            for (int i = 0; i < currentEggs; i++)
            {
                eggsToTransferFreshness.Add(100f);
            }
            if (Warehouse.Instance != null)
            {
                Warehouse.Instance.AddEggs(eggsToTransferFreshness);
            }
            currentEggs = 0;
            NotificationManager.Instance.ShowNotification("�ٱ����� ���� ��� â��� �Ű���ϴ�!");
        }

        if (currentMilkFreshness.Count > 0)
        {
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

    public int GetMilkCount()
    {
        if (Warehouse.Instance != null)
        {
            return Warehouse.Instance.GetMilkCount();
        }
        return 0;
    }

    public float GetAverageFreshness()
    {
        if (Warehouse.Instance != null)
        {
            return Warehouse.Instance.GetAverageMilkFreshness();
        }
        return 0f;
    }

    public bool CanSellMilk(int requiredAmount, float requiredFreshness)
    {
        if (GetMilkCount() < requiredAmount)
        {
            NotificationManager.Instance.ShowNotification("â�� ������ �����մϴ�!");
            return false;
        }

        if (GetAverageFreshness() < requiredFreshness)
        {
            NotificationManager.Instance.ShowNotification("������ ��� �ż����� ���� ������ �䱸 ������ �������� ���մϴ�.");
            return false;
        }

        return true;
    }

    public void SellMilk(int amount)
    {
        if (Warehouse.Instance != null)
        {
            Warehouse.Instance.RemoveMilk(amount);
            NotificationManager.Instance.ShowNotification($"���� {amount}���� ���ο��� �Ǹ��߽��ϴ�!");
        }
    }
}