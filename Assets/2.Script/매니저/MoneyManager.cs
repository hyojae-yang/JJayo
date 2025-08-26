using UnityEngine;
using TMPro;
using System;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [Header("Dependencies")]
    public GameManager gameManager;
    public TextMeshProUGUI moneyText;

    // ���� ���� �ܺο��� ������ �� �ֵ��� public ������Ƽ�� �߰��մϴ�.
    public int CurrentMoney => gameManager.gameData.money;

    // ���� ����� �� �˸��� �̺�Ʈ
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
            moneyText.text = $"�ڱ�: {gameManager.gameData.money}G";
        }
    }
}