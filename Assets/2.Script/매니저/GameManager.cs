using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }

    [Header("게임 데이터")]
    [Tooltip("현재 소지 금액")]
    [SerializeField]
    private int currentMoney = 1000;

    [Header("시간 및 날짜")]
    [Tooltip("게임 시작 시간")]
    public DateTime gameDate = new DateTime(1, 1, 1);
    [Tooltip("현실 60초에 게임 시간 1일")]
    private float timeElapsed = 0f;

    // --- 프로퍼티 ---
    public int CurrentMoney => currentMoney;
    public string CurrentDate => $"{gameDate.Year}년 {gameDate.Month}월 {gameDate.Day}일";

    // 돈이 변경될 때 호출되는 이벤트
    public event Action<int> OnMoneyChanged;

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
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= 60f)
        {
            gameDate = gameDate.AddDays(1);
            timeElapsed -= 60f;
            Debug.Log("새로운 날이 밝았습니다!");
        }
    }

    /// <summary>
    /// 지정된 양만큼 돈을 추가합니다.
    /// </summary>
    /// <param name="amount">추가할 돈의 양</param>
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        Debug.Log($"돈을 {amount}만큼 획득했습니다. 현재 소지 금액: {currentMoney}");
        // 돈이 변경될 때 이벤트를 호출
        OnMoneyChanged?.Invoke(currentMoney);
    }

    /// <summary>
    /// 지정된 양만큼 돈을 사용합니다.
    /// </summary>
    /// <param name="amount">사용할 돈의 양</param>
    /// <returns>돈 사용 성공 여부</returns>
    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            Debug.Log($"돈을 {amount}만큼 사용했습니다. 현재 소지 금액: {currentMoney}");
            // 돈이 변경될 때 이벤트를 호출
            OnMoneyChanged?.Invoke(currentMoney);
            return true;
        }

        Debug.Log($"소지 금액이 부족합니다! 현재 소지 금액: {currentMoney}");
        return false;
    }
}