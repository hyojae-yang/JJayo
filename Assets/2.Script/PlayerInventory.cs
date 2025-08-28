using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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

    // �ڡڡ� �κ��丮 ������ �˸��� �̺�Ʈ �߰� �ڡڡ�
    public event Action OnInventoryChanged;

    [Header("�ٱ��� ����")]
    [Tooltip("���� �ٱ��Ͽ� ��� �ް��� ����.")]
    public int currentEggs = 0;
    [Tooltip("�ٱ��� ���׷��̵� ������ ScriptableObject�� �����ϼ���.")]
    public BasketUpgradeData basketUpgradeData;

    [Header("������ ����")]
    [Tooltip("���� �����⿡ ��� �������� �ż��� ���.")]
    public List<Milk> milkList = new List<Milk>();
    [Tooltip("������ ���׷��̵� ������ ScriptableObject�� �����ϼ���.")]
    public MilkerUpgradeData milkerUpgradeData;

    [Header("�ѱ� ����")]
    public int currentBullets = 0;
    [Tooltip("�ѱ� ���׷��̵� ������ ScriptableObject�� �����ϼ���.")]
    public GunUpgradeData gunUpgradeData;

    public int BasketCapacity
    {
        get
        {
            if (GameManager.Instance == null || GameManager.Instance.gameData.basketLevel <= 0 || basketUpgradeData == null)
            {
                return 0;
            }
            return basketUpgradeData.GetCapacity(GameManager.Instance.gameData.basketLevel);
        }
    }

    public int MilkerCapacity
    {
        get
        {
            if (GameManager.Instance == null || GameManager.Instance.gameData.milkerLevel <= 0 || milkerUpgradeData == null)
            {
                return 0;
            }
            return milkerUpgradeData.GetCapacity(GameManager.Instance.gameData.milkerLevel);
        }
    }

    public int MilkingYield
    {
        get
        {
            if (GameManager.Instance == null || GameManager.Instance.gameData.milkerLevel <= 0 || milkerUpgradeData == null)
            {
                return 0;
            }
            return milkerUpgradeData.GetMilkingYield(GameManager.Instance.gameData.milkerLevel);
        }
    }

    public float GunDamage
    {
        get
        {
            if (GameManager.Instance == null || GameManager.Instance.gameData.gunLevel <= 0 || gunUpgradeData == null)
            {
                return 0;
            }
            return gunUpgradeData.GetDamage(GameManager.Instance.gameData.gunLevel);
        }
    }

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

    // �ڡڡ� �̺�Ʈ ȣ���� ���� ���� �޼��� �߰� �ڡڡ�
    public void NotifyInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }

    public int AddEggs(int amount)
    {
        if (GameManager.Instance.gameData.basketLevel == 0)
        {
            NotificationManager.Instance.ShowNotification("�ٱ��ϸ� ���� �����ؾ� �մϴ�!");
            return 0;
        }

        int spaceLeft = BasketCapacity - currentEggs;
        int eggsToAdd = Mathf.Min(amount, spaceLeft);
        currentEggs += eggsToAdd;

        // �ڡڡ� �ް� �߰� �� �̺�Ʈ ȣ�� �ڡڡ�
        NotifyInventoryChanged();

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

        // �ڡڡ� �ް� ���� �� �̺�Ʈ ȣ�� �ڡڡ�
        NotifyInventoryChanged();

        NotificationManager.Instance.ShowNotification($"�ٱ��Ͽ��� �ް� {amount}���� ���½��ϴ�. ����: {currentEggs}/{BasketCapacity}");
    }

    public void AddMilk(Milk milk)
    {
        if (GameManager.Instance.gameData.milkerLevel == 0)
        {
            NotificationManager.Instance.ShowNotification("�����⸦ ���� �����ؾ� �մϴ�!");
            return;
        }

        if (milkList.Count < MilkerCapacity)
        {
            milkList.Add(milk);

            // �ڡڡ� ���� �߰� �� �̺�Ʈ ȣ�� �ڡڡ�
            NotifyInventoryChanged();

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
        }

        if (milkList.Count > 0)
        {
            if (Warehouse.Instance != null)
            {
                Warehouse.Instance.AddMilk(new List<Milk>(milkList));
            }
            milkList.Clear();
        }

        // �ڡڡ� â��� �ű� �� �̺�Ʈ ȣ�� �ڡڡ�
        NotifyInventoryChanged();
        NotificationManager.Instance.ShowNotification("�������� ��� â��� �Ű���ϴ�!");
    }

    public void AddBullets(int amount)
    {
        currentBullets += amount;
        GameManager.Instance.gameData.bulletCount = currentBullets;
    }
}