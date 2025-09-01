using UnityEngine;
using System.Collections.Generic;

public class BuildingHandler : MonoBehaviour
{
    private static BuildingHandler m_instance;
    public static BuildingHandler Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindFirstObjectByType<BuildingHandler>();
            }
            return m_instance;
        }
    }

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] public List<Transform> buildingSpawnPoints;

    public bool CanBuy()
    {
        return buildingSpawnPoints.Count > 0;
    }

    public void Purchase(BuildingData buildingData)
    {
        if (buildingSpawnPoints.Count == 0)
        {
            if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("더 이상 건물을 설치할 공간이 없습니다!");
            return;
        }

        if (buildingData == null || buildingData.buildingPrefab == null)
        {
            Debug.LogError("구매하려는 건물 데이터 또는 프리팹이 유효하지 않습니다.");
            return;
        }

        GameObject newBuilding = Instantiate(buildingData.buildingPrefab, buildingSpawnPoints[0].position, Quaternion.identity);
        buildingSpawnPoints.RemoveAt(0);

        GameData gameData = GameManager.Instance.CurrentGameData;
        if (gameData != null)
        {
            gameData.ownedBuildingIds.Add(buildingData.buildingId);
        }

        if (BuildingManager.Instance != null)
        {
            BuildingManager.Instance.AddBuilding(newBuilding);
        }
        else
        {
            Debug.LogError("BuildingManager 인스턴스를 찾을 수 없습니다.");
        }

        if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification(buildingData.buildingName + "을(를) 구매했습니다. 목장에 설치되었습니다!");
    }
}