using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Warehouse : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static Warehouse Instance { get; private set; }

    [Header("창고 설정")]
    [Tooltip("창고에 보관된 모든 달걀의 신선도 목록.")]
    public List<float> storedEggFreshness = new List<float>(); // 달걀 신선도 목록

    [Tooltip("창고에 보관된 모든 우유 목록.")]
    public List<Milk> storedMilkList = new List<Milk>(); // 우유 목록 (신선도 감소 로직 제외)

    [Tooltip("아이템의 신선도가 감소하는 주기(초).")]
    public float freshnessDecayInterval = 90f;
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
        // 신선도 감소 타이머 업데이트 (달걀만 해당)
        decayTimer += Time.deltaTime;
        if (decayTimer >= freshnessDecayInterval)
        {
            DecayFreshness();
            decayTimer = 0f;
        }
    }

    // 새로운 달걀을 창고에 추가하는 메서드
    public void AddEggs(List<float> newEggs)
    {
        storedEggFreshness.AddRange(newEggs);
    }

    // 새로운 우유를 창고에 추가하는 메서드
    public void AddMilk(List<Milk> newMilkList)
    {
        storedMilkList.AddRange(newMilkList);
    }

    // 아이템 신선도 감소 로직 (달걀만 해당)
    private void DecayFreshness()
    {
        // 창고 레벨에 따른 신선도 감소 배율 가져오기
        int warehouseLevel = GameManager.Instance.CurrentGameData.warehouseLevel;
        float freshnessDecayMultiplier = 1.0f;

        // 창고 업그레이드 데이터에서 배율 가져오기
        var warehouseUpgradeItem = ShopService.Instance.GetShopItems()
            .FirstOrDefault(item => item.itemType == ItemType.Upgrade && item.upgradeData is WarehouseUpgradeData);

        if (warehouseUpgradeItem != null)
        {
            var warehouseUpgradeData = warehouseUpgradeItem.upgradeData as WarehouseUpgradeData;
            freshnessDecayMultiplier = warehouseUpgradeData.GetFreshnessDecayMultiplier(warehouseLevel);
        }

        float decayAmount = 1f * freshnessDecayMultiplier; // 실제 감소량 계산

        // 달걀 신선도 감소
        for (int i = storedEggFreshness.Count - 1; i >= 0; i--)
        {
            storedEggFreshness[i] = Mathf.Max(0, storedEggFreshness[i] - decayAmount);
            if (storedEggFreshness[i] <= 0)
            {
                storedEggFreshness.RemoveAt(i);
            }
        }

        // 우유 신선도 감소 로직은 제거되었습니다.
        // NotificationManager.Instance.ShowNotification("창고에 있는 모든 아이템의 신선도가 감소했습니다."); // 이 알림도 달걀만 해당되도록 수정할 수 있습니다.
        // 달걀만 해당된다는 알림으로 수정하거나, 아니면 이 줄을 삭제해도 좋습니다.
        NotificationManager.Instance.ShowNotification("창고에 있는 달걀의 신선도가 감소했습니다.");
    }

    // 창고에 있는 달걀의 총 개수 반환
    public int GetEggCount()
    {
        return storedEggFreshness.Count;
    }

    // 창고에 있는 우유의 총 개수 반환
    public int GetMilkCount()
    {
        return storedMilkList.Count;
    }

    // 창고에 있는 우유의 평균 신선도 반환 (신선도 감소 로직 제외)
    public float GetAverageMilkFreshness()
    {
        return storedMilkList.Any() ? storedMilkList.Average(m => m.freshness) : 0f;
    }

    // 창고에 있는 달걀의 평균 신선도 반환
    public float GetAverageEggFreshness()
    {
        return storedEggFreshness.Any() ? storedEggFreshness.Average() : 0f;
    }

    // 판매를 위해 우유 제거 (신선도 낮은 것부터)
    public void RemoveMilk(int amount)
    {
        if (amount > storedMilkList.Count)
        {
            Debug.LogError("창고에 판매할 우유가 부족합니다!");
            return;
        }
        // 우유는 신선도 감소 로직이 없으므로, 단순히 먼저 들어온 순서대로 제거해도 무방합니다.
        // 만약 '가장 오래된 우유'부터 판매해야 한다면, Milk 클래스에 추가된 '생산 시간' 등으로 정렬해야 합니다.
        // 현재 요구사항에 따라 신선도 기반 정렬은 제거했습니다.
        storedMilkList.RemoveRange(0, amount);
    }

    // 판매를 위해 달걀 제거 (신선도 낮은 것부터)
    public void RemoveEggs(int amount)
    {
        if (amount > storedEggFreshness.Count)
        {
            Debug.LogError("창고에 판매할 달걀이 부족합니다!");
            return;
        }
        storedEggFreshness.Sort(); // 신선도 낮은 순으로 정렬
        storedEggFreshness.RemoveRange(0, amount);
    }

    // 우유 판매 가능 여부 확인 (TraderManager에서 호출)
    public bool CanSellMilk(int requiredAmount, float requiredFreshness)
    {
        if (GetMilkCount() < requiredAmount)
        {
            return false;
        }

        // 우유 신선도 조건 체크는 필요에 따라 유지하거나 제거할 수 있습니다.
        // 현재는 신선도 감소 로직이 없으므로, 항상 최대 신선도라고 가정해도 됩니다.
        // 만약 우유별로 개별적인 신선도 값이 있다면, 이 부분을 수정해야 합니다.
        // 지금은 일단 요구되는 평균 신선도보다 현재 평균 신선도가 높거나 같으면 판매 가능하도록 유지합니다.
        if (GetAverageMilkFreshness() < requiredFreshness)
        {
            return false;
        }
        return true;
    }

    // 달걀 판매 가능 여부 확인 (TraderManager에서 호출)
    public bool CanSellEggs(int requiredAmount)
    {
        return GetEggCount() >= requiredAmount;
    }

    // 우유 판매 처리 (TraderManager에서 호출)
    public void SellMilk(int amount)
    {
        SoundManager.Instance.PlaySFX(SFXType.Button_Click);
        // CanSellMilk에서 신선도 조건도 체크하므로, 여기서 다시 amount만 확인합니다.
        if (CanSellMilk(amount, 0))
        {
            // 우유 신선도 감소 로직이 제거되었으므로, 단순히 개수만 줄입니다.
            // (추후 우유별 생산 시간 등을 기반으로 판매할 우유를 결정해야 한다면 이 부분 수정 필요)
            storedMilkList.RemoveRange(0, amount);
            GameManager.Instance.CurrentGameData.totalMilkSold += amount;
            NotificationManager.Instance.ShowNotification($"우유 {amount}개를 상인에게 판매했습니다!");
        }
        else
        {
            Debug.LogWarning("우유를 판매할 수 없습니다."); // 판매 불가 시 알림
        }
    }

    // 달걀 판매 처리 (TraderManager에서 호출)
    public void SellEggs(int amount)
    {
        SoundManager.Instance.PlaySFX(SFXType.Button_Click);
        if (CanSellEggs(amount))
        {
            storedEggFreshness.Sort(); // 신선도 낮은 달걀부터 정렬
            storedEggFreshness.RemoveRange(0, amount);
            GameManager.Instance.CurrentGameData.totalEggsSold += amount;
            NotificationManager.Instance.ShowNotification($"달걀 {amount}개를 상인에게 판매했습니다!");
        }
        else
        {
            Debug.LogWarning("달걀을 판매할 수 없습니다."); // 판매 불가 시 알림
        }
    }
}