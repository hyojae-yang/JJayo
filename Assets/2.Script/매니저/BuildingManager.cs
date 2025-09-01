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

                if (_instance == null)
                {
                    Debug.LogError("씬에 BuildingManager 오브젝트가 없습니다. 각 씬에 하나씩 추가해야 합니다.");
                }
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
        // Start에서 씬에 있는 모든 Building 오브젝트를 찾아 리스트에 추가합니다.
        // 이렇게 하면 씬 로딩 후 모든 건물이 자동으로 등록됩니다.
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
}