using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PastureLevelStats : UpgradeLevelStats // UpgradeLevelStats ����� ����
{
    // MilkerLevelStatsó�� ���� �Ӽ��� �����ϴ�.
    [Tooltip("�ּ� �ż��� �� (��: 20)")]
    [Range(0, 100)] public int minFreshness;
    [Tooltip("�ִ� �ż��� �� (��: 30)")]
    [Range(0, 100)] public int maxFreshness;
}

[CreateAssetMenu(fileName = "New Pasture Upgrade Data", menuName = "Tycoon Game/Upgrade Data/Pasture")]
public class PastureUpgradeData : UpgradeData // UpgradeData ��� ����
{
    public List<PastureLevelStats> upgradeLevels = new List<PastureLevelStats>();

    // �ٽ�: GetNextUpgradePrice() �Լ��� �����Ͽ� CS0115 ������ �ذ��մϴ�.
    // ���� ��ũ��Ʈ��ó�� �� �Լ��� ����� ���� �۵��մϴ�.

    /// <summary>
    /// Ư�� ������ �ż��� ������ ��ȯ�մϴ�.
    /// </summary>
    public (int min, int max) GetFreshnessRange(int currentLevel)
    {
        if (currentLevel < 0 || upgradeLevels.Count == 0)
        {
            Debug.LogError("Pasture data is invalid or level is out of range.");
            return (0, 0);
        }

        int index = Mathf.Clamp(currentLevel, 0, upgradeLevels.Count - 1);

        return (upgradeLevels[index].minFreshness, upgradeLevels[index].maxFreshness);
    }
}