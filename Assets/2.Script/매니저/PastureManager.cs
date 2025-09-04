using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic; // List를 사용하기 위해 추가

public class PastureManager : MonoBehaviour
{
    public static PastureManager Instance { get; private set; }

    [Header("Dependencies")]
    private GameManager gameManager;
    private Camera mainCamera;

    [Header("Pasture Data")]
    public PastureUpgradeData pastureUpgradeData;

    public int CurrentPastureLevel
    {
        get
        {
            if (gameManager != null && gameManager.CurrentGameData != null)
            {
                return gameManager.CurrentGameData.pastureLevel;
            }
            return 0;
        }
    }

    [Header("Visual Feedback")]
    public Color[] pastureColors;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        gameManager = GameManager.Instance;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        UpdateVisuals();
    }

    public void UpgradePasture()
    {
        if (pastureUpgradeData == null)
        {
            Debug.LogError("PastureUpgradeData is not assigned to the PastureManager.");
            return;
        }

        if (gameManager == null || gameManager.CurrentGameData == null)
        {
            Debug.LogError("GameManager 또는 CurrentGameData가 초기화되지 않았습니다.");
            return;
        }

        int nextLevel = gameManager.CurrentGameData.pastureLevel + 1;

        if (nextLevel < pastureUpgradeData.upgradeLevels.Count)
        {
            gameManager.CurrentGameData.pastureLevel = nextLevel;
            Debug.Log($"Pasture upgraded to level {gameManager.CurrentGameData.pastureLevel}.");
            UpdateVisuals();
        }
        else
        {
            Debug.LogWarning("Pasture is already at max level.");
        }
    }

    public void UpdateVisuals()
    {
        if (mainCamera != null && gameManager != null && gameManager.CurrentGameData != null && pastureColors.Length > gameManager.CurrentGameData.pastureLevel)
        {
            mainCamera.backgroundColor = pastureColors[gameManager.CurrentGameData.pastureLevel];
        }
    }

    // ★★★ 새로 추가된 메서드 ★★★
    public float GetFreshnessBonus()
    {
        if (pastureUpgradeData == null)
        {
            Debug.LogError("PastureUpgradeData가 할당되지 않았습니다.");
            return 0f;
        }

        (int min, int max) range = pastureUpgradeData.GetFreshnessRange(CurrentPastureLevel);

        // 범위 내에서 무작위 값을 반환합니다.
        return UnityEngine.Random.Range((float)range.min, (float)range.max);
    }
}