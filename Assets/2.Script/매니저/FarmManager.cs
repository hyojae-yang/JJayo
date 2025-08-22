using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FarmManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static FarmManager Instance { get; private set; }

    [Header("���� ����Ʈ")]
    public Transform[] cowSpawnPoints;
    public Transform[] chickenSpawnPoints;

    [Header("���� ���� ���")]
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

    // ���� ���� �� ���� �ִ� ��� ������ �ڵ����� ã���ϴ�.
    void Start()
    {
        cowsInScene = FindObjectsByType<Animal>(FindObjectsSortMode.None).Where(a => a.animalData.animalName == "����").Select(a => a.gameObject).ToList();
        chickensInScene = FindObjectsByType<Animal>(FindObjectsSortMode.None).Where(a => a.animalData.animalName == "��").Select(a => a.gameObject).ToList();
    }

    /// <summary>
    /// ������ �߰��� �� �ִ� ������ �ִ��� Ȯ���մϴ�.
    /// </summary>
    /// <param name="animalName">Ȯ���� ������ �̸�</param>
    /// <returns>������ ������ true, ������ false</returns>
    public bool CanAddAnimal(string animalName)
    {
        if (animalName == "����")
        {
            return cowsInScene.Count < cowSpawnPoints.Length;
        }
        else if (animalName == "��")
        {
            return chickensInScene.Count < chickenSpawnPoints.Length;
        }
        return false;
    }

    /// <summary>
    /// ������ ��ġ�� ������ �߰��մϴ�.
    /// </summary>
    /// <param name="animalName">�߰��� ������ �̸�</param>
    /// <param name="animalPrefab">���� ������</param>
    public void AddAnimal(string animalName, GameObject animalPrefab)
    {
        if (animalName == "����")
        {
            Transform emptySpawnPoint = FindEmptySpawnPoint(cowSpawnPoints, cowsInScene);
            if (emptySpawnPoint != null)
            {
                GameObject newCowObject = Instantiate(animalPrefab, emptySpawnPoint.position, emptySpawnPoint.rotation);
                cowsInScene.Add(newCowObject);
            }
        }
        else if (animalName == "��")
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