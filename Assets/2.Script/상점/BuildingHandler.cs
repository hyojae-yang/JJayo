using UnityEngine;
using System.Collections.Generic;

public class BuildingHandler : MonoBehaviour
{
    // BuildingHandler가 직접 스폰 포인트를 관리합니다.
    [SerializeField] public List<Transform> buildingSpawnPoints;

    // 이 클래스는 이제 특정 건물 변수를 가질 필요가 없습니다.
    // private ChickenCoop chickenCoop;
    // ...

    private void Awake()
    {
        // 씬에서 필요한 컴포넌트들을 찾아와 연결합니다.
        // 건물별로 FindObjectOfType을 호출할 필요가 없어집니다.
    }

    /// <summary>
    /// 건물 구매 가능 여부를 확인합니다.
    /// 이 핸들러는 오직 설치할 공간이 있는지 여부만 확인합니다.
    /// </summary>
    public bool CanBuy()
    {
        return buildingSpawnPoints.Count > 0;
    }

    /// <summary>
    /// 건물을 구매하고 설치합니다.
    /// </summary>
    /// <param name="buildingData">구매하려는 건물의 데이터 에셋입니다.</param>
    public void Purchase(BuildingData buildingData)
    {
        // buildingSpawnPoints 리스트의 유효성 검사
        if (buildingSpawnPoints.Count == 0)
        {
            NotificationManager.Instance.ShowNotification("더 이상 건물을 설치할 공간이 없습니다!");
            return;
        }

        // buildingData가 유효한지 확인
        if (buildingData == null || buildingData.buildingPrefab == null)
        {
            Debug.LogError("구매하려는 건물 데이터 또는 프리팹이 유효하지 않습니다.");
            return;
        }

        // 이제 안전하게 인스턴스화 로직을 실행합니다.
        Instantiate(buildingData.buildingPrefab, buildingSpawnPoints[0].position, Quaternion.identity);
        buildingSpawnPoints.RemoveAt(0);

        // 건물 구매 후 GameData에 보유 정보를 추가합니다.
        GameData gameData = GameManager.Instance.gameData;
        if (gameData != null)
        {
            // GameData의 List에 건물의 ID를 추가합니다.
            gameData.ownedBuildingIds.Add(buildingData.buildingId);
        }

        NotificationManager.Instance.ShowNotification(buildingData.buildingName + "을(를) 구매했습니다. 목장에 설치되었습니다!");
    }
}