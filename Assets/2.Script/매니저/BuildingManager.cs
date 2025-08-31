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
                _instance = FindObjectOfType<BuildingManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<BuildingManager>();
                    singletonObject.name = typeof(BuildingManager).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    // 씬에 있는 모든 건물 오브젝트를 관리하는 리스트
    public List<GameObject> activeBuildings = new List<GameObject>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
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