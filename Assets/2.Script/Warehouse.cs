using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Warehouse : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static Warehouse Instance { get; private set; }

    [Header("창고 설정")]
    [Tooltip("창고에 보관된 모든 달걀의 신선도 목록.")]
    public List<float> storedEggFreshness = new List<float>();

    [Tooltip("창고에 보관된 모든 우유의 신선도 목록.")]
    public List<Milk> storedMilkList = new List<Milk>();

    [Tooltip("아이템의 신선도가 감소하는 주기(초).")]
    public float freshnessDecayInterval = 120f;
    private float decayTimer = 0f;

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

    void Update()
    {
        decayTimer += Time.deltaTime;
        if (decayTimer >= freshnessDecayInterval)
        {
            DecayFreshness();
            decayTimer = 0f;
        }
    }

    public void AddEggs(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            storedEggFreshness.Add(100f);
        }
    }

    public void AddMilk(List<Milk> newMilkList)
    {
        storedMilkList.AddRange(newMilkList);
        NotificationManager.Instance.ShowNotification($"창고에 우유 {newMilkList.Count}개를 추가했습니다. 현재 총 우유: {storedMilkList.Count}개");
    }

    private void DecayFreshness()
    {
        for (int i = storedMilkList.Count - 1; i >= 0; i--)
        {
            storedMilkList[i].freshness = Mathf.Max(0, storedMilkList[i].freshness - 1);
            if (storedMilkList[i].freshness <= 0)
            {
                storedMilkList.RemoveAt(i);
            }
        }

        for (int i = storedEggFreshness.Count - 1; i >= 0; i--)
        {
            storedEggFreshness[i] = Mathf.Max(0, storedEggFreshness[i] - 1);
            if (storedEggFreshness[i] <= 0)
            {
                storedEggFreshness.RemoveAt(i);
            }
        }
        NotificationManager.Instance.ShowNotification("창고에 있는 모든 아이템의 신선도가 감소했습니다.");
    }

    public int GetEggCount()
    {
        return storedEggFreshness.Count;
    }

    public int GetMilkCount()
    {
        return storedMilkList.Count;
    }

    public float GetAverageMilkFreshness()
    {
        return storedMilkList.Any() ? storedMilkList.Average(m => m.freshness) : 0f;
    }

    public void RemoveMilk(int amount)
    {
        if (amount > storedMilkList.Count)
        {
            Debug.LogError("창고에 판매할 우유가 부족합니다!");
            return;
        }
        storedMilkList.Sort((a, b) => a.freshness.CompareTo(b.freshness));
        storedMilkList.RemoveRange(0, amount);
    }

    public void RemoveEggs(int amount)
    {
        if (amount > storedEggFreshness.Count)
        {
            Debug.LogError("창고에 판매할 달걀이 부족합니다!");
            return;
        }
        storedEggFreshness.RemoveRange(0, amount);
    }

    // ★★★ 추가: TraderManager가 호출하는 메서드들을 여기에 추가합니다. ★★★
    public bool CanSellMilk(int requiredAmount, float requiredFreshness)
    {
        if (GetMilkCount() < requiredAmount)
        {
            NotificationManager.Instance.ShowNotification("창고에 우유가 부족합니다!");
            return false;
        }

        if (GetAverageMilkFreshness() < requiredFreshness)
        {
            NotificationManager.Instance.ShowNotification("우유의 평균 신선도가 낮아 상인의 요구 조건을 충족하지 못합니다.");
            return false;
        }
        return true;
    }

    public void SellMilk(int amount)
    {
        if (CanSellMilk(amount, 0)) // 여기서 신선도 조건은 이미 TraderManager에서 체크했으므로 0으로 설정
        {
            // 신선도가 낮은 우유부터 판매 (가장 오래된 우유부터 처리)
            storedMilkList.Sort((a, b) => a.freshness.CompareTo(b.freshness));
            storedMilkList.RemoveRange(0, amount);

            NotificationManager.Instance.ShowNotification($"우유 {amount}개를 상인에게 판매했습니다!");
        }
    }
}