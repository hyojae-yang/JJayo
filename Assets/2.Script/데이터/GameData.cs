using System;
using System.Collections.Generic;

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

    public EquipmentType currentEquipment;

    // ★★★ 건물 보유 여부 (List로 변경)
    public List<string> ownedBuildingIds;

    // ★★★ 장비 보유 여부 (List 추가) ★★★
    public List<string> ownedEquipmentIds;

    // NPC(상인) 관련 데이터
    public int traderRequiredMilkAmount;
    public int traderRequiredFreshness;
    public int traderOfferedPrice;
    public float traderCurrentEggPrice;

    public GameData()
    {
        // 모든 변수의 초기값을 명확히 설정합니다.
        this.money = 50000;
        this.reputation = 0;

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

        // ★★★ 건물 보유 리스트 초기화
        this.ownedBuildingIds = new List<string>();

        // ★★★ 장비 보유 리스트 초기화 ★★★
        this.ownedEquipmentIds = new List<string>();

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