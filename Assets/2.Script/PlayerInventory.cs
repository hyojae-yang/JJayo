using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

// Milk 클래스 위에 이 속성을 추가합니다.
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
    // 싱글톤 인스턴스
    public static PlayerInventory Instance { get; private set; }

    public event Action OnCapacityChanged;

    [Header("바구니 설정")]
    [Tooltip("현재 바구니에 담긴 달걀의 개수.")]
    public int currentEggs = 0;
    public int basketLevel = 0;
    [Tooltip("바구니 업그레이드 데이터 ScriptableObject를 연결하세요.")]
    public BasketUpgradeData basketUpgradeData;

    [Header("착유기 설정")]
    [Tooltip("현재 착유기에 담긴 우유들의 신선도 목록.")]
    public List<Milk> milkList = new List<Milk>();
    public int milkerLevel = 0;
    [Tooltip("착유기 업그레이드 데이터 ScriptableObject를 연결하세요.")]
    public MilkerUpgradeData milkerUpgradeData;

    [Header("총기 설정")]
    public bool hasGun = false;
    public int currentBullets = 0;
    public int gunLevel = 0;
    [Tooltip("총기 업그레이드 데이터 ScriptableObject를 연결하세요.")]
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
        NotificationManager.Instance.ShowNotification($"바구니가 레벨 {basketLevel}로 업그레이드 되었습니다!");
        OnCapacityChanged?.Invoke();
    }

    public void UpgradeMilker()
    {
        if (milkerUpgradeData == null) return;
        milkerLevel++;
        NotificationManager.Instance.ShowNotification($"착유기가 레벨 {milkerLevel}로 업그레이드 되었습니다!");
        OnCapacityChanged?.Invoke();
    }

    public void UpgradeGun()
    {
        if (gunUpgradeData == null) return;
        gunLevel++;
        NotificationManager.Instance.ShowNotification($"총이 레벨 {gunLevel}로 업그레이드 되었습니다!");
    }

    public int AddEggs(int amount)
    {
        if (basketLevel == 0)
        {
            NotificationManager.Instance.ShowNotification("바구니를 먼저 구매해야 합니다!");
            return 0;
        }

        int spaceLeft = BasketCapacity - currentEggs;
        int eggsToAdd = Mathf.Min(amount, spaceLeft);
        currentEggs += eggsToAdd;
        NotificationManager.Instance.ShowNotification($"바구니에 달걀 {eggsToAdd}개를 담았습니다. 현재: {currentEggs}/{BasketCapacity}");
        return eggsToAdd;
    }

    public int GetEggCount()
    {
        return currentEggs;
    }

    public void RemoveEggs(int amount)
    {
        currentEggs = Mathf.Max(0, currentEggs - amount);
        NotificationManager.Instance.ShowNotification($"바구니에서 달걀 {amount}개를 꺼냈습니다. 현재: {currentEggs}/{BasketCapacity}");
    }

    public void AddMilk(Milk milk)
    {
        if (milkerLevel == 0)
        {
            NotificationManager.Instance.ShowNotification("착유기를 먼저 구매해야 합니다!");
            return;
        }

        if (milkList.Count < MilkerCapacity)
        {
            milkList.Add(milk);
            NotificationManager.Instance.ShowNotification($"착유기에 우유 1개를 담았습니다. 현재: {milkList.Count}/{MilkerCapacity}");
        }
        else
        {
            NotificationManager.Instance.ShowNotification("착유기가 꽉 찼습니다!");
        }
    }

    public void AddMilk(float freshness)
    {
        AddMilk(new Milk(freshness));
    }

    public void TransferToWarehouse()
    {
        NotificationManager.Instance.ShowNotification($"현재 알: {currentEggs}개, 현재 우유: {milkList.Count}개");

        if (currentEggs > 0)
        {
            if (Warehouse.Instance != null)
            {
                Warehouse.Instance.AddEggs(currentEggs);
            }
            currentEggs = 0;
            NotificationManager.Instance.ShowNotification("바구니의 알을 모두 창고로 옮겼습니다!");
        }

        if (milkList.Count > 0)
        {
            if (Warehouse.Instance != null)
            {
                Warehouse.Instance.AddMilk(new List<Milk>(milkList));
            }
            milkList.Clear();
            NotificationManager.Instance.ShowNotification("착유기의 우유를 모두 창고로 옮겼습니다!");
        }
    }

    public void AddGun()
    {
        if (!hasGun)
        {
            hasGun = true;
            gunLevel = 1;
            NotificationManager.Instance.ShowNotification("총을 획득했습니다!");
        }
        else
        {
            NotificationManager.Instance.ShowNotification("이미 총이 있습니다.");
        }
    }

    public void AddBullets(int amount)
    {
        currentBullets += amount;
    }
}