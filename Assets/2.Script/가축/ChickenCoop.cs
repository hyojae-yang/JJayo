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

    // ★★★ 삭제된 부분: 이 변수들은 이제 GameData에서 관리됩니다. ★★★
    // public int currentEggCount = 0;
    // public int numberOfChickens;

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

    void Start()
    {
        // numberOfChickens = 0;
    }

    void Update()
    {
        // ★★★ 수정된 부분: numberOfChickens 대신 GameData의 chickenCount를 사용합니다. ★★★
        if (GameManager.Instance == null || GameManager.Instance.CurrentGameData == null) return;

        if (GameManager.Instance.CurrentGameData.chickenCount <= 0)
        {
            return;
        }

        productionTimer += Time.deltaTime * GameManager.Instance.CurrentGameData.chickenCount;

        if (productionTimer >= chickenCoopData.eggProductionInterval)
        {
            // ★★★ 수정된 부분: currentEggCount 대신 GameData의 currentChickenEggCount를 사용합니다. ★★★
            GameManager.Instance.CurrentGameData.currentChickenEggCount++;
            productionTimer = 0f;

            if (GameManager.Instance != null && GameManager.Instance.CurrentGameData != null)
            {
                GameManager.Instance.CurrentGameData.dailyEggsProduced++;
            }
        }
    }

    void OnMouseDown()
    {
        if (EquipmentManager.Instance.GetCurrentEquipment() == EquipmentType.Basket)
        {
            // ★★★ 수정된 부분: currentEggCount 대신 GameData의 currentChickenEggCount를 사용합니다. ★★★
            if (GameManager.Instance.CurrentGameData.currentChickenEggCount > 0)
            {
                int eggsTransferred = PlayerInventory.Instance.AddEggs(GameManager.Instance.CurrentGameData.currentChickenEggCount);
                GameManager.Instance.CurrentGameData.currentChickenEggCount -= eggsTransferred;
                NotificationManager.Instance.ShowNotification($"바구니에 알 {eggsTransferred}개를 담았습니다. 닭장에 남은 알: {GameManager.Instance.CurrentGameData.currentChickenEggCount}");
                productionTimer = 0f;
            }
        }
    }

    // ★★★ 수정된 부분: 이 메서드들은 이제 GameData의 값을 직접 수정하도록 AnimalHandler에서 호출됩니다. ★★★
    public void AddChicken()
    {
        // 이제 이 메서드에서 직접 chickenCount를 수정하지 않습니다. AnimalHandler에서 GameData를 직접 수정합니다.
        // 이 메서드는 더 이상 AnimalHandler에서 호출되지 않습니다.
    }

    public void RemoveChicken()
    {
        // 이 메서드는 더 이상 AnimalHandler에서 호출되지 않습니다.
    }
}