using UnityEngine;
using System.Collections.Generic;

// 모든 업그레이드 데이터의 기본 클래스
public abstract class UpgradeData : ScriptableObject
{
    // 각 업그레이드 데이터가 구현해야 할 공통 메서드
    public abstract int GetUpgradePrice(int currentLevel);
    public abstract int GetMaxLevel();
}