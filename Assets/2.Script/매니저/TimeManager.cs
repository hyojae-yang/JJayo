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

    // ��� �ð� ���� �̺�Ʈ�� TimeManager���� �����մϴ�.
    public event Action<float> OnTimeChanged; // UI ������Ʈ�� ���� �ð� ���� �̺�Ʈ
    public event Action<int> OnDayChanged;   // ��¥�� ����� �� ȣ��Ǵ� �̺�Ʈ
    public event Action OnMonthChanged;      // ���� ����� �� ȣ��Ǵ� �̺�Ʈ

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

    // Start �޼��忡�� GameManager�� ���� ������ �����ɴϴ�.
    void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }

        // GameManager�κ��� �ʱ� �����͸� �����ͼ� �ʱ�ȭ�մϴ�.
        if (gameManager != null)
        {
            Initialize(gameManager.dayLengthInSeconds, gameManager.timeText);
        }
    }

    public void Tick()
    {
        timeElapsed += Time.deltaTime;

        float timeProgress = 1f - (timeElapsed / dayLengthInSeconds);
        // OnTimeChanged �̺�Ʈ�� ȣ���Ͽ� UI ���� ������Ʈ�մϴ�.
        OnTimeChanged?.Invoke(timeProgress);

        if (timeText != null)
        {
            timeText.text = $"���� �ð�: {Mathf.RoundToInt(timeProgress * 100)}%";
        }

        if (timeElapsed >= dayLengthInSeconds)
        {
            timeElapsed -= dayLengthInSeconds;
            PassOneDay();
        }
    }

    private void PassOneDay()
    {
        // GameManager�� ���� �����Ϳ� ���� �����Ͽ� ��¥�� ������Ʈ�մϴ�.
        gameManager.gameData.day++;

        // OnDayChanged �̺�Ʈ�� ȣ���Ͽ� ��¥�� �ٲ������ �˸��ϴ�.
        OnDayChanged?.Invoke(gameManager.gameData.day);

        if (gameManager.gameData.day > 30)
        {
            gameManager.gameData.day = 1;
            gameManager.gameData.month++;
            // ���� ����� �� OnMonthChanged �̺�Ʈ�� ȣ���մϴ�.
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