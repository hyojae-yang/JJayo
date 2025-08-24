using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PastureLevelStats : UpgradeLevelStats // UpgradeLevelStats 상속은 유지
{
    // MilkerLevelStats처럼 고유 속성만 가집니다.
    [Tooltip("최소 신선도 값 (예: 20)")]
    [Range(0, 100)] public int minFreshness;
    [Tooltip("최대 신선도 값 (예: 30)")]
    [Range(0, 100)] public int maxFreshness;
}

[CreateAssetMenu(fileName = "New Pasture Upgrade Data", menuName = "Tycoon Game/Upgrade Data/Pasture")]
public class PastureUpgradeData : UpgradeData // UpgradeData 상속 유지
{
    public List<PastureLevelStats> upgradeLevels = new List<PastureLevelStats>();

    // 핵심: GetNextUpgradePrice() 함수를 삭제하여 CS0115 에러를 해결합니다.
    // 기존 스크립트들처럼 이 함수가 없어야 정상 작동합니다.

    /// <summary>
    /// 특정 레벨의 신선도 범위를 반환합니다.
    /// </summary>
    public (int min, int max) GetFreshnessRange(int currentLevel)
    {
        if (currentLevel < 0 || upgradeLevels.Count == 0)
        {
            Debug.LogError("Pasture data is invalid or level is out of range.");
            return (0, 0);
        }

        int index = Mathf.Clamp(currentLevel, 0, upgradeLevels.Count - 1);

        return (upgradeLevels[index].minFreshness, upgradeLevels[index].maxFreshness);
    }
}