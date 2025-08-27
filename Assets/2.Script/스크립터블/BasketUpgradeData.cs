using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BasketLevelStats
{
    public int upgradePrice;
    public int capacity;
}

[CreateAssetMenu(fileName = "New Basket Upgrade Data", menuName = "Tycoon Game/Upgrade Data/Basket")]
public class BasketUpgradeData : UpgradeData
{
    public List<BasketLevelStats> upgradeLevels;

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