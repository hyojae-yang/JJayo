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
    // 싱글톤 인스턴스
    public static PlayerInventory Instance { get; private set; }

    // ★★★ 인벤토리 변경을 알리는 이벤트 추가 ★★★
    public event Action OnInventoryChanged;

    [Header("바구니 설정")]
    [Tooltip("현재 바구니에 담긴 달걀의 개수.")]
    public int currentEggs = 0;
    [Tooltip("바구니 업그레이드 데이터 ScriptableObject를 연결하세요.")]
    public BasketUpgradeData basketUpgradeData;

    [Header("착유기 설정")]
    [Tooltip("현재 착유기에 담긴 우유들의 신선도 목록.")]
    public List<Milk> milkList = new List<Milk>();
    [Tooltip("착유기 업그레이드 데이터 ScriptableObject를 연결하세요.")]
    public MilkerUpgradeData milkerUpgradeData;

    [Header("총기 설정")]
    public int currentBullets = 0;
    [Tooltip("총기 업그레이드 데이터 ScriptableObject를 연결하세요.")]
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

    // ★★★ 이벤트 호출을 위한 공용 메서드 추가 ★★★
    public void NotifyInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }

    public int AddEggs(int amount)
    {
        if (GameManager.Instance.gameData.basketLevel == 0)
        {
            NotificationManager.Instance.ShowNotification("바구니를 먼저 구매해야 합니다!");
            return 0;
        }

        int spaceLeft = BasketCapacity - currentEggs;
        int eggsToAdd = Mathf.Min(amount, spaceLeft);
        currentEggs += eggsToAdd;

        // ★★★ 달걀 추가 후 이벤트 호출 ★★★
        NotifyInventoryChanged();

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

        // ★★★ 달걀 제거 후 이벤트 호출 ★★★
        NotifyInventoryChanged();

        NotificationManager.Instance.ShowNotification($"바구니에서 달걀 {amount}개를 꺼냈습니다. 현재: {currentEggs}/{BasketCapacity}");
    }

    public void AddMilk(Milk milk)
    {
        if (GameManager.Instance.gameData.milkerLevel == 0)
        {
            NotificationManager.Instance.ShowNotification("착유기를 먼저 구매해야 합니다!");
            return;
        }

        if (milkList.Count < MilkerCapacity)
        {
            milkList.Add(milk);

            // ★★★ 우유 추가 후 이벤트 호출 ★★★
            NotifyInventoryChanged();

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
        }

        if (milkList.Count > 0)
        {
            if (Warehouse.Instance != null)
            {
                Warehouse.Instance.AddMilk(new List<Milk>(milkList));
            }
            milkList.Clear();
        }

        // ★★★ 창고로 옮긴 후 이벤트 호출 ★★★
        NotifyInventoryChanged();
        NotificationManager.Instance.ShowNotification("아이템을 모두 창고로 옮겼습니다!");
    }

    public void AddBullets(int amount)
    {
        currentBullets += amount;
        GameManager.Instance.gameData.bulletCount = currentBullets;
    }
}