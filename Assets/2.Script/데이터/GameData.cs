using System;
using System.Collections.Generic;
using UnityEngine;

// 유니티 에디터 메뉴에 'Create/Game Data' 항목을 추가합니다.
[CreateAssetMenu(fileName = "New Game Data", menuName = "Game Data")]
public class GameData : ScriptableObject
{
    // 게임의 재화 및 명성
    public int money;
    public int reputation;

    // 시간 및 날짜
    public int day;
    public int month;
    public int year;

    // 생산량
    public int dailyMilkProduced;
    public int dailyEggsProduced;
    public int milkCount;

    public int eggCount;

    // 장비 및 업그레이드 레벨
    public int pastureLevel;
    public int gunLevel;
    public int basketLevel;
    public int milkerLevel;
    public int warehouseLevel;
    public int bulletCount;

    public EquipmentType currentEquipment;

    // 건물 및 장비 보유 여부
    public List<string> ownedBuildingIds;
    public List<string> ownedEquipmentIds;

    // 닭 마릿수 및 닭장에 쌓인 알의 개수를 저장합니다.
    public int chickenCount;
    // 기존의 달걀 개수 변수 (호환성을 위해 유지)
    public int currentChickenEggCount;
    // ★★★ 추가: 닭장에 있는 달걀들의 신선도 리스트 ★★★
    public List<float> savedChickenEggs = new List<float>();

    // 하루가 지났는지 여부를 저장합니다.
    public bool dailyProductionReset = false;

    // 젖소와 건물의 위치 및 ID 데이터를 담을 리스트
    public List<SavedCowData> savedCows;
    public List<SavedBuildingData> savedBuildings;

    // NPC(상인) 관련 데이터
    public int traderRequiredMilkAmount;
    public int traderRequiredFreshness;
    public int traderOfferedPrice;
    public float traderCurrentEggPrice;

    // IsBuildingOwned() 메서드는 그대로 유지됩니다.
    public bool IsBuildingOwned(string buildingId)
    {
        return ownedBuildingIds.Contains(buildingId);
    }
    // ★★★ 게임 통계 변수 추가 ★★★
    public int totalMoneyEarned;
    public int totalMilkSold;
    public int totalEggsSold;
    public int totalCowsPurchased;
    public int totalCowsSold;
    public int totalChickensPurchased;
    public int totalChickensSold;
    public int totalWolvesKilled;
    public int totalCowsEaten;
    public float totalPlayTime;
    public int totalCowsKilledByPlayer;
    // ★★★ 통계 변수 추가 끝 ★★★
}

[Serializable]
public class SavedCowData
{
    public string cowId;
    public float posX, posY;
}

[Serializable]
public class SavedBuildingData
{
    public string buildingId;
    public float posX, posY;
}

public enum EquipmentType
{
    None,
    Basket,
    Milker,
    Gun
}