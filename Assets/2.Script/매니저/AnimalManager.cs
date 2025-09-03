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

    // 수정된 부분: 이제 스폰포인트 리스트를 AnimalManager가 직접 관리합니다.
    [SerializeField] public List<Transform> cowSpawnPoints;

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

    public void LoadCowData(List<SavedCowData> savedDataList, List<GameObject> cowPrefabs)
    {
        if (savedDataList == null)
        {
            Debug.Log("불러올 젖소 데이터가 없습니다.");
            return;
        }

        foreach (Animal animal in activeAnimals)
        {
            Destroy(animal.gameObject);
        }
        activeAnimals.Clear();
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
                    occupiedCowPositions.Add(loadedPosition);
                }
            }
            else
            {
                Debug.LogWarning($"젖소 ID '{data.cowId}'에 해당하는 프리팹을 찾을 수 없습니다!");
            }
        }
    }

    // 수정된 부분: 인수를 받지 않고 내부 스폰포인트 리스트를 사용합니다.
    public Vector2 GetAvailableCowPosition()
    {
        foreach (Transform spawnPoint in cowSpawnPoints)
        {
            if (!occupiedCowPositions.Contains(spawnPoint.position))
            {
                return spawnPoint.position;
            }
        }
        return Vector2.zero;
    }
}