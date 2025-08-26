using UnityEngine;
using System.Collections.Generic;

public class WolfManager : MonoBehaviour
{
    public static WolfManager Instance { get; private set; }

    [Header("���� Ǯ�� �ý���")]
    public ObjectPool wolfObjectPool;

    [Header("���� ���� ����")]
    public float baseHealth = 30f;
    public float baseDamage = 20f;
    public float difficultyScale = 1.2f;

    [Header("���� �̺�Ʈ ����")]
    private List<int> eventDates = new List<int>();

    void Awake()
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

    void Start()
    {
        // GameManager ��� TimeManager�� �̺�Ʈ�� ����
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnMonthChanged += GenerateRandomEventDates;
            TimeManager.Instance.OnDayChanged += CheckForWolfEvent;
        }
        GenerateRandomEventDates();
    }
    public void GenerateRandomEventDates()
    {
        eventDates.Clear();

        int currentMonth = GameManager.Instance.gameData.month;
        int maxEventCount;

        if (currentMonth <= 3)
        {
            maxEventCount = 1;
        }
        else if (currentMonth <= 6)
        {
            maxEventCount = 3;
        }
        else if (currentMonth <= 9)
        {
            maxEventCount = 4;
        }
        else
        {
            maxEventCount = 5;
        }

        int eventCount = UnityEngine.Random.Range(0, maxEventCount + 1);

        HashSet<int> randomDays = new HashSet<int>();
        while (randomDays.Count < eventCount)
        {
            int day = UnityEngine.Random.Range(1, 31);
            randomDays.Add(day);
        }

        foreach (int day in randomDays)
        {
            eventDates.Add(day);
        }

        Debug.Log($"�̹� �� ���� �̺�Ʈ ��¥: {string.Join(", ", eventDates)}");
    }

    public void CheckForWolfEvent(int currentDay)
    {
        if (eventDates.Contains(currentDay))
        {
            int currentYear = GameManager.Instance.gameData.year;
            int minWolves, maxWolves;

            if (currentYear == 1)
            {
                minWolves = 1;
                maxWolves = 1;
            }
            else if (currentYear == 2)
            {
                minWolves = 1;
                maxWolves = 2;
            }
            else if (currentYear == 3)
            {
                minWolves = 1;
                maxWolves = 3;
            }
            else if (currentYear == 4)
            {
                minWolves = 1;
                maxWolves = 4;
            }
            else if (currentYear == 5)
            {
                minWolves = 1;
                maxWolves = 5;
            }
            else if (currentYear == 6)
            {
                minWolves = 2;
                maxWolves = 5;
            }
            else if (currentYear == 7)
            {
                minWolves = 3;
                maxWolves = 5;
            }
            else if (currentYear == 8)
            {
                minWolves = 4;
                maxWolves = 5;
            }
            else
            {
                minWolves = 5;
                maxWolves = 5;
            }

            int wolvesToSpawn = UnityEngine.Random.Range(minWolves, maxWolves + 1);
            for (int i = 0; i < wolvesToSpawn; i++)
            {
                SpawnWolf();
            }
            NotificationManager.Instance.ShowNotification("���밡 ��Ÿ�����ϴ�! ���Ҹ� ��Ű����!");
        }
    }

    public void SpawnWolf()
    {
        if (wolfObjectPool == null)
        {
            Debug.LogError("Wolf Object Pool�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        GameObject wolfObj = wolfObjectPool.GetFromPool();
        if (wolfObj != null)
        {
            Vector3 randomSpawnPosition = GetRandomSpawnPosition();
            wolfObj.transform.position = randomSpawnPosition;
            wolfObj.transform.rotation = Quaternion.identity;

            Wolf wolfScript = wolfObj.GetComponent<Wolf>();
            if (wolfScript != null)
            {
                int currentYear = GameManager.Instance.gameData.year;
                float scaledHealth = baseHealth * Mathf.Pow(difficultyScale, currentYear - 1);
                float scaledDamage = baseDamage * Mathf.Pow(difficultyScale, currentYear - 1);

                wolfScript.Initialize(this, scaledHealth, scaledDamage);
            }
        }
    }

    public void ReturnWolfToPool(GameObject wolfObj)
    {
        wolfObj.GetComponent<Wolf>().isReturning = false;
        wolfObj.SetActive(false);
    }

    public void ReturnAllWolvesToPool()
    {
        GameObject[] activeWolves = GameObject.FindGameObjectsWithTag("Wolf");

        foreach (GameObject wolfObj in activeWolves)
        {
            ReturnWolfToPool(wolfObj);
        }

        Debug.Log("�Ϸ簡 ���� ��� ���밡 Ǯ�� ���ư����ϴ�.");
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 spawnPosition = Vector3.zero;
        int side = UnityEngine.Random.Range(0, 4);

        float cameraHeight = Camera.main.orthographicSize;
        float cameraWidth = cameraHeight * Camera.main.aspect;

        switch (side)
        {
            case 0:
                spawnPosition = new Vector3(UnityEngine.Random.Range(-cameraWidth, cameraWidth), cameraHeight + 1f, 0);
                break;
            case 1:
                spawnPosition = new Vector3(UnityEngine.Random.Range(-cameraWidth, cameraWidth), -cameraHeight - 1f, 0);
                break;
            case 2:
                spawnPosition = new Vector3(-cameraWidth - 1f, UnityEngine.Random.Range(-cameraHeight, cameraHeight), 0);
                break;
            case 3:
                spawnPosition = new Vector3(cameraWidth + 1f, UnityEngine.Random.Range(-cameraHeight, cameraHeight), 0);
                break;
        }

        return spawnPosition + Camera.main.transform.position;
    }
}