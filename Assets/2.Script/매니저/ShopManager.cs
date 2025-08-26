using UnityEngine;
using System.Collections.Generic;

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
    /// 모든 상점 아이템 목록을 반환하는 메서드
    /// </summary>
    public List<PurchasableItemData> GetShopItems()
    {
        return allShopItems;
    }
}