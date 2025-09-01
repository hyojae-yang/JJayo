using UnityEngine;
using System;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    // gameManager 변수 제거
    // private GameManager gameManager;

    // GameManager.Instance를 직접 참조하도록 변경
    public int CurrentMoney => GameManager.Instance.CurrentGameData.money;

    public event Action<int> OnMoneyChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // TimeManager와 동일하게 DontDestroyOnLoad를 추가하여 씬 전환 시 파괴 방지
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // InitializeMoney 메서드 제거. GameManager가 직접 게임 데이터를 초기화합니다.

    public void AddMoney(int amount)
    {
        // GameManager.Instance를 직접 참조하도록 변경
        GameManager.Instance.CurrentGameData.money += amount;
        OnMoneyChanged?.Invoke(GameManager.Instance.CurrentGameData.money);
    }

    public bool SpendMoney(int amount)
    {
        // GameManager.Instance를 직접 참조하도록 변경
        if (GameManager.Instance.CurrentGameData.money >= amount)
        {
            GameManager.Instance.CurrentGameData.money -= amount;
            OnMoneyChanged?.Invoke(GameManager.Instance.CurrentGameData.money);
            return true;
        }
        return false;
    }
}