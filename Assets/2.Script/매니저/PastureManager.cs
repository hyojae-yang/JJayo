using UnityEngine;
using TMPro; // TMP를 사용하기 위해 추가
using System;

public class PastureManager : MonoBehaviour
{
    public static PastureManager Instance { get; private set; }

    [Header("Dependencies")]
    public GameManager gameManager;
    public Camera mainCamera;

    [Header("Pasture Data")]
    public PastureUpgradeData pastureUpgradeData;

    // 외부에서 목초지 레벨을 읽을 수 있도록 public 프로퍼티를 추가합니다.
    public int CurrentPastureLevel => gameManager.gameData.pastureLevel;

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

    void Start()
    {
        // GameManager 인스턴스를 Start에서 가져옵니다.
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }

        // 카메라를 찾아서 할당합니다.
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // 게임 시작 시 초기 시각적 업데이트를 수행합니다.
        UpdateVisuals();
    }

    /// <summary>
    /// Upgrades the pasture by 1 level.
    /// </summary>
    public void UpgradePasture()
    {
        if (pastureUpgradeData == null)
        {
            Debug.LogError("PastureUpgradeData is not assigned to the PastureManager.");
            return;
        }

        // UpgradePastureData의 다음 레벨을 확인합니다.
        int nextLevel = gameManager.gameData.pastureLevel + 1;

        if (nextLevel < pastureUpgradeData.upgradeLevels.Count)
        {
            gameManager.gameData.pastureLevel = nextLevel;
            Debug.Log($"Pasture upgraded to level {gameManager.gameData.pastureLevel}.");

            // 업그레이드 후 시각적 업데이트를 호출합니다.
            UpdateVisuals();
        }
        else
        {
            Debug.LogWarning("Pasture is already at max level.");
        }
    }

    /// <summary>
    /// 목장 레벨에 따라 카메라 색상을 업데이트합니다. (public으로 변경)
    /// </summary>
    public void UpdateVisuals()
    {
        if (mainCamera != null && pastureColors.Length > gameManager.gameData.pastureLevel)
        {
            mainCamera.backgroundColor = pastureColors[gameManager.gameData.pastureLevel];
        }
    }
}