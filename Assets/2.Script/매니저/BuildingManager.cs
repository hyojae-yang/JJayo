// BuildingManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq; // ★★★ Linq 네임스페이스 추가 ★★★

public class BuildingManager : MonoBehaviour
{
    private static BuildingManager _instance;
    public static BuildingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<BuildingManager>();
            }
            return _instance;
        }
    }

    public List<GameObject> activeBuildings = new List<GameObject>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        foreach (GameObject building in buildings)
        {
            AddBuilding(building);
        }
    }

    public void AddBuilding(GameObject building)
    {
        if (!activeBuildings.Contains(building))
        {
            activeBuildings.Add(building);
        }
    }

    public void RemoveBuilding(GameObject building)
    {
        if (activeBuildings.Contains(building))
        {
            activeBuildings.Remove(building);
        }
    }

    public List<SavedBuildingData> SaveBuildingData()
    {
        List<SavedBuildingData> savedDataList = new List<SavedBuildingData>();
        foreach (GameObject building in activeBuildings)
        {
            BuildingComponent buildingComp = building.GetComponent<BuildingComponent>();
            if (buildingComp != null && buildingComp.buildingData != null)
            {
                SavedBuildingData data = new SavedBuildingData();
                data.buildingId = buildingComp.buildingData.buildingId;
                data.posX = building.transform.position.x;
                data.posY = building.transform.position.y;
                savedDataList.Add(data);
            }
        }
        return savedDataList;
    }

    public void LoadBuildingData(List<SavedBuildingData> savedDataList, List<GameObject> buildingPrefabs)
    {
        if (savedDataList == null)
        {
            Debug.Log("불러올 건물 데이터가 없습니다.");
            return;
        }

        foreach (GameObject building in activeBuildings)
        {
            Destroy(building);
        }
        activeBuildings.Clear();

        foreach (SavedBuildingData data in savedDataList)
        {
            GameObject prefabToInstantiate = buildingPrefabs.Find(p => p.GetComponent<BuildingComponent>().buildingData.buildingId == data.buildingId);

            if (prefabToInstantiate != null)
            {
                GameObject newBuilding = Instantiate(prefabToInstantiate, new Vector2(data.posX, data.posY), Quaternion.identity);
                AddBuilding(newBuilding);
            }
            else
            {
                Debug.LogWarning($"건물 ID '{data.buildingId}'에 해당하는 프리팹을 찾을 수 없습니다!");
            }
        }
    }

    /// <summary>
    /// 특정 ID를 가진 건물이 씬에 존재하는지 확인합니다.
    /// </summary>
    public bool IsBuildingOwned(string buildingId)
    {
        // Linq의 Any()를 사용하여 리스트를 순회하며 조건에 맞는 요소를 찾습니다.
        return activeBuildings.Any(b => b.GetComponent<BuildingComponent>()?.buildingData?.buildingId == buildingId);
    }
}