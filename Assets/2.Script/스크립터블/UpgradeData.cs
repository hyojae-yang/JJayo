using UnityEngine;

// 모든 업그레이드 데이터의 기본 클래스
// 이 스크립트 자체로는 에셋을 만들 수 없습니다.
public abstract class UpgradeData : ScriptableObject
{
    public int level;
    public int upgradePrice;
}