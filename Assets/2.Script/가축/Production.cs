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
            Debug.Log(gameObject.name + "�� �����߽��ϴ�. ���� " + currentProductionCount + "��");
        }
    }
}