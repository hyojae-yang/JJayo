using UnityEngine;
using TMPro;
using System;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [Header("Dependencies")]
    public GameManager gameManager;
    public TextMeshProUGUI moneyText;

    // 현재 돈을 외부에서 가져올 수 있도록 public 프로퍼티를 추가합니다.
    public int CurrentMoney => gameManager.gameData.money;

    // 돈이 변경될 때 알리는 이벤트
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

    public void Initialize(TextMeshProUGUI moneyUI)
    {
        moneyText = moneyUI;
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        gameManager.gameData.money += amount;
        UpdateMoneyUI();
        OnMoneyChanged?.Invoke(gameManager.gameData.money);
    }

    public bool SpendMoney(int amount)
    {
        if (gameManager.gameData.money >= amount)
        {
            gameManager.gameData.money -= amount;
            UpdateMoneyUI();
            OnMoneyChanged?.Invoke(gameManager.gameData.money);
            return true;
        }
        return false;
    }

    public void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"자금: {gameManager.gameData.money}G";
        }
    }
}