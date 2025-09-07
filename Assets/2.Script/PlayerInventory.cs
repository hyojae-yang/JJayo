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
    [Tooltip("현재 바구니에 담긴 달걀의 신선도 목록.")]
    public List<float> eggFreshnessList = new List<float>(); // ★★★ int에서 List<float>로 변경 ★★★
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
            if (GameManager.Instance == null || GameManager.Instance.CurrentGameData.basketLevel <= 0 || basketUpgradeData == null)
            {
                return 0;
            }
            return basketUpgradeData.GetCapacity(GameManager.Instance.CurrentGameData.basketLevel);
        }
    }

    public int MilkerCapacity
    {
        get
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentGameData.milkerLevel <= 0 || milkerUpgradeData == null)
            {
                return 0;
            }
            return milkerUpgradeData.GetCapacity(GameManager.Instance.CurrentGameData.milkerLevel);
        }
    }

    public int MilkingYield
    {
        get
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentGameData.milkerLevel <= 0 || milkerUpgradeData == null)
            {
                return 0;
            }
            return milkerUpgradeData.GetMilkingYield(GameManager.Instance.CurrentGameData.milkerLevel);
        }
    }

    public float GunDamage
    {
        get
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentGameData.gunLevel <= 0 || gunUpgradeData == null)
            {
                return 0;
            }
            return gunUpgradeData.GetDamage(GameManager.Instance.CurrentGameData.gunLevel);
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

    public void NotifyInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }

    // ★★★ 수정된 메서드: 달걀 리스트를 인자로 받습니다. ★★★
    public int AddEggs(List<float> newEggs)
    {
        int spaceLeft = BasketCapacity - eggFreshnessList.Count;
        int eggsToAdd = Mathf.Min(newEggs.Count, spaceLeft);

        for (int i = 0; i < eggsToAdd; i++)
        {
            eggFreshnessList.Add(newEggs[i]);
        }

        NotifyInventoryChanged();
        return eggsToAdd;
    }

    public int GetEggCount()
    {
        return eggFreshnessList.Count;
    }

    public void RemoveEggs(int amount)
    {
        if (amount > eggFreshnessList.Count)
        {
            Debug.LogError("바구니에 달걀이 부족합니다.");
            return;
        }

        // ★★★ 추가: 신선도가 낮은 달걀부터 정렬하여 제거 ★★★
        eggFreshnessList.Sort();
        eggFreshnessList.RemoveRange(0, amount);

        NotifyInventoryChanged();
        NotificationManager.Instance.ShowNotification($"바구니에서 달걀 {amount}개를 꺼냈습니다. 현재: {eggFreshnessList.Count}/{BasketCapacity}");
    }

    public int AddMilk(int amount, float freshness)
    {
        int addedCount = 0;
        for (int i = 0; i < amount; i++)
        {
            if (milkList.Count < MilkerCapacity)
            {
                milkList.Add(new Milk(freshness));
                addedCount++;
            }
            else
            {
                NotificationManager.Instance.ShowNotification("착유기가 꽉 찼습니다!");
                break;
            }
        }

        if (addedCount > 0)
        {
            NotifyInventoryChanged();
        }

        return addedCount;
    }

    public void TransferToWarehouse()
    {
        if (eggFreshnessList.Count > 0)
        {
            if (Warehouse.Instance != null)
            {
                Warehouse.Instance.AddEggs(new List<float>(eggFreshnessList));
            }
            eggFreshnessList.Clear();
        }

        if (milkList.Count > 0)
        {
            if (Warehouse.Instance != null)
            {
                Warehouse.Instance.AddMilk(new List<Milk>(milkList));
            }
            milkList.Clear();
        }

        NotifyInventoryChanged();
    }

    public void AddBullets(int amount)
    {
        currentBullets += amount;
        GameManager.Instance.CurrentGameData.bulletCount = currentBullets;
    }
}