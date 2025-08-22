using UnityEngine;
using UnityEngine.UI;

public class AnimalUI : MonoBehaviour
{
    [Header("UI ����")]
    [Tooltip("���귮 �������� UI�� �����մϴ�.")]
    public Slider productionGauge;

    /// <summary>
    /// ���������� ���� ������ ������Ʈ�մϴ�.
    /// </summary>
    /// <param name="currentCount">���� ����� ������ ����</param>
    /// <param name="maxCount">�ִ� ���� ���� ����</param>
    public void UpdateProductionGauge(int currentCount, int maxCount)
    {
        if (productionGauge != null)
        {
            float productionRate = (float)currentCount / maxCount;
            productionGauge.value = productionRate;

            if (productionRate <= 0.5f)
            {
                productionGauge.fillRect.GetComponent<Image>().color = Color.green;
            }
            else if (productionRate > 0.5f && productionRate <= 0.9f)
            {
                productionGauge.fillRect.GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                productionGauge.fillRect.GetComponent<Image>().color = Color.red;
            }
        }
    }
}