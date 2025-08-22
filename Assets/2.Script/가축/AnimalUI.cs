using UnityEngine;
using UnityEngine.UI;

public class AnimalUI : MonoBehaviour
{
    [Header("UI 연결")]
    [Tooltip("생산량 게이지바 UI를 연결합니다.")]
    public Slider productionGauge;

    /// <summary>
    /// 게이지바의 값과 색상을 업데이트합니다.
    /// </summary>
    /// <param name="currentCount">현재 생산된 아이템 개수</param>
    /// <param name="maxCount">최대 생산 가능 개수</param>
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