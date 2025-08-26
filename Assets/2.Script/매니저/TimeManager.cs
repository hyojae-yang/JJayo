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

    // 모든 시간 관련 이벤트를 TimeManager에서 관리합니다.
    public event Action<float> OnTimeChanged; // UI 업데이트를 위한 시간 진행 이벤트
    public event Action<int> OnDayChanged;   // 날짜가 변경될 때 호출되는 이벤트
    public event Action OnMonthChanged;      // 달이 변경될 때 호출되는 이벤트

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

    // Start 메서드에서 GameManager에 대한 참조를 가져옵니다.
    void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }

        // GameManager로부터 초기 데이터를 가져와서 초기화합니다.
        if (gameManager != null)
        {
            Initialize(gameManager.dayLengthInSeconds, gameManager.timeText);
        }
    }

    public void Tick()
    {
        timeElapsed += Time.deltaTime;

        float timeProgress = 1f - (timeElapsed / dayLengthInSeconds);
        // OnTimeChanged 이벤트를 호출하여 UI 등을 업데이트합니다.
        OnTimeChanged?.Invoke(timeProgress);

        if (timeText != null)
        {
            timeText.text = $"남은 시간: {Mathf.RoundToInt(timeProgress * 100)}%";
        }

        if (timeElapsed >= dayLengthInSeconds)
        {
            timeElapsed -= dayLengthInSeconds;
            PassOneDay();
        }
    }

    private void PassOneDay()
    {
        // GameManager의 게임 데이터에 직접 접근하여 날짜를 업데이트합니다.
        gameManager.gameData.day++;

        // OnDayChanged 이벤트를 호출하여 날짜가 바뀌었음을 알립니다.
        OnDayChanged?.Invoke(gameManager.gameData.day);

        if (gameManager.gameData.day > 30)
        {
            gameManager.gameData.day = 1;
            gameManager.gameData.month++;
            // 달이 변경될 때 OnMonthChanged 이벤트를 호출합니다.
            OnMonthChanged?.Invoke();
        }

        if (gameManager.gameData.month > 12)
        {
            gameManager.gameData.month = 1;
            gameManager.gameData.year++;
        }
    }

    public void Initialize(float dayLength, TextMeshProUGUI uiText)
    {
        dayLengthInSeconds = dayLength;
        timeText = uiText;
    }
}