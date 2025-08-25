using UnityEngine;

public class Production : MonoBehaviour
{
    [Header("���� ����")]
    [Tooltip("�ִ� ���귮. �� ���� �����ϸ� ������ ����ϴ�.")]
    public int productionMax = 10;
    [Tooltip("���� �ֱ�(��).")]
    public float productionTime = 10f;
    [Tooltip("���� ����� ������ ����.")]
    public int currentProductionCount = 0; // �� ������ �ν����Ϳ��� 0���� �����Ǿ�� �մϴ�.

    private float productionTimer = 0f;
    private Freshness freshness;

    void Awake()
    {
        freshness = GetComponent<Freshness>();

        // �ڡڡ� �߰��� ���� �κ� �ڡڡ�
        // ���Ұ� ���� ������ ������ ���귮�� 0���� �ʱ�ȭ�մϴ�.
        currentProductionCount = 0;
        productionTimer = 0f;
    }

    void Update()
    {
        // �ִ� ���귮�� �����ϸ� ������ ����ϴ�.
        if (currentProductionCount >= productionMax)
        {
            return;
        }

        productionTimer += Time.deltaTime;
        if (productionTimer >= productionTime)
        {
            currentProductionCount++;
            productionTimer = 0f;

            if (freshness != null)
            {
                freshness.SetFreshnessBasedOnPasture();
            }
        }
    }
}