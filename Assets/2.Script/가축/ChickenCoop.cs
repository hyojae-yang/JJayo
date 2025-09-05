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
        if (GameManager.Instance.IsMenuOn) return;
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
}