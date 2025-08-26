using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static ShopManager Instance { get; private set; }

    [Header("���� ������ ������")]
    // ����Ƽ �����Ϳ��� ��� ������ �����͸� ������ ����Ʈ�Դϴ�.
    public List<PurchasableItemData> allShopItems;

    void Awake()
    {
        // �̱��� ���� ����
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
    /// ��� ���� ������ ����� ��ȯ�ϴ� �޼���
    /// </summary>
    public List<PurchasableItemData> GetShopItems()
    {
        return allShopItems;
    }
}