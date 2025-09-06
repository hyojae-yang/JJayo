using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;

public class ChickenCoop : MonoBehaviour
{
    public static ChickenCoop Instance { get; private set; }

    [Header("닭장 고유 데이터")]
    public ChickenCoopData chickenCoopData;
    [Tooltip("목초지 업그레이드 데이터를 연결하세요.")]
    public PastureUpgradeData pastureUpgradeData; // ★★★ PastureUpgradeData 참조 추가 ★★★

    private float productionTimer = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            AnimalHandler handler = FindFirstObjectByType<AnimalHandler>();
            if (handler != null)
            {
                handler.RegisterChickenCoop(this);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentGameData == null) return;

        if (GameManager.Instance.CurrentGameData.chickenCount <= 0)
        {
            return;
        }

        productionTimer += Time.deltaTime * GameManager.Instance.CurrentGameData.chickenCount;

        if (productionTimer >= chickenCoopData.eggProductionInterval)
        {
            int currentLevel = GameManager.Instance.CurrentGameData.pastureLevel;
            // ★★★ GetFreshnessRange() 호출 시 결과를 변수에 저장 ★★★
            var freshnessRange = pastureUpgradeData.GetFreshnessRange(currentLevel);

            // ★★★ 추가: 신선도 범위를 2배로 만들기 위해 min과 max 값에 2를 곱합니다. ★★★
            float minFreshness = Mathf.Clamp(freshnessRange.min * 2f, 0f, 100f);
            float maxFreshness = Mathf.Clamp(freshnessRange.max * 2f, 0f, 100f);

            // ★★★ 수정: 2배로 계산된 신선도 범위를 사용하여 난수 생성 ★★★
            float newEggFreshness = UnityEngine.Random.Range(minFreshness, maxFreshness);
            GameManager.Instance.CurrentGameData.savedChickenEggs.Add(newEggFreshness);

            productionTimer = 0f;

            if (GameManager.Instance != null && GameManager.Instance.CurrentGameData != null)
            {
                GameManager.Instance.CurrentGameData.dailyEggsProduced++;
            }
        }
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.IsMenuOn) return;
        if (EquipmentManager.Instance.GetCurrentEquipment() == EquipmentType.Basket)
        {
            // ★★★ 수정된 부분: 저장된 달걀 리스트를 PlayerInventory로 전달 ★★★
            if (GameManager.Instance.CurrentGameData.savedChickenEggs.Count > 0)
            {
                int eggsTransferred = PlayerInventory.Instance.AddEggs(new List<float>(GameManager.Instance.CurrentGameData.savedChickenEggs));
                GameManager.Instance.CurrentGameData.savedChickenEggs.Clear(); // 닭장에 있는 알 리스트를 비움

                NotificationManager.Instance.ShowNotification($"바구니에 알 {eggsTransferred}개를 담았습니다. 닭장에 남은 알: {GameManager.Instance.CurrentGameData.savedChickenEggs.Count}");
                productionTimer = 0f;
            }
        }
    }
}