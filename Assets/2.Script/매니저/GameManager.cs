using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static GameManager Instance { get; private set; }

    [Header("���� ������")]
    [Tooltip("���� ���� �ݾ�")]
    [SerializeField]
    private int currentMoney = 1000;

    [Header("�ð� �� ��¥")]
    [Tooltip("���� ���� �ð�")]
    public DateTime gameDate = new DateTime(1, 1, 1);
    [Tooltip("���� 60�ʿ� ���� �ð� 1��")]
    private float timeElapsed = 0f;

    // --- ������Ƽ ---
    public int CurrentMoney => currentMoney;
    public string CurrentDate => $"{gameDate.Year}�� {gameDate.Month}�� {gameDate.Day}��";

    // ���� ����� �� ȣ��Ǵ� �̺�Ʈ
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
            NotificationManager.Instance.ShowNotification("���ο� ���� ��ҽ��ϴ�!");
        }
    }

    /// <summary>
    /// ������ �縸ŭ ���� �߰��մϴ�.
    /// </summary>
    /// <param name="amount">�߰��� ���� ��</param>
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        NotificationManager.Instance.ShowNotification($"���� {amount}��ŭ ȹ���߽��ϴ�. ���� ���� �ݾ�: {currentMoney}");
        // ���� ����� �� �̺�Ʈ�� ȣ��
        OnMoneyChanged?.Invoke(currentMoney);
    }

    /// <summary>
    /// ������ �縸ŭ ���� ����մϴ�.
    /// </summary>
    /// <param name="amount">����� ���� ��</param>
    /// <returns>�� ��� ���� ����</returns>
    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            NotificationManager.Instance.ShowNotification($"���� {amount}��ŭ ����߽��ϴ�. ���� ���� �ݾ�: {currentMoney}");
            // ���� ����� �� �̺�Ʈ�� ȣ��
            OnMoneyChanged?.Invoke(currentMoney);
            return true;
        }

        NotificationManager.Instance.ShowNotification($"���� �ݾ��� �����մϴ�! ���� ���� �ݾ�: {currentMoney}");
        return false;
    }
}