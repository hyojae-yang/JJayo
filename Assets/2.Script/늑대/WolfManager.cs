using UnityEngine;
using System.Collections.Generic;

public class WolfManager : MonoBehaviour
{
    public static WolfManager Instance { get; private set; }

    [Header("늑대 풀링 시스템")]
    public ObjectPool wolfObjectPool;

    [Header("늑대 스탯 설정")]
    public float baseHealth = 30f;
    public float baseDamage = 20f;
    public float difficultyScale = 1.2f;

    [Header("늑대 이벤트 설정")]
    private List<int> eventDates = new List<int>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // GameManager 대신 TimeManager의 이벤트를 구독
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.OnMonthChanged += GenerateRandomEventDates;
                TimeManager.Instance.OnDayChanged += CheckForWolfEvent;
            }
            GenerateRandomEventDates();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GenerateRandomEventDates()
    {
        eventDates.Clear();

        int currentMonth = GameManager.Instance.CurrentGameData.month;
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

        Debug.Log($"이번 달 늑대 이벤트 날짜: {string.Join(", ", eventDates)}");
    }

    public void CheckForWolfEvent(int currentDay)
    {
        if (eventDates.Contains(currentDay))
        {
            // 젖소가 한 마리라도 있는지 확인
            if (AnimalManager.Instance.activeAnimals.Count == 0)
            {
                Debug.Log("젖소가 없어 늑대가 나타나지 않습니다.");
                return;
            }

            int currentYear = GameManager.Instance.CurrentGameData.year;
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

            // ★★★ 추가된 코드: 늑대 등장 효과음 재생 ★★★
            SoundManager.Instance.PlaySFX(SFXType.Wolf_Appear);

            NotificationManager.Instance.ShowNotification("늑대가 나타났습니다! 젖소를 지키세요!");
        }
    }

    public void SpawnWolf()
    {
        if (wolfObjectPool == null)
        {
            Debug.LogError("Wolf Object Pool이 할당되지 않았습니다!");
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
                int currentYear = GameManager.Instance.CurrentGameData.year;
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

        Debug.Log("하루가 지나 모든 늑대가 풀로 돌아갔습니다.");
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

    // Wolf 스크립트가 젖소 목록에 접근할 수 있도록 하는 중개자 메서드
    public List<Animal> GetActiveCows()
    {
        if (AnimalManager.Instance != null)
        {
            return AnimalManager.Instance.activeAnimals;
        }
        return new List<Animal>();
    }
}