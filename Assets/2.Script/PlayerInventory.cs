using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static PlayerInventory Instance { get; private set; }

    [Header("바구니 설정")]
    [Tooltip("바구니의 최대 용량.")]
    public int basketCapacity = 10;
    [Tooltip("현재 바구니에 담긴 달걀의 개수.")]
    public int currentEggs = 0;

    [Header("착유기 설정")]
    [Tooltip("착유기의 최대 용량.")]
    public int milkerCapacity = 10;
    [Tooltip("현재 착유기에 담긴 우유들의 신선도 목록.")]
    public List<float> currentMilkFreshness = new List<float>();

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
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

    /// <summary>
    /// 바구니에 달걀을 추가합니다. 바구니 용량을 초과할 수 없습니다.
    /// </summary>
    /// <param name="amount">추가할 달걀의 개수</param>
    /// <returns>실제로 추가된 달걀의 개수</returns>
    public int AddEggs(int amount)
    {
        int spaceLeft = basketCapacity - currentEggs;
        int eggsToAdd = Mathf.Min(amount, spaceLeft);

        currentEggs += eggsToAdd;
        Debug.Log($"바구니에 달걀 {eggsToAdd}개를 담았습니다. 현재: {currentEggs}/{basketCapacity}");

        return eggsToAdd;
    }

    /// <summary>
    /// 바구니에서 달걀을 제거합니다.
    /// </summary>
    /// <param name="amount">제거할 달걀의 개수</param>
    public void RemoveEggs(int amount)
    {
        currentEggs = Mathf.Max(0, currentEggs - amount);
        Debug.Log($"바구니에서 달걀 {amount}개를 꺼냈습니다. 현재: {currentEggs}/{basketCapacity}");
    }

    /// <summary>
    /// 착유기에 우유를 추가합니다. 착유기 용량을 초과할 수 없습니다.
    /// </summary>
    /// <param name="freshness">추가할 우유의 신선도</param>
    /// <returns>실제로 추가된 우유의 개수</returns>
    public int AddMilk(float freshness)
    {
        if (currentMilkFreshness.Count < milkerCapacity)
        {
            currentMilkFreshness.Add(freshness);
            Debug.Log($"착유기에 신선도 {freshness:F2}의 우유를 담았습니다. 현재: {currentMilkFreshness.Count}/{milkerCapacity}");
            return 1;
        }

        Debug.Log("착유기가 꽉 찼습니다!");
        return 0;
    }

    /// <summary>
    /// 착유기와 바구니의 모든 내용물을 창고로 옮깁니다.
    /// </summary>
    public void TransferToWarehouse()
    {
        // 현재 인벤토리 상태를 콘솔에 출력합니다.
        Debug.Log($"TransferToWarehouse 함수 호출됨. 현재 알: {currentEggs}개, 현재 우유: {currentMilkFreshness.Count}개");

        // 바구니에 알이 있으면 창고로 옮깁니다.
        if (currentEggs > 0)
        {
            List<float> eggFreshnessList = new List<float>();
            for (int i = 0; i < currentEggs; i++)
            {
                eggFreshnessList.Add(100f);
            }
            Warehouse.Instance.AddEggs(eggFreshnessList);
            currentEggs = 0; // 바구니 비우기
            Debug.Log("바구니의 알을 모두 창고로 옮겼습니다!");
        }

        // 착유기에 우유가 있으면 창고로 옮깁니다.
        if (currentMilkFreshness.Count > 0)
        {
            Warehouse.Instance.AddMilk(currentMilkFreshness);
            currentMilkFreshness.Clear(); // 착유기 비우기
            Debug.Log("착유기의 우유를 모두 창고로 옮겼습니다!");
        }
    }
}