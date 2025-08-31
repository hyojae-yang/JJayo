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
        if (GameManager.Instance == null || GameManager.Instance.gameData == null)
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
    }

    public void Initialize(float dayLength, int year, int month, int day, int reputation)
    {
        dayLengthInSeconds = dayLength;
        if (GameManager.Instance != null && GameManager.Instance.gameData != null)
        {
            GameManager.Instance.gameData.year = year;
            GameManager.Instance.gameData.month = month;
            GameManager.Instance.gameData.day = day;
            GameManager.Instance.gameData.reputation = reputation;
        }
    }

    private void PassOneDay()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameData != null)
        {
            GameManager.Instance.gameData.dailyMilkProduced = 0;
            GameManager.Instance.gameData.dailyEggsProduced = 0;
            GameManager.Instance.gameData.day++;
            OnDayChanged?.Invoke(GameManager.Instance.gameData.day);

            if (GameManager.Instance.gameData.day > 30)
            {
                GameManager.Instance.gameData.day = 1;
                GameManager.Instance.gameData.month++;
                OnMonthChanged?.Invoke();
            }

            if (GameManager.Instance.gameData.month > 12)
            {
                GameManager.Instance.gameData.month = 1;
                GameManager.Instance.gameData.year++;
                OnYearChanged?.Invoke();
            }
        }
    }
}