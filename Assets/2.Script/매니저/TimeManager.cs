using UnityEngine;
using System;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Dependencies")]
    public GameManager gameManager;
    public TextMeshProUGUI timeText;

    private float dayLengthInSeconds;
    private float timeElapsed = 0f;

    public event Action<float> OnTimeChanged;
    public event Action<int> OnDayChanged;
    public event Action OnMonthChanged;
    public event Action OnYearChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }

        if (gameManager != null)
        {
            dayLengthInSeconds = gameManager.dayLengthInSeconds;
        }
    }

    void Update()
    {
        if (gameManager == null || gameManager.gameData == null)
        {
            return;
        }

        timeElapsed += Time.deltaTime;

        float timeProgress = 1f - (timeElapsed / dayLengthInSeconds);
        OnTimeChanged?.Invoke(timeProgress);

        if (timeElapsed >= dayLengthInSeconds)
        {
            timeElapsed -= dayLengthInSeconds;
            PassOneDay();
        }
    }

    private void PassOneDay()
    {
        if (gameManager != null && gameManager.gameData != null)
        {
            gameManager.gameData.dailyMilkProduced = 0;
            gameManager.gameData.dailyEggsProduced = 0;
        }

        gameManager.gameData.day++;
        OnDayChanged?.Invoke(gameManager.gameData.day);

        if (gameManager.gameData.day > 30)
        {
            gameManager.gameData.day = 1;
            gameManager.gameData.month++;
            OnMonthChanged?.Invoke();
        }

        if (gameManager.gameData.month > 12)
        {
            gameManager.gameData.month = 1;
            gameManager.gameData.year++;
            OnYearChanged?.Invoke();
        }
    }
}