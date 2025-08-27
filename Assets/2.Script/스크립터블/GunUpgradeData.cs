using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GunLevelStats
{
    public int upgradePrice;
    public float damageIncrease;
}

[CreateAssetMenu(fileName = "New Gun Upgrade Data", menuName = "Tycoon Game/Upgrade Data/Gun")]
public class GunUpgradeData : UpgradeData
{
    public List<GunLevelStats> upgradeLevels;

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