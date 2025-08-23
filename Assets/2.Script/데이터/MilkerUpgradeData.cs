using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MilkerLevelStats : UpgradeLevelStats
{
    public int capacity;
    public float speed;
}

[CreateAssetMenu(fileName = "New Milker Upgrade Data", menuName = "Tycoon Game/Upgrade Data/Milker")]
public class MilkerUpgradeData : UpgradeData
{
    public List<MilkerLevelStats> upgradeLevels;
}