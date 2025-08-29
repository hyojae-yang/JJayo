using UnityEngine;
using System;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [Header("Dependencies")]
    public GameManager gameManager;

    public int CurrentMoney => gameManager.gameData.money;

    public event Action<int> OnMoneyChanged;

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

    public void AddMoney(int amount)
    {
        gameManager.gameData.money += amount;
        // OnMoneyChanged 이벤트를 호출하여 PlayerUI에게 업데이트를 요청합니다.
        OnMoneyChanged?.Invoke(gameManager.gameData.money);
    }

    public bool SpendMoney(int amount)
    {
        if (gameManager.gameData.money >= amount)
        {
            gameManager.gameData.money -= amount;
            OnMoneyChanged?.Invoke(gameManager.gameData.money);
            return true;
        }
        return false;
    }
}