using UnityEngine;
using System.Collections.Generic;
using System; // Action 타입을 사용하기 위해 추가

public class PlayerInventory : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static PlayerInventory Instance { get; private set; }

    // **********************************************
    // ★★★ 새로 추가된 부분 ★★★
    // 용량이 변경될 때 다른 스크립트(PlayerUI)에게 알리는 이벤트
    public event Action OnCapacityChanged;
    // **********************************************

    [Header("바구니 설정")]
    [Tooltip("바구니의 최대 용량.")]
    public int basketCapacity = 0;
    [Tooltip("현재 바구니에 담긴 달걀의 개수.")]
    public int currentEggs = 0;
    public int basketLevel = 0;

    [Header("착유기 설정")]
    [Tooltip("착유기의 최대 용량.")]
    public int milkerCapacity = 0;
    [Tooltip("현재 착유기에 담긴 우유들의 신선도 목록.")]
    public List<float> currentMilkFreshness = new List<float>();
    public float milkerSpeed = 0f;
    public int milkerLevel = 0;

    [Header("총기 설정")]
    public bool hasGun = false;
    public int currentBullets = 0;
    public int gunLevel = 0;
    public float gunDamage = 10f;
    public float gunFireRate = 1.0f;

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

    // 상점에서 업그레이드 시 이 함수들을 호출해야 합니다.

    public void SetBasketCapacity(int newCapacity, int newLevel)
    {
        basketCapacity = newCapacity;
        basketLevel = newLevel;
        // 용량이 변경되었으니 이벤트 발생! (UI에게 알림)
        OnCapacityChanged?.Invoke();
    }

    public void SetMilkerCapacity(int newCapacity, int newLevel, float newSpeed)
    {
        milkerCapacity = newCapacity;
        milkerLevel = newLevel;
        milkerSpeed = newSpeed;
        // 용량이 변경되었으니 이벤트 발생! (UI에게 알림)
        OnCapacityChanged?.Invoke();
    }

    public int AddEggs(int amount)
    {
        if (basketLevel == 0) // 바구니가 없을 경우
        {
            NotificationManager.Instance.ShowNotification("바구니를 먼저 구매해야 합니다!");
            return 0;
        }

        int spaceLeft = basketCapacity - currentEggs;
        int eggsToAdd = Mathf.Min(amount, spaceLeft);
        currentEggs += eggsToAdd;
        NotificationManager.Instance.ShowNotification($"바구니에 달걀 {eggsToAdd}개를 담았습니다. 현재: {currentEggs}/{basketCapacity}");
        return eggsToAdd;
    }

    public void RemoveEggs(int amount)
    {
        currentEggs = Mathf.Max(0, currentEggs - amount);
        NotificationManager.Instance.ShowNotification($"바구니에서 달걀 {amount}개를 꺼냈습니다. 현재: {currentEggs}/{basketCapacity}");
    }

    public int AddMilk(float freshness)
    {
        if (milkerLevel == 0) // 착유기가 없을 경우
        {
            NotificationManager.Instance.ShowNotification("착유기를 먼저 구매해야 합니다!");
            return 0;
        }

        if (currentMilkFreshness.Count < milkerCapacity)
        {
            currentMilkFreshness.Add(freshness);
            NotificationManager.Instance.ShowNotification($"착유기에 신선도 {freshness:F2}의 우유를 담았습니다. 현재: {currentMilkFreshness.Count}/{milkerCapacity}");
            return 1;
        }
        NotificationManager.Instance.ShowNotification("착유기가 꽉 찼습니다!");
        return 0;
    }

    // ★★★ 이 함수가 수정되었습니다. ★★★
    public void TransferToWarehouse()
    {
        NotificationManager.Instance.ShowNotification($"현재 알: {currentEggs}개, 현재 우유: {currentMilkFreshness.Count}개");

        // 알을 창고로 옮기는 로직
        if (currentEggs > 0)
        {
            List<float> eggsToTransferFreshness = new List<float>();
            for (int i = 0; i < currentEggs; i++)
            {
                eggsToTransferFreshness.Add(100f);
            }

            // 실제로 창고로 알을 옮깁니다.
            if (Warehouse.Instance != null)
            {
                Warehouse.Instance.AddEggs(eggsToTransferFreshness);
            }

            currentEggs = 0;
            NotificationManager.Instance.ShowNotification("바구니의 알을 모두 창고로 옮겼습니다!");
        }

        // 우유를 창고로 옮기는 로직
        if (currentMilkFreshness.Count > 0)
        {
            // PlayerInventory에 있는 우유 신선도 목록을 그대로 창고로 옮깁니다.
            if (Warehouse.Instance != null)
            {
                Warehouse.Instance.AddMilk(currentMilkFreshness);
            }

            currentMilkFreshness.Clear();
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
        NotificationManager.Instance.ShowNotification($"총알 {amount}개를 획득했습니다! 현재 총알: {currentBullets}");
    }

    // **********************************************
    // ★★★ 상인 시스템 연동 함수 시작 ★★★
    // **********************************************

    /// <summary>
    /// 창고에 저장된 우유의 개수를 반환합니다.
    /// (우유 판매는 창고의 우유를 기준으로 합니다.)
    /// </summary>
    public int GetMilkCount()
    {
        if (Warehouse.Instance != null)
        {
            return Warehouse.Instance.GetMilkCount();
        }
        return 0;
    }

    /// <summary>
    /// 창고에 저장된 우유들의 평균 신선도를 반환합니다.
    /// </summary>
    public float GetAverageFreshness()
    {
        if (Warehouse.Instance != null)
        {
            return Warehouse.Instance.GetAverageMilkFreshness();
        }
        return 0f;
    }

    /// <summary>
    /// 상인의 요구 조건을 충족하여 우유를 판매할 수 있는지 확인합니다.
    /// </summary>
    /// <param name="requiredAmount">상인이 요구하는 우유 개수</param>
    /// <param name="requiredFreshness">상인이 요구하는 최소 신선도</param>
    /// <returns>판매 가능 여부</returns>
    public bool CanSellMilk(int requiredAmount, float requiredFreshness)
    {
        // 1. 개수 충족 확인
        if (GetMilkCount() < requiredAmount)
        {
            NotificationManager.Instance.ShowNotification("창고에 우유가 부족합니다!");
            return false;
        }

        // 2. 신선도 충족 확인 (평균 신선도로 확인)
        if (GetAverageFreshness() < requiredFreshness)
        {
            NotificationManager.Instance.ShowNotification("우유의 평균 신선도가 낮아 상인의 요구 조건을 충족하지 못합니다.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 상인에게 우유를 판매하고 창고에서 우유를 제거합니다.
    /// </summary>
    /// <param name="amount">판매할 우유 개수</param>
    public void SellMilk(int amount)
    {
        if (Warehouse.Instance != null)
        {
            Warehouse.Instance.RemoveMilk(amount);
            NotificationManager.Instance.ShowNotification($"우유 {amount}개를 상인에게 판매했습니다!");
        }
    }

    // **********************************************
    // ★★★ 상인 시스템 연동 함수 끝 ★★★
    // **********************************************
}