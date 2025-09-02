using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    private static BuildingManager _instance;
    public static BuildingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에 있는 유일한 BuildingManager 인스턴스를 찾습니다.
                _instance = FindFirstObjectByType<BuildingManager>();

            }
            return _instance;
        }
    }

    // 씬에 있는 모든 건물 오브젝트를 관리하는 리스트
    public List<GameObject> activeBuildings = new List<GameObject>();

    private void Awake()
    {
        // 씬 내 유일한 인스턴스로 설정하고, DontDestroyOnLoad는 제거합니다.
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
        // 게임 시작 시, 씬에 이미 배치되어 있는 건물들을 자동으로 찾아서 리스트에 추가합니다.
        // 이 로직은 첫 게임 시작 시 또는 씬에 미리 건물이 배치되어 있을 경우 유용합니다.
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        foreach (GameObject building in buildings)
        {
            AddBuilding(building);
        }
    }

    // 건물을 리스트에 추가하는 메서드
    public void AddBuilding(GameObject building)
    {
        if (!activeBuildings.Contains(building))
        {
            activeBuildings.Add(building);
        }
    }

    // 건물을 리스트에서 제거하는 메서드
    public void RemoveBuilding(GameObject building)
    {
        if (activeBuildings.Contains(building))
        {
            activeBuildings.Remove(building);
        }
    }

    /// <summary>
    /// 현재 활성화된 모든 건물의 ID와 위치를 리스트로 저장합니다.
    /// </summary>
    /// <returns>저장할 건물 데이터 리스트</returns>
    public List<SavedBuildingData> SaveBuildingData()
    {
        List<SavedBuildingData> savedDataList = new List<SavedBuildingData>();
        foreach (GameObject building in activeBuildings)
        {
            // BuildingComponent에 접근하여 ID와 위치를 가져옵니다.
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

    /// <summary>
    /// 저장된 데이터를 바탕으로 건물을 씬에 다시 생성합니다.
    /// </summary>
    /// <param name="savedDataList">불러올 건물 데이터 리스트</param>
    /// <param name="buildingPrefabs">모든 건물 프리팹 리스트</param>
    public void LoadBuildingData(List<SavedBuildingData> savedDataList, List<GameObject> buildingPrefabs)
    {
        if (savedDataList == null)
        {
            Debug.Log("불러올 건물 데이터가 없습니다.");
            return;
        }

        // ★★★ 기존에 씬에 남아있을 수 있는 건물을 모두 제거
        foreach (GameObject building in activeBuildings)
        {
            Destroy(building);
        }
        activeBuildings.Clear();

        foreach (SavedBuildingData data in savedDataList)
        {
            // ID에 해당하는 건물 프리팹을 찾습니다.
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
}