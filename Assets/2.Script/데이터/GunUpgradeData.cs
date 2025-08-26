// GunLevelStats.cs

using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class GunLevelStats : UpgradeLevelStats
{
    public float damageIncrease;
}

// GunUpgradeData.cs

[CreateAssetMenu(fileName = "New Gun Upgrade Data", menuName = "Tycoon Game/Upgrade Data/Gun")]
public class GunUpgradeData : UpgradeData
{
    public List<GunLevelStats> upgradeLevels;
}