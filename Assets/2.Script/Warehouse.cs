using System.Collections.Generic;
using UnityEngine;

public class Warehouse : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static Warehouse Instance { get; private set; }

    [Header("창고 설정")]
    [Tooltip("창고에 보관된 모든 달걀의 신선도 목록.")]
    public List<float> storedEggFreshness = new List<float>();

    [Tooltip("창고에 보관된 모든 우유의 신선도 목록.")]
    public List<float> storedMilkFreshness = new List<float>();

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
    /// 창고에 달걀을 추가하고 신선도를 평균값으로 계산합니다.
    /// </summary>
    /// <param name="newEggsFreshness">추가할 달걀들의 신선도 리스트</param>
    public void AddEggs(List<float> newEggsFreshness)
    {
        // 1. 기존 신선도 총합 계산
        float oldTotalFreshness = 0;
        foreach (float freshness in storedEggFreshness)
        {
            oldTotalFreshness += freshness;
        }

        // 2. 새로 들어올 달걀의 신선도 총합 계산
        float newTotalFreshness = 0;
        foreach (float freshness in newEggsFreshness)
        {
            newTotalFreshness += freshness;
        }

        // 3. 새로운 평균 신선도 계산
        float newAverageFreshness = (oldTotalFreshness + newTotalFreshness) / (storedEggFreshness.Count + newEggsFreshness.Count);

        // 4. 새로운 달걀 추가 및 전체 신선도 갱신
        storedEggFreshness.AddRange(newEggsFreshness);

        // 새로운 평균값으로 전체 달걀의 신선도 갱신
        for (int i = 0; i < storedEggFreshness.Count; i++)
        {
            storedEggFreshness[i] = newAverageFreshness;
        }

        NotificationManager.Instance.ShowNotification($"창고에 달걀 {newEggsFreshness.Count}개를 추가했습니다. 현재 총 달걀: {storedEggFreshness.Count}개, 평균 신선도: {newAverageFreshness:F2}");
    }

    /// <summary>
    /// 창고에 우유를 추가하고 신선도를 평균값으로 계산합니다.
    /// </summary>
    /// <param name="newMilkFreshness">추가할 우유들의 신선도 리스트</param>
    public void AddMilk(List<float> newMilkFreshness)
    {
        // 1. 기존 신선도 총합 계산
        float oldTotalFreshness = 0;
        foreach (float freshness in storedMilkFreshness)
        {
            oldTotalFreshness += freshness;
        }

        // 2. 새로 들어올 우유의 신선도 총합 계산
        float newTotalFreshness = 0;
        foreach (float freshness in newMilkFreshness)
        {
            newTotalFreshness += freshness;
        }

        // 3. 새로운 평균 신선도 계산
        float newAverageFreshness = (oldTotalFreshness + newTotalFreshness) / (storedMilkFreshness.Count + newMilkFreshness.Count);

        // 4. 새로운 우유 추가 및 전체 신선도 갱신
        storedMilkFreshness.AddRange(newMilkFreshness);

        // 새로운 평균값으로 전체 우유의 신선도 갱신
        for (int i = 0; i < storedMilkFreshness.Count; i++)
        {
            storedMilkFreshness[i] = newAverageFreshness;
        }

        NotificationManager.Instance.ShowNotification($"창고에 우유 {newMilkFreshness.Count}개를 추가했습니다. 현재 총 우유: {storedMilkFreshness.Count}개, 평균 신선도: {newAverageFreshness:F2}");
    }

    /// <summary>
    /// 창고에 있는 모든 아이템의 신선도를 감소시킵니다.
    /// </summary>
    private void DecayFreshness()
    {
        for (int i = 0; i < storedEggFreshness.Count; i++)
        {
            storedEggFreshness[i] = Mathf.Max(0, storedEggFreshness[i] - 1);
        }

        for (int i = 0; i < storedMilkFreshness.Count; i++)
        {
            storedMilkFreshness[i] = Mathf.Max(0, storedMilkFreshness[i] - 1);
        }

        NotificationManager.Instance.ShowNotification("창고에 있는 모든 아이템의 신선도가 감소했습니다.");
    }
    // **********************************************
    // ★★★ 상인 시스템 연동 함수 추가 시작 ★★★
    // **********************************************

    /// <summary>
    /// 창고에 있는 우유의 총 개수를 반환합니다.
    /// </summary>
    public int GetMilkCount()
    {
        return storedMilkFreshness.Count;
    }

    /// <summary>
    /// 창고에 있는 모든 우유의 평균 신선도를 계산하여 반환합니다.
    /// </summary>
    public float GetAverageMilkFreshness()
    {
        if (storedMilkFreshness.Count == 0)
        {
            return 0f;
        }

        float totalFreshness = 0;
        foreach (float freshness in storedMilkFreshness)
        {
            totalFreshness += freshness;
        }

        return totalFreshness / storedMilkFreshness.Count;
    }

    /// <summary>
    /// 상인에게 판매할 우유를 창고에서 제거합니다.
    /// (신선도가 높은 우유부터 판매)
    /// </summary>
    /// <param name="amount">제거할 우유 개수</param>
    public void RemoveMilk(int amount)
    {
        if (amount > storedMilkFreshness.Count)
        {
            Debug.LogError("창고에 판매할 우유가 부족합니다!");
            return;
        }

        // 1. 우유를 신선도 순으로 정렬 (내림차순)
        storedMilkFreshness.Sort((a, b) => b.CompareTo(a));

        // 2. 가장 신선한 우유부터 판매량만큼 제거
        storedMilkFreshness.RemoveRange(0, amount);
    }

    // **********************************************
    // ★★★ 상인 시스템 연동 함수 추가 끝 ★★★
    // **********************************************
}