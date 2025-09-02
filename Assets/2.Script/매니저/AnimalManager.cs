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
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    public List<Animal> activeAnimals = new List<Animal>();

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
            // ★★★ 수정된 부분: animalData를 통해 ID에 접근합니다.
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

        foreach (SavedCowData data in savedDataList)
        {
            // ★★★ 수정된 부분: ID에 해당하는 젖소 프리팹을 찾습니다.
            // 프리팹 자체에 연결된 Animal 컴포넌트의 animalData에 접근하여 ID를 비교합니다.
            GameObject prefabToInstantiate = cowPrefabs.Find(p => p.GetComponent<Animal>().animalData.animalId == data.cowId);

            if (prefabToInstantiate != null)
            {
                GameObject newCow = Instantiate(prefabToInstantiate, new Vector2(data.posX, data.posY), Quaternion.identity);
                Animal newAnimal = newCow.GetComponent<Animal>();
                if (newAnimal != null)
                {
                    AddAnimal(newAnimal);
                }
            }
            else
            {
                Debug.LogWarning($"젖소 ID '{data.cowId}'에 해당하는 프리팹을 찾을 수 없습니다!");
            }
        }
    }
}