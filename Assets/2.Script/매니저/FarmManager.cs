using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FarmManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static FarmManager Instance { get; private set; }

    [Header("스폰 포인트")]
    public Transform[] cowSpawnPoints;
    public Transform[] chickenSpawnPoints;

    [Header("현재 동물 목록")]
    public List<GameObject> cowsInScene = new List<GameObject>();
    public List<GameObject> chickensInScene = new List<GameObject>();

    private void Awake()
    {
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

    // 게임 시작 시 씬에 있는 모든 동물을 자동으로 찾습니다.
    void Start()
    {
        cowsInScene = FindObjectsByType<Animal>(FindObjectsSortMode.None).Where(a => a.animalData.animalName == "젖소").Select(a => a.gameObject).ToList();
        chickensInScene = FindObjectsByType<Animal>(FindObjectsSortMode.None).Where(a => a.animalData.animalName == "닭").Select(a => a.gameObject).ToList();
    }

    /// <summary>
    /// 동물을 추가할 수 있는 공간이 있는지 확인합니다.
    /// </summary>
    /// <param name="animalName">확인할 동물의 이름</param>
    /// <returns>공간이 있으면 true, 없으면 false</returns>
    public bool CanAddAnimal(string animalName)
    {
        if (animalName == "젖소")
        {
            return cowsInScene.Count < cowSpawnPoints.Length;
        }
        else if (animalName == "닭")
        {
            return chickensInScene.Count < chickenSpawnPoints.Length;
        }
        return false;
    }

    /// <summary>
    /// 지정된 위치에 동물을 추가합니다.
    /// </summary>
    /// <param name="animalName">추가할 동물의 이름</param>
    /// <param name="animalPrefab">동물 프리팹</param>
    public void AddAnimal(string animalName, GameObject animalPrefab)
    {
        if (animalName == "젖소")
        {
            Transform emptySpawnPoint = FindEmptySpawnPoint(cowSpawnPoints, cowsInScene);
            if (emptySpawnPoint != null)
            {
                GameObject newCowObject = Instantiate(animalPrefab, emptySpawnPoint.position, emptySpawnPoint.rotation);
                cowsInScene.Add(newCowObject);
            }
        }
        else if (animalName == "닭")
        {
            Transform emptySpawnPoint = FindEmptySpawnPoint(chickenSpawnPoints, chickensInScene);
            if (emptySpawnPoint != null)
            {
                GameObject newChickenObject = Instantiate(animalPrefab, emptySpawnPoint.position, emptySpawnPoint.rotation);
                chickensInScene.Add(newChickenObject);
            }
        }
    }

    private Transform FindEmptySpawnPoint(Transform[] spawnPoints, List<GameObject> animalsInScene)
    {
        List<Vector3> occupiedPositions = animalsInScene.Select(animal => animal.transform.position).ToList();
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (!occupiedPositions.Contains(spawnPoint.position))
            {
                return spawnPoint;
            }
        }
        return null;
    }
}