using UnityEngine;
using TMPro; // TMP�� ����ϱ� ���� �߰�
using System;

public class PastureManager : MonoBehaviour
{
    public static PastureManager Instance { get; private set; }

    [Header("Dependencies")]
    public GameManager gameManager;
    public Camera mainCamera;

    [Header("Pasture Data")]
    public PastureUpgradeData pastureUpgradeData;

    // �ܺο��� ������ ������ ���� �� �ֵ��� public ������Ƽ�� �߰��մϴ�.
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
        // GameManager �ν��Ͻ��� Start���� �����ɴϴ�.
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }

        // ī�޶� ã�Ƽ� �Ҵ��մϴ�.
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // ���� ���� �� �ʱ� �ð��� ������Ʈ�� �����մϴ�.
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

        // UpgradePastureData�� ���� ������ Ȯ���մϴ�.
        int nextLevel = gameManager.gameData.pastureLevel + 1;

        if (nextLevel < pastureUpgradeData.upgradeLevels.Count)
        {
            gameManager.gameData.pastureLevel = nextLevel;
            Debug.Log($"Pasture upgraded to level {gameManager.gameData.pastureLevel}.");

            // ���׷��̵� �� �ð��� ������Ʈ�� ȣ���մϴ�.
            UpdateVisuals();
        }
        else
        {
            Debug.LogWarning("Pasture is already at max level.");
        }
    }

    /// <summary>
    /// ���� ������ ���� ī�޶� ������ ������Ʈ�մϴ�. (public���� ����)
    /// </summary>
    public void UpdateVisuals()
    {
        if (mainCamera != null && pastureColors.Length > gameManager.gameData.pastureLevel)
        {
            mainCamera.backgroundColor = pastureColors[gameManager.gameData.pastureLevel];
        }
    }
}