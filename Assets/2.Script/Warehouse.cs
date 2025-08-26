using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Warehouse : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static Warehouse Instance { get; private set; }

    [Header("â�� ����")]
    [Tooltip("â�� ������ ��� �ް��� �ż��� ���.")]
    public List<float> storedEggFreshness = new List<float>();

    [Tooltip("â�� ������ ��� ������ �ż��� ���.")]
    public List<Milk> storedMilkList = new List<Milk>();

    [Tooltip("�������� �ż����� �����ϴ� �ֱ�(��).")]
    public float freshnessDecayInterval = 120f;
    private float decayTimer = 0f;

    private void Awake()
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
        decayTimer += Time.deltaTime;
        if (decayTimer >= freshnessDecayInterval)
        {
            DecayFreshness();
            decayTimer = 0f;
        }
    }

    public void AddEggs(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            storedEggFreshness.Add(100f);
        }
    }

    public void AddMilk(List<Milk> newMilkList)
    {
        storedMilkList.AddRange(newMilkList);
        NotificationManager.Instance.ShowNotification($"â�� ���� {newMilkList.Count}���� �߰��߽��ϴ�. ���� �� ����: {storedMilkList.Count}��");
    }

    private void DecayFreshness()
    {
        for (int i = storedMilkList.Count - 1; i >= 0; i--)
        {
            storedMilkList[i].freshness = Mathf.Max(0, storedMilkList[i].freshness - 1);
            if (storedMilkList[i].freshness <= 0)
            {
                storedMilkList.RemoveAt(i);
            }
        }

        for (int i = storedEggFreshness.Count - 1; i >= 0; i--)
        {
            storedEggFreshness[i] = Mathf.Max(0, storedEggFreshness[i] - 1);
            if (storedEggFreshness[i] <= 0)
            {
                storedEggFreshness.RemoveAt(i);
            }
        }
        NotificationManager.Instance.ShowNotification("â�� �ִ� ��� �������� �ż����� �����߽��ϴ�.");
    }

    public int GetEggCount()
    {
        return storedEggFreshness.Count;
    }

    public int GetMilkCount()
    {
        return storedMilkList.Count;
    }

    public float GetAverageMilkFreshness()
    {
        return storedMilkList.Any() ? storedMilkList.Average(m => m.freshness) : 0f;
    }

    public void RemoveMilk(int amount)
    {
        if (amount > storedMilkList.Count)
        {
            Debug.LogError("â�� �Ǹ��� ������ �����մϴ�!");
            return;
        }
        storedMilkList.Sort((a, b) => a.freshness.CompareTo(b.freshness));
        storedMilkList.RemoveRange(0, amount);
    }

    public void RemoveEggs(int amount)
    {
        if (amount > storedEggFreshness.Count)
        {
            Debug.LogError("â�� �Ǹ��� �ް��� �����մϴ�!");
            return;
        }
        storedEggFreshness.RemoveRange(0, amount);
    }

    // �ڡڡ� �߰�: TraderManager�� ȣ���ϴ� �޼������ ���⿡ �߰��մϴ�. �ڡڡ�
    public bool CanSellMilk(int requiredAmount, float requiredFreshness)
    {
        if (GetMilkCount() < requiredAmount)
        {
            NotificationManager.Instance.ShowNotification("â�� ������ �����մϴ�!");
            return false;
        }

        if (GetAverageMilkFreshness() < requiredFreshness)
        {
            NotificationManager.Instance.ShowNotification("������ ��� �ż����� ���� ������ �䱸 ������ �������� ���մϴ�.");
            return false;
        }
        return true;
    }

    public void SellMilk(int amount)
    {
        if (CanSellMilk(amount, 0)) // ���⼭ �ż��� ������ �̹� TraderManager���� üũ�����Ƿ� 0���� ����
        {
            // �ż����� ���� �������� �Ǹ� (���� ������ �������� ó��)
            storedMilkList.Sort((a, b) => a.freshness.CompareTo(b.freshness));
            storedMilkList.RemoveRange(0, amount);

            NotificationManager.Instance.ShowNotification($"���� {amount}���� ���ο��� �Ǹ��߽��ϴ�!");
        }
    }
}