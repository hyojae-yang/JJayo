using UnityEngine;

public class Production : MonoBehaviour
{
    [Header("���� ����")]
    [Tooltip("�ִ� ���귮. �� ���� �����ϸ� ������ ����ϴ�.")]
    public int productionMax = 10;
    [Tooltip("���� �ֱ�(��).")]
    public float productionTime = 10f;
    [Tooltip("���� ����� ������ ����.")]
    public int currentProductionCount = 0;

    private float productionTimer = 0f;
    private Freshness freshness;

    void Awake()
    {
        freshness = GetComponent<Freshness>();
        currentProductionCount = 0;
        productionTimer = 0f;
    }

    void Update()
    {
        if (currentProductionCount >= productionMax)
        {
            return;
        }

        productionTimer += Time.deltaTime;
        if (productionTimer >= productionTime)
        {
            currentProductionCount++;
            productionTimer = 0f;

            // �ڡڡ� �߰��� ����: ���� ���� ���귮 ��� �ڡڡ�
            if (GameManager.Instance != null && GameManager.Instance.gameData != null)
            {
                GameManager.Instance.gameData.dailyMilkProduced++;
            }

            if (freshness != null)
            {
                freshness.SetFreshnessBasedOnPasture();
            }
        }
    }
}