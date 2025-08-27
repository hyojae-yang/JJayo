using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MilkerLevelStats
{
    public int upgradePrice;
    public int capacity;
    public int milkingYield;
}

[CreateAssetMenu(fileName = "New Milker Upgrade Data", menuName = "Tycoon Game/Upgrade Data/Milker")]
public class MilkerUpgradeData : UpgradeData
{
    public List<MilkerLevelStats> upgradeLevels;

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
}