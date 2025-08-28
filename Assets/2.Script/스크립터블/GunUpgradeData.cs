using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GunLevelStats
{
    public int level;
    public int upgradePrice;
    public float damage;
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

    public float GetDamage(int level)
    {
        if (level < 0 || level >= upgradeLevels.Count)
        {
            return 0;
        }
        return upgradeLevels[level].damage;
    }

    public override int GetMaxLevel()
    {
        return upgradeLevels.Count;
    }
}