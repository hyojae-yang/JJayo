using System;
using System.Collections.Generic;

// 각 젖소의 위치 정보를 저장하는 클래스
[Serializable]
public class SavedAnimalData
{
    public float posX, posY;
    // 추가적인 데이터 (예: 체력, 성장 단계 등)도 여기에 포함될 수 있습니다.
}

// 각 건물의 위치 정보를 저장하는 클래스
[Serializable]
public class SavedBuildingData
{
    public string buildingId;
    public float posX, posY;
    // 추가적인 데이터 (예: 건물 체력, 소유 여부 등)도 여기에 포함될 수 있습니다.
}

[Serializable]
public class GameData
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
    public float milkAverageFreshness;
    public int eggCount;

    // 장비 및 업그레이드 레벨
    public int pastureLevel;
    public bool hasGun;
    public int gunLevel;
    public int basketLevel;
    public int milkerLevel;
    public int bulletCount;

    public float gunDamage;

    public EquipmentType currentEquipment;

    // 건물 및 장비 보유 여부
    public List<string> ownedBuildingIds;
    public List<string> ownedEquipmentIds;

    // ★★★ 추가된 부분: 젖소와 건물의 위치 데이터를 담을 리스트 ★★★
    public List<SavedAnimalData> savedAnimals;
    public List<SavedBuildingData> savedBuildings;

    // NPC(상인) 관련 데이터
    public int traderRequiredMilkAmount;
    public int traderRequiredFreshness;
    public int traderOfferedPrice;
    public float traderCurrentEggPrice;

    public GameData()
    {
        this.money = 50000;
        this.reputation = 50;

        this.pastureLevel = 0;

        this.day = 1;
        this.month = 1;
        this.year = 1;

        this.dailyMilkProduced = 0;
        this.dailyEggsProduced = 0;
        this.milkCount = 0;
        this.milkAverageFreshness = 0f;
        this.eggCount = 0;

        this.hasGun = false;
        this.gunLevel = 0;
        this.basketLevel = 1;
        this.milkerLevel = 1;
        this.bulletCount = 0;

        this.gunDamage = 10f;

        this.ownedBuildingIds = new List<string>();
        this.ownedEquipmentIds = new List<string>();

        // ★★★ 추가된 부분: 새로운 리스트 초기화 ★★★
        this.savedAnimals = new List<SavedAnimalData>();
        this.savedBuildings = new List<SavedBuildingData>();

        this.traderRequiredMilkAmount = 0;
        this.traderRequiredFreshness = 0;
        this.traderOfferedPrice = 0;
        this.traderCurrentEggPrice = 0;
    }

    /// <summary>
    /// 특정 건물을 보유하고 있는지 확인합니다.
    /// </summary>
    public bool IsBuildingOwned(string buildingId)
    {
        return ownedBuildingIds.Contains(buildingId);
    }
}