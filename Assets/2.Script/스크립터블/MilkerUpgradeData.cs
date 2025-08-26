using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MilkerLevelStats : UpgradeLevelStats
{
    public int capacity;
    // �ڡڡ� ������ �κ�: speed �������� milkingYield�� �����մϴ�. �ڡڡ�
    public int milkingYield;
}

[CreateAssetMenu(fileName = "New Milker Upgrade Data", menuName = "Tycoon Game/Upgrade Data/Milker")]
public class MilkerUpgradeData : UpgradeData
{
    public List<MilkerLevelStats> upgradeLevels;
}