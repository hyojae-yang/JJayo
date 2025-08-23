using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GunLevelStats : UpgradeLevelStats
{
    public float damageIncrease;
    public float fireRateIncrease;
}

[CreateAssetMenu(fileName = "New Gun Upgrade Data", menuName = "Tycoon Game/Upgrade Data/Gun")]
public class GunUpgradeData : UpgradeData
{
    public List<GunLevelStats> upgradeLevels;
}