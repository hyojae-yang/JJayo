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
    // ★★★ 수정: List<float>에서 List<Milk>로 변경 ★★★
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

    /// <summary>
    /// 창고에 달걀을 추가합니다.
    /// </summary>
    /// <param name="newEggsFreshness">추가할 달걀들의 신선도 리스트</param>
    public void AddEggs(List<float> newEggsFreshness)
    {
        // AddRange는 이미 리스트를 추가하는 기능을 하므로,
        // 별도의 복잡한 평균 신선도 계산 로직이 필요하지 않습니다.
        // 이 코드가 AddEggs(int amount)로 변경되어야 TraderManager의 RemoveEggs와 연동이 됩니다.
        storedEggFreshness.AddRange(newEggsFreshness);
    }

    // ★★★ 추가: SaveLoadManager에서 달걀 개수를 직접 설정할 수 있도록 추가 ★★★
    public void SetEggCount(int count)
    {
        // 저장된 달걀 개수에 맞춰 리스트를 재구성합니다.
        // 신선도는 임의로 100으로 설정하거나, 다른 방법으로 복원해야 합니다.
        // 현재는 '개수'만 저장하므로 임의의 값으로 채웁니다.
        storedEggFreshness.Clear();
        for (int i = 0; i < count; i++)
        {
            storedEggFreshness.Add(100f);
        }
    }

    /// <summary>
    /// 창고에 우유를 추가합니다.
    /// </summary>
    /// <param name="newMilkList">추가할 우유들의 리스트</param>
    // ★★★ 수정: List<float>에서 List<Milk>를 매개변수로 받도록 변경 ★★★
    public void AddMilk(List<Milk> newMilkList)
    {
        // 새롭게 들어온 우유 리스트를 기존 창고 리스트에 추가합니다.
        storedMilkList.AddRange(newMilkList);

        NotificationManager.Instance.ShowNotification($"창고에 우유 {newMilkList.Count}개를 추가했습니다. 현재 총 우유: {storedMilkList.Count}개");
    }

    // ★★★ 추가: SaveLoadManager에서 우유 리스트를 직접 설정할 수 있도록 추가 ★★★
    public void SetMilkList(List<Milk> newMilkList)
    {
        storedMilkList = newMilkList;
    }

    /// <summary>
    /// 창고에 있는 모든 아이템의 신선도를 감소시킵니다.
    /// </summary>
    private void DecayFreshness()
    {
        // 우유 신선도 감소
        for (int i = storedMilkList.Count - 1; i >= 0; i--)
        {
            storedMilkList[i].freshness = Mathf.Max(0, storedMilkList[i].freshness - 1);
            if (storedMilkList[i].freshness <= 0)
            {
                storedMilkList.RemoveAt(i);
            }
        }

        // 달걀 신선도 감소 후 제거
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

    /// <summary>
    /// 창고에 있는 달걀의 총 개수를 반환합니다.
    /// </summary>
    public int GetEggCount()
    {
        return storedEggFreshness.Count;
    }

    /// <summary>
    /// 창고에 있는 우유의 총 개수를 반환합니다.
    /// </summary>
    public int GetMilkCount()
    {
        return storedMilkList.Count;
    }

    /// <summary>
    /// 창고에 있는 모든 우유의 평균 신선도를 계산하여 반환합니다.
    /// </summary>
    public float GetAverageMilkFreshness()
    {
        if (storedMilkList.Count == 0)
        {
            return 0f;
        }

        float totalFreshness = 0;
        foreach (Milk milk in storedMilkList)
        {
            totalFreshness += milk.freshness;
        }

        return totalFreshness / storedMilkList.Count;
    }

    /// <summary>
    /// 상인에게 판매할 우유를 창고에서 제거합니다.
    /// (신선도가 높은 우유부터 판매)
    /// </summary>
    /// <param name="amount">제거할 우유 개수</param>
    public void RemoveMilk(int amount)
    {
        if (amount > storedMilkList.Count)
        {
            Debug.LogError("창고에 판매할 우유가 부족합니다!");
            return;
        }

        storedMilkList.Sort((a, b) => b.freshness.CompareTo(a.freshness));
        storedMilkList.RemoveRange(0, amount);
    }

    /// <summary>
    /// 창고에서 특정 개수만큼 달걀을 제거합니다.
    /// </summary>
    public void RemoveEggs(int amount)
    {
        if (amount > storedEggFreshness.Count)
        {
            Debug.LogError("창고에 판매할 달걀이 부족합니다!");
            return;
        }

        storedEggFreshness.RemoveRange(0, amount);
    }
}