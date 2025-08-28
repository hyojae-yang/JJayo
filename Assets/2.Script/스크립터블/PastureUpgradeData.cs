using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PastureLevelStats
{
    public int upgradePrice;
    [Tooltip("최소 신선도 값 (예: 20)")]
    [Range(0, 100)] public int minFreshness;
    [Tooltip("최대 신선도 값 (예: 30)")]
    [Range(0, 100)] public int maxFreshness;
    
}

[CreateAssetMenu(fileName = "New Pasture Upgrade Data", menuName = "Tycoon Game/Upgrade Data/Pasture")]
public class PastureUpgradeData : UpgradeData
{
    public List<PastureLevelStats> upgradeLevels;

    public override int GetUpgradePrice(int currentLevel)
    {
        if (currentLevel < 0 || currentLevel >= upgradeLevels.Count)
        {
            return 0;
        }
        return upgradeLevels[currentLevel].upgradePrice;
    }

    public override int GetMaxLevel()
    {
        return upgradeLevels.Count;
    }

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