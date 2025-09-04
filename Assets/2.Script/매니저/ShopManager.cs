using UnityEngine;
using System.Collections.Generic;
using System.Linq; // ★★★ Linq 사용을 위해 추가되었습니다. ★★★

public class ShopManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static ShopManager Instance { get; private set; }

    [Header("상점 아이템 데이터")]
    // 유니티 에디터에서 모든 아이템 데이터를 연결할 리스트입니다.
    public List<PurchasableItemData> allShopItems;

    void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 플레이어의 명성도에 따라 해금된 상점 아이템 목록을 반환하는 메서드
    /// </summary>
    public List<PurchasableItemData> GetShopItems()
    {
        // ★★★ 수정된 로직: 명성도에 따라 젖소 아이템을 필터링합니다. ★★★
        var unlockedItems = new List<PurchasableItemData>();

        // 현재 플레이어의 명성도
        int currentReputation = GameManager.Instance.CurrentGameData.reputation;

        foreach (var item in allShopItems)
        {
            // 아이템 타입이 젖소인 경우에만 해금 조건을 확인합니다.
            if (item.itemType == ItemType.Animal)
            {
                // 젖소의 해금 명성도가 현재 명성도보다 낮거나 같을 때만 목록에 추가합니다.
                if (item.animalData != null && item.animalData.unlockReputation <= currentReputation)
                {
                    unlockedItems.Add(item);
                }
            }
            // 동물 아이템이 아니면 (건물, 장비 등) 무조건 목록에 추가합니다.
            else
            {
                unlockedItems.Add(item);
            }
        }
        return unlockedItems;
    }
}