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
    // �ڡڡ� ����: List<float>���� List<Milk>�� ���� �ڡڡ�
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

    /// <summary>
    /// â�� �ް��� �߰��մϴ�.
    /// </summary>
    /// <param name="newEggsFreshness">�߰��� �ް����� �ż��� ����Ʈ</param>
    public void AddEggs(List<float> newEggsFreshness)
    {
        // AddRange�� �̹� ����Ʈ�� �߰��ϴ� ����� �ϹǷ�,
        // ������ ������ ��� �ż��� ��� ������ �ʿ����� �ʽ��ϴ�.
        // �� �ڵ尡 AddEggs(int amount)�� ����Ǿ�� TraderManager�� RemoveEggs�� ������ �˴ϴ�.
        storedEggFreshness.AddRange(newEggsFreshness);
    }

    // �ڡڡ� �߰�: SaveLoadManager���� �ް� ������ ���� ������ �� �ֵ��� �߰� �ڡڡ�
    public void SetEggCount(int count)
    {
        // ����� �ް� ������ ���� ����Ʈ�� �籸���մϴ�.
        // �ż����� ���Ƿ� 100���� �����ϰų�, �ٸ� ������� �����ؾ� �մϴ�.
        // ����� '����'�� �����ϹǷ� ������ ������ ä��ϴ�.
        storedEggFreshness.Clear();
        for (int i = 0; i < count; i++)
        {
            storedEggFreshness.Add(100f);
        }
    }

    /// <summary>
    /// â�� ������ �߰��մϴ�.
    /// </summary>
    /// <param name="newMilkList">�߰��� �������� ����Ʈ</param>
    // �ڡڡ� ����: List<float>���� List<Milk>�� �Ű������� �޵��� ���� �ڡڡ�
    public void AddMilk(List<Milk> newMilkList)
    {
        // ���Ӱ� ���� ���� ����Ʈ�� ���� â�� ����Ʈ�� �߰��մϴ�.
        storedMilkList.AddRange(newMilkList);

        NotificationManager.Instance.ShowNotification($"â�� ���� {newMilkList.Count}���� �߰��߽��ϴ�. ���� �� ����: {storedMilkList.Count}��");
    }

    // �ڡڡ� �߰�: SaveLoadManager���� ���� ����Ʈ�� ���� ������ �� �ֵ��� �߰� �ڡڡ�
    public void SetMilkList(List<Milk> newMilkList)
    {
        storedMilkList = newMilkList;
    }

    /// <summary>
    /// â�� �ִ� ��� �������� �ż����� ���ҽ�ŵ�ϴ�.
    /// </summary>
    private void DecayFreshness()
    {
        // ���� �ż��� ����
        for (int i = storedMilkList.Count - 1; i >= 0; i--)
        {
            storedMilkList[i].freshness = Mathf.Max(0, storedMilkList[i].freshness - 1);
            if (storedMilkList[i].freshness <= 0)
            {
                storedMilkList.RemoveAt(i);
            }
        }

        // �ް� �ż��� ���� �� ����
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

    /// <summary>
    /// â�� �ִ� �ް��� �� ������ ��ȯ�մϴ�.
    /// </summary>
    public int GetEggCount()
    {
        return storedEggFreshness.Count;
    }

    /// <summary>
    /// â�� �ִ� ������ �� ������ ��ȯ�մϴ�.
    /// </summary>
    public int GetMilkCount()
    {
        return storedMilkList.Count;
    }

    /// <summary>
    /// â�� �ִ� ��� ������ ��� �ż����� ����Ͽ� ��ȯ�մϴ�.
    /// </summary>
    public float GetAverageMilkFreshness()
    {
        if (storedMilkList.Count == 0)
        {
            return 0f;
        }

        float totalFreshness = 0;
        foreach (Milk milk in storedMilkList)
        {
            totalFreshness += milk.freshness;
        }

        return totalFreshness / storedMilkList.Count;
    }

    /// <summary>
    /// ���ο��� �Ǹ��� ������ â���� �����մϴ�.
    /// (�ż����� ���� �������� �Ǹ�)
    /// </summary>
    /// <param name="amount">������ ���� ����</param>
    public void RemoveMilk(int amount)
    {
        if (amount > storedMilkList.Count)
        {
            Debug.LogError("â�� �Ǹ��� ������ �����մϴ�!");
            return;
        }

        storedMilkList.Sort((a, b) => b.freshness.CompareTo(a.freshness));
        storedMilkList.RemoveRange(0, amount);
    }

    /// <summary>
    /// â���� Ư�� ������ŭ �ް��� �����մϴ�.
    /// </summary>
    public void RemoveEggs(int amount)
    {
        if (amount > storedEggFreshness.Count)
        {
            Debug.LogError("â�� �Ǹ��� �ް��� �����մϴ�!");
            return;
        }

        storedEggFreshness.RemoveRange(0, amount);
    }
}