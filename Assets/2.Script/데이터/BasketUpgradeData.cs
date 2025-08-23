using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BasketLevelStats : UpgradeLevelStats
{
    public int capacity;
}

[CreateAssetMenu(fileName = "New Basket Upgrade Data", menuName = "Tycoon Game/Upgrade Data/Basket")]
public class BasketUpgradeData : UpgradeData
{
    public List<BasketLevelStats> upgradeLevels;
}