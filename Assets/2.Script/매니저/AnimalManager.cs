using UnityEngine;
using System.Collections.Generic;

public class AnimalManager : MonoBehaviour
{
    private static AnimalManager _instance;
    public static AnimalManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<AnimalManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<AnimalManager>();
                    singletonObject.name = typeof(AnimalManager).ToString() + " (Singleton)";
                }
            }
            return _instance;
        }
    }

    public List<Animal> activeAnimals = new List<Animal>();
    // 추가된 부분: 현재 젖소들이 차지하고 있는 위치 리스트
    public List<Vector2> occupiedCowPositions = new List<Vector2>();

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

    public void AddAnimal(Animal animal)
    {
        if (!activeAnimals.Contains(animal))
        {
            activeAnimals.Add(animal);
        }
    }

    public void RemoveAnimal(Animal animal)
    {
        if (activeAnimals.Contains(animal))
        {
            activeAnimals.Remove(animal);
            // 추가된 부분: 젖소를 제거할 때 해당 위치를 점유 리스트에서 제거
            occupiedCowPositions.Remove(animal.transform.position);
        }
    }

    /// <summary>
    /// 현재 활성화된 모든 젖소의 ID와 위치를 리스트로 저장합니다.
    /// </summary>
    /// <returns>저장할 젖소 데이터 리스트</returns>
    public List<SavedCowData> SaveCowData()
    {
        List<SavedCowData> savedDataList = new List<SavedCowData>();
        foreach (Animal animal in activeAnimals)
        {
            if (animal.animalData != null)
            {
                SavedCowData data = new SavedCowData();
                data.cowId = animal.animalData.animalId;
                data.posX = animal.transform.position.x;
                data.posY = animal.transform.position.y;
                savedDataList.Add(data);
            }
        }
        return savedDataList;
    }

    /// <summary>
    /// 저장된 데이터를 바탕으로 젖소를 씬에 다시 생성합니다.
    /// </summary>
    /// <param name="savedDataList">불러올 젖소 데이터 리스트</param>
    /// <param name="cowPrefabs">모든 젖소 프리팹 리스트</param>
    public void LoadCowData(List<SavedCowData> savedDataList, List<GameObject> cowPrefabs)
    {
        if (savedDataList == null)
        {
            Debug.Log("불러올 젖소 데이터가 없습니다.");
            return;
        }

        // 기존에 씬에 남아있을 수 있는 젖소를 모두 제거
        foreach (Animal animal in activeAnimals)
        {
            Destroy(animal.gameObject);
        }
        activeAnimals.Clear();
        // 추가된 부분: 새로운 젖소를 불러오기 전에 위치 리스트를 비웁니다.
        occupiedCowPositions.Clear();

        foreach (SavedCowData data in savedDataList)
        {
            GameObject prefabToInstantiate = cowPrefabs.Find(p => p.GetComponent<Animal>().animalData.animalId == data.cowId);

            if (prefabToInstantiate != null)
            {
                Vector2 loadedPosition = new Vector2(data.posX, data.posY);
                GameObject newCow = Instantiate(prefabToInstantiate, loadedPosition, Quaternion.identity);
                Animal newAnimal = newCow.GetComponent<Animal>();
                Production productionComponent = newCow.GetComponent<Production>();

                if (newAnimal != null)
                {
                    newAnimal.Initialize(prefabToInstantiate.GetComponent<Animal>().animalData);
                }

                if (productionComponent != null && GameManager.Instance != null && GameManager.Instance.pastureUpgradeData != null)
                {
                    productionComponent.Initialize(GameManager.Instance.CurrentPastureLevel, GameManager.Instance.pastureUpgradeData);
                }
                else
                {
                    Debug.LogError("생산 및 신선도 초기화에 필요한 데이터가 유효하지 않습니다. GameManager 또는 Production 컴포넌트를 확인하세요.");
                }

                if (newAnimal != null)
                {
                    AddAnimal(newAnimal);
                    // 추가된 부분: 불러온 젖소의 위치를 occupiedCowPositions에 기록합니다.
                    occupiedCowPositions.Add(loadedPosition);
                }
            }
            else
            {
                Debug.LogWarning($"젖소 ID '{data.cowId}'에 해당하는 프리팹을 찾을 수 없습니다!");
            }
        }
    }

    // 추가된 부분: 젖소를 배치할 빈 공간을 찾아주는 메서드
    public Vector2 GetAvailableCowPosition(List<Transform> spawnPoints)
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (!occupiedCowPositions.Contains(spawnPoint.position))
            {
                return spawnPoint.position;
            }
        }
        return Vector2.zero; // 빈 공간이 없을 경우 Vector2.zero 반환
    }
}