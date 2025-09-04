using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Warehouse Upgrade Data", menuName = "Upgrade Data/Warehouse")]
public class WarehouseUpgradeData : UpgradeData
{
    [System.Serializable]
    public class LevelData
    {
        public int price;
        [Tooltip("신선도 감소 배율 (0.1 = 10% 감소, 0.5 = 50% 감소)")]
        [Range(0, 1)] public float freshnessDecayMultiplier;
    }

    public List<LevelData> levels;

    public override int GetUpgradePrice(int currentLevel)
    {
        if (currentLevel >= levels.Count) return -1;
        return levels[currentLevel].price;
    }

    public override int GetMaxLevel()
    {
        return levels.Count;
    }

    public float GetFreshnessDecayMultiplier(int currentLevel)
    {
        if (currentLevel <= 0 || currentLevel > levels.Count)
        {
            // 레벨이 0이거나 데이터 범위를 벗어나면 기본값인 1.0f(감소율 변화 없음)를 반환
            return 1.0f;
        }
        return levels[currentLevel - 1].freshnessDecayMultiplier;
    }
}