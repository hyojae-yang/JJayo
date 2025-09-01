using System;
using System.Collections.Generic;
using UnityEngine; // ScriptableObject 사용을 위해 UnityEngine 네임스페이스 추가

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

    // IsBuildingOwned() 메서드는 그대로 유지됩니다.
    public bool IsBuildingOwned(string buildingId)
    {
        return ownedBuildingIds.Contains(buildingId);
    }

    // ★★★ GameData() 생성자는 ScriptableObject에서 직접 사용되지 않으므로 제거합니다. ★★★
    // 초기값 설정은 다음 단계에서 GameManager가 담당하게 됩니다.
}

// 각 젖소의 위치 정보를 저장하는 클래스 (변경 없음)
[Serializable]
public class SavedAnimalData
{
    public float posX, posY;
}

// 각 건물의 위치 정보를 저장하는 클래스 (변경 없음)
[Serializable]
public class SavedBuildingData
{
    public string buildingId;
    public float posX, posY;
}

// 기존에 GameData 스크립트가 사용하던 열거형이 필요할 경우를 대비하여 추가
// 이 부분은 기존 스크립트의 의존성에 따라 달라질 수 있습니다.
public enum EquipmentType
{
    None,
    Basket,
    Milker,
    Gun
}