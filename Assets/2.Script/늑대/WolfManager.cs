using UnityEngine;
using System.Collections.Generic;

public class WolfManager : MonoBehaviour
{
    [Header("���� Ǯ�� �ý���")]
    public ObjectPool wolfObjectPool;

    [Header("���� ���� ����")]
    public float baseHealth = 100f;
    public float baseDamage = 20f;
    public float difficultyScale = 1.2f; // �Ŵ� ������ ü��/���ݷ� ����

    [Header("���� �̺�Ʈ ����")]
    // private���� �����Ͽ� �ν����Ϳ� ������� ����
    private List<int> eventDates = new List<int>();

    void Start()
    {
        if (GameManager.Instance != null)
        {
            // ���� ����� ������ �̺�Ʈ ��¥�� �ٽ� ���
            GameManager.Instance.OnMonthChanged += GenerateRandomEventDates;
            // ���ڰ� ����� ������ ���� �̺�Ʈ�� Ȯ��
            GameManager.Instance.OnDayChanged += CheckForWolfEvent;
        }
    }

    // �Ŵ� ������ �̺�Ʈ ��¥�� �����մϴ�.
    public void GenerateRandomEventDates()
    {
        eventDates.Clear();

        int currentMonth = GameManager.Instance.gameDate.Month;
        int maxEventCount;

        // �ڡڡ� ����� ���� �ݿ�: 3���� ������ �̺�Ʈ Ƚ�� ���� �ڡڡ�
        if (currentMonth <= 3)
        {
            maxEventCount = 1; // 0~1ȸ
        }
        else if (currentMonth <= 6)
        {
            maxEventCount = 3; // 1~3ȸ
        }
        else if (currentMonth <= 9)
        {
            maxEventCount = 4; // 2~4ȸ
        }
        else
        {
            maxEventCount = 5; // 3~5ȸ
        }

        int eventCount = UnityEngine.Random.Range(0, maxEventCount + 1);

        // ��¥�� ���� HashSet�� ����Ͽ� �ߺ� ����
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
            // �ڡڡ� ����� ���� �ݿ�: ������ ���� ��ȯ ������ ���� �ڡڡ�
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
                maxWolves = 5; // 9�������� 5���� ����
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
                // �ڡڡ� ���� ������ ���� ������ ����Ͽ� �����մϴ�. �ڡڡ�
                int currentYear = GameManager.Instance.gameDate.Year;
                float scaledHealth = baseHealth * Mathf.Pow(difficultyScale, currentYear - 1); // 1������ ���� 0
                float scaledDamage = baseDamage * Mathf.Pow(difficultyScale, currentYear - 1);

                wolfScript.Initialize(this, scaledHealth, scaledDamage);
            }
        }
    }

    public void ReturnWolfToPool(GameObject wolfObj)
    {
        // �ڡڡ� ���� Ǯ�� ��ȯ�ϱ� �� �ʱ�ȭ �ڡڡ�
        wolfObj.GetComponent<Wolf>().isReturning = false; // ��ȯ ���� �ʱ�ȭ
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