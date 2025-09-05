using UnityEngine;
using System;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Dependencies")]
    private float dayLengthInSeconds;
    private float timeElapsed = 0f;

    public float timeProgress;

    public event Action<float> OnTimeChanged;
    public event Action<int> OnDayChanged;
    public event Action OnMonthChanged;
    public event Action OnYearChanged;

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

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentGameData == null)
        {
            return;
        }

        timeElapsed += Time.deltaTime;

        timeProgress = 1f - (timeElapsed / dayLengthInSeconds);

        OnTimeChanged?.Invoke(timeProgress);

        if (timeElapsed >= dayLengthInSeconds)
        {
            timeElapsed -= dayLengthInSeconds;
            PassOneDay();
        }
        // ★★★ 이 줄을 추가합니다. ★★★
        GameManager.Instance.CurrentGameData.totalPlayTime += Time.deltaTime;
    }

    public void Initialize(float dayLength, int year, int month, int day, int reputation)
    {
        dayLengthInSeconds = dayLength;
        if (GameManager.Instance != null && GameManager.Instance.CurrentGameData != null)
        {
            GameManager.Instance.CurrentGameData.year = year;
            GameManager.Instance.CurrentGameData.month = month;
            GameManager.Instance.CurrentGameData.day = day;
            GameManager.Instance.CurrentGameData.reputation = reputation;
        }
    }

    private void PassOneDay()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentGameData != null)
        {
            GameManager.Instance.CurrentGameData.dailyMilkProduced = 0;
            GameManager.Instance.CurrentGameData.dailyEggsProduced = 0;
            GameManager.Instance.CurrentGameData.day++;
            OnDayChanged?.Invoke(GameManager.Instance.CurrentGameData.day);

            if (GameManager.Instance.CurrentGameData.day > 30)
            {
                GameManager.Instance.CurrentGameData.day = 1;
                GameManager.Instance.CurrentGameData.month++;
                OnMonthChanged?.Invoke();
            }

            if (GameManager.Instance.CurrentGameData.month > 12)
            {
                GameManager.Instance.CurrentGameData.month = 1;
                GameManager.Instance.CurrentGameData.year++;
                OnYearChanged?.Invoke();
            }
        }
    }
}