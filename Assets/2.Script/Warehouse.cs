using System.Collections.Generic;
using UnityEngine;

public class Warehouse : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static Warehouse Instance { get; private set; }

    [Header("â�� ����")]
    [Tooltip("â�� ������ ��� �ް��� �ż��� ���.")]
    public List<float> storedEggFreshness = new List<float>();

    [Tooltip("â�� ������ ��� ������ �ż��� ���.")]
    public List<float> storedMilkFreshness = new List<float>();

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
    /// â�� �ް��� �߰��ϰ� �ż����� ��հ����� ����մϴ�.
    /// </summary>
    /// <param name="newEggsFreshness">�߰��� �ް����� �ż��� ����Ʈ</param>
    public void AddEggs(List<float> newEggsFreshness)
    {
        // 1. ���� �ż��� ���� ���
        float oldTotalFreshness = 0;
        foreach (float freshness in storedEggFreshness)
        {
            oldTotalFreshness += freshness;
        }

        // 2. ���� ���� �ް��� �ż��� ���� ���
        float newTotalFreshness = 0;
        foreach (float freshness in newEggsFreshness)
        {
            newTotalFreshness += freshness;
        }

        // 3. ���ο� ��� �ż��� ���
        float newAverageFreshness = (oldTotalFreshness + newTotalFreshness) / (storedEggFreshness.Count + newEggsFreshness.Count);

        // 4. ���ο� �ް� �߰� �� ��ü �ż��� ����
        storedEggFreshness.AddRange(newEggsFreshness);

        // ���ο� ��հ����� ��ü �ް��� �ż��� ����
        for (int i = 0; i < storedEggFreshness.Count; i++)
        {
            storedEggFreshness[i] = newAverageFreshness;
        }

        NotificationManager.Instance.ShowNotification($"â�� �ް� {newEggsFreshness.Count}���� �߰��߽��ϴ�. ���� �� �ް�: {storedEggFreshness.Count}��, ��� �ż���: {newAverageFreshness:F2}");
    }

    /// <summary>
    /// â�� ������ �߰��ϰ� �ż����� ��հ����� ����մϴ�.
    /// </summary>
    /// <param name="newMilkFreshness">�߰��� �������� �ż��� ����Ʈ</param>
    public void AddMilk(List<float> newMilkFreshness)
    {
        // 1. ���� �ż��� ���� ���
        float oldTotalFreshness = 0;
        foreach (float freshness in storedMilkFreshness)
        {
            oldTotalFreshness += freshness;
        }

        // 2. ���� ���� ������ �ż��� ���� ���
        float newTotalFreshness = 0;
        foreach (float freshness in newMilkFreshness)
        {
            newTotalFreshness += freshness;
        }

        // 3. ���ο� ��� �ż��� ���
        float newAverageFreshness = (oldTotalFreshness + newTotalFreshness) / (storedMilkFreshness.Count + newMilkFreshness.Count);

        // 4. ���ο� ���� �߰� �� ��ü �ż��� ����
        storedMilkFreshness.AddRange(newMilkFreshness);

        // ���ο� ��հ����� ��ü ������ �ż��� ����
        for (int i = 0; i < storedMilkFreshness.Count; i++)
        {
            storedMilkFreshness[i] = newAverageFreshness;
        }

        NotificationManager.Instance.ShowNotification($"â�� ���� {newMilkFreshness.Count}���� �߰��߽��ϴ�. ���� �� ����: {storedMilkFreshness.Count}��, ��� �ż���: {newAverageFreshness:F2}");
    }

    /// <summary>
    /// â�� �ִ� ��� �������� �ż����� ���ҽ�ŵ�ϴ�.
    /// </summary>
    private void DecayFreshness()
    {
        for (int i = 0; i < storedEggFreshness.Count; i++)
        {
            storedEggFreshness[i] = Mathf.Max(0, storedEggFreshness[i] - 1);
        }

        for (int i = 0; i < storedMilkFreshness.Count; i++)
        {
            storedMilkFreshness[i] = Mathf.Max(0, storedMilkFreshness[i] - 1);
        }

        NotificationManager.Instance.ShowNotification("â�� �ִ� ��� �������� �ż����� �����߽��ϴ�.");
    }
    // **********************************************
    // �ڡڡ� ���� �ý��� ���� �Լ� �߰� ���� �ڡڡ�
    // **********************************************

    /// <summary>
    /// â�� �ִ� ������ �� ������ ��ȯ�մϴ�.
    /// </summary>
    public int GetMilkCount()
    {
        return storedMilkFreshness.Count;
    }

    /// <summary>
    /// â�� �ִ� ��� ������ ��� �ż����� ����Ͽ� ��ȯ�մϴ�.
    /// </summary>
    public float GetAverageMilkFreshness()
    {
        if (storedMilkFreshness.Count == 0)
        {
            return 0f;
        }

        float totalFreshness = 0;
        foreach (float freshness in storedMilkFreshness)
        {
            totalFreshness += freshness;
        }

        return totalFreshness / storedMilkFreshness.Count;
    }

    /// <summary>
    /// ���ο��� �Ǹ��� ������ â���� �����մϴ�.
    /// (�ż����� ���� �������� �Ǹ�)
    /// </summary>
    /// <param name="amount">������ ���� ����</param>
    public void RemoveMilk(int amount)
    {
        if (amount > storedMilkFreshness.Count)
        {
            Debug.LogError("â�� �Ǹ��� ������ �����մϴ�!");
            return;
        }

        // 1. ������ �ż��� ������ ���� (��������)
        storedMilkFreshness.Sort((a, b) => b.CompareTo(a));

        // 2. ���� �ż��� �������� �Ǹŷ���ŭ ����
        storedMilkFreshness.RemoveRange(0, amount);
    }

    // **********************************************
    // �ڡڡ� ���� �ý��� ���� �Լ� �߰� �� �ڡڡ�
    // **********************************************
}