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

    // ★★★ 삭제된 부분: milkAverageFreshness는 더 이상 저장하지 않습니다. ★★★
    // public float milkAverageFreshness;

    public int eggCount;

    // 장비 및 업그레이드 레벨
    public int pastureLevel;
    // ★★★ 삭제된 부분: hasGun은 더 이상 저장하지 않습니다. ownedEquipmentIds로 대체합니다. ★★★
    // public bool hasGun;

    public int gunLevel;
    public int basketLevel;
    public int milkerLevel;
    public int bulletCount;

    // ★★★ 삭제된 부분: gunDamage는 더 이상 저장하지 않습니다. gunLevel을 기반으로 계산됩니다. ★★★
    // public float gunDamage;

    public EquipmentType currentEquipment;

    // 건물 및 장비 보유 여부
    public List<string> ownedBuildingIds;
    public List<string> ownedEquipmentIds;

    // ★★★ 추가된 부분: 닭 마릿수 및 닭장에 쌓인 알의 개수를 저장합니다. ★★★
    public int chickenCount;
    public int currentChickenEggCount;

    // ★★★ 추가된 부분: 하루가 지났는지 여부를 저장합니다. ★★★
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