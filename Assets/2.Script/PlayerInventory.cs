using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

// Milk Ŭ���� ���� �� �Ӽ��� �߰��մϴ�.
[System.Serializable]
public class Milk
{
    public float freshness;

    public Milk(float freshness)
    {
        this.freshness = freshness;
    }
}

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
    public List<Milk> milkList = new List<Milk>();
    public int milkerLevel = 0;
    [Tooltip("������ ���׷��̵� ������ ScriptableObject�� �����ϼ���.")]
    public MilkerUpgradeData milkerUpgradeData;

    [Header("�ѱ� ����")]
    public bool hasGun = false;
    public int currentBullets = 0;
    public int gunLevel = 0;
    [Tooltip("�ѱ� ���׷��̵� ������ ScriptableObject�� �����ϼ���.")]
    public GunUpgradeData gunUpgradeData;

    public int BasketCapacity => (basketLevel > 0 && basketUpgradeData != null) ? basketUpgradeData.upgradeLevels[basketLevel - 1].capacity : 0;
    public int MilkerCapacity => (milkerLevel > 0 && milkerUpgradeData != null) ? milkerUpgradeData.upgradeLevels[milkerLevel - 1].capacity : 0;
    public int MilkingYield => (milkerLevel > 0 && milkerUpgradeData != null) ? milkerUpgradeData.upgradeLevels[milkerLevel - 1].milkingYield : 1;
    public float GunDamage => (gunLevel > 0 && gunUpgradeData != null) ? gunUpgradeData.upgradeLevels[gunLevel - 1].damageIncrease : 10f;

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

    public int GetEggCount()
    {
        return currentEggs;
    }

    public void RemoveEggs(int amount)
    {
        currentEggs = Mathf.Max(0, currentEggs - amount);
        NotificationManager.Instance.ShowNotification($"�ٱ��Ͽ��� �ް� {amount}���� ���½��ϴ�. ����: {currentEggs}/{BasketCapacity}");
    }

    public void AddMilk(Milk milk)
    {
        if (milkerLevel == 0)
        {
            NotificationManager.Instance.ShowNotification("�����⸦ ���� �����ؾ� �մϴ�!");
            return;
        }

        if (milkList.Count < MilkerCapacity)
        {
            milkList.Add(milk);
            NotificationManager.Instance.ShowNotification($"�����⿡ ���� 1���� ��ҽ��ϴ�. ����: {milkList.Count}/{MilkerCapacity}");
        }
        else
        {
            NotificationManager.Instance.ShowNotification("�����Ⱑ �� á���ϴ�!");
        }
    }

    public void AddMilk(float freshness)
    {
        AddMilk(new Milk(freshness));
    }

    public void TransferToWarehouse()
    {
        NotificationManager.Instance.ShowNotification($"���� ��: {currentEggs}��, ���� ����: {milkList.Count}��");

        if (currentEggs > 0)
        {
            if (Warehouse.Instance != null)
            {
                Warehouse.Instance.AddEggs(currentEggs);
            }
            currentEggs = 0;
            NotificationManager.Instance.ShowNotification("�ٱ����� ���� ��� â��� �Ű���ϴ�!");
        }

        if (milkList.Count > 0)
        {
            if (Warehouse.Instance != null)
            {
                Warehouse.Instance.AddMilk(new List<Milk>(milkList));
            }
            milkList.Clear();
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
    }
}