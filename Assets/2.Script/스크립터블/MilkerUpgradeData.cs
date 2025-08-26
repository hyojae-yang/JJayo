using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MilkerLevelStats : UpgradeLevelStats
{
    public int capacity;
    // ★★★ 수정된 부분: speed 변수명을 milkingYield로 변경합니다. ★★★
    public int milkingYield;
}

[CreateAssetMenu(fileName = "New Milker Upgrade Data", menuName = "Tycoon Game/Upgrade Data/Milker")]
public class MilkerUpgradeData : UpgradeData
{
    public List<MilkerLevelStats> upgradeLevels;
}