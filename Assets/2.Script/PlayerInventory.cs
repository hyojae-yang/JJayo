using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static PlayerInventory Instance { get; private set; }

    [Header("바구니 설정")]
    [Tooltip("바구니의 최대 용량.")]
    public int basketCapacity = 0; // 초기 용량을 0으로 설정
    [Tooltip("현재 바구니에 담긴 달걀의 개수.")]
    public int currentEggs = 0;
    public int basketLevel = 0; // 초기 레벨을 0으로 설정

    [Header("착유기 설정")]
    [Tooltip("착유기의 최대 용량.")]
    public int milkerCapacity = 0; // 초기 용량을 0으로 설정
    [Tooltip("현재 착유기에 담긴 우유들의 신선도 목록.")]
    public List<float> currentMilkFreshness = new List<float>();
    public float milkerSpeed = 0f; // 초기 속도를 0으로 설정
    public int milkerLevel = 0; // 초기 레벨을 0으로 설정

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

    public void TransferToWarehouse()
    {
        NotificationManager.Instance.ShowNotification($"TransferToWarehouse 함수 호출됨. 현재 알: {currentEggs}개, 현재 우유: {currentMilkFreshness.Count}개");
        if (currentEggs > 0)
        {
            // 임시로 우유 신선도 100f을 전달
            // Warehouse.Instance.AddEggs(currentEggs);
            currentEggs = 0;
            NotificationManager.Instance.ShowNotification("바구니의 알을 모두 창고로 옮겼습니다!");
        }

        if (currentMilkFreshness.Count > 0)
        {
            // Warehouse.Instance.AddMilk(currentMilkFreshness);
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
}