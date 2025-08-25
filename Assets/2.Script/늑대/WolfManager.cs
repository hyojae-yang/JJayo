using UnityEngine;
using System.Collections.Generic;

public class WolfManager : MonoBehaviour
{
    [Header("늑대 풀링 시스템")]
    public ObjectPool wolfObjectPool;

    [Header("늑대 스탯 설정")]
    public float baseHealth = 100f;
    public float baseDamage = 20f;
    public float difficultyScale = 1.2f; // 매달 늑대의 체력/공격력 배율

    [Header("늑대 이벤트 설정")]
    // private으로 변경하여 인스펙터에 노출되지 않음
    private List<int> eventDates = new List<int>();

    void Start()
    {
        if (GameManager.Instance != null)
        {
            // 월이 변경될 때마다 이벤트 날짜를 다시 계산
            GameManager.Instance.OnMonthChanged += GenerateRandomEventDates;
            // 일자가 변경될 때마다 늑대 이벤트를 확인
            GameManager.Instance.OnDayChanged += CheckForWolfEvent;
        }
    }

    // 매달 랜덤한 이벤트 날짜를 생성합니다.
    public void GenerateRandomEventDates()
    {
        eventDates.Clear();

        int currentMonth = GameManager.Instance.gameDate.Month;
        int maxEventCount;

        // ★★★ 사용자 구상 반영: 3개월 단위로 이벤트 횟수 증가 ★★★
        if (currentMonth <= 3)
        {
            maxEventCount = 1; // 0~1회
        }
        else if (currentMonth <= 6)
        {
            maxEventCount = 3; // 1~3회
        }
        else if (currentMonth <= 9)
        {
            maxEventCount = 4; // 2~4회
        }
        else
        {
            maxEventCount = 5; // 3~5회
        }

        int eventCount = UnityEngine.Random.Range(0, maxEventCount + 1);

        // 날짜를 담을 HashSet을 사용하여 중복 방지
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
            // ★★★ 사용자 구상 반영: 연도에 따라 소환 마릿수 증가 ★★★
            int currentYear = GameManager.Instance.gameDate.Year;
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
                maxWolves = 5; // 9년차부터 5마리 고정
            }

            int wolvesToSpawn = UnityEngine.Random.Range(minWolves, maxWolves + 1);
            for (int i = 0; i < wolvesToSpawn; i++)
            {
                SpawnWolf();
            }
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
                // ★★★ 현재 연도에 따라 스탯을 계산하여 전달합니다. ★★★
                int currentYear = GameManager.Instance.gameDate.Year;
                float scaledHealth = baseHealth * Mathf.Pow(difficultyScale, currentYear - 1); // 1년차는 배율 0
                float scaledDamage = baseDamage * Mathf.Pow(difficultyScale, currentYear - 1);

                wolfScript.Initialize(this, scaledHealth, scaledDamage);
            }
        }
    }

    public void ReturnWolfToPool(GameObject wolfObj)
    {
        // ★★★ 늑대 풀로 반환하기 전 초기화 ★★★
        wolfObj.GetComponent<Wolf>().isReturning = false; // 반환 상태 초기화
        wolfObj.SetActive(false);
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