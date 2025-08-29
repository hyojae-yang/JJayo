using UnityEngine;

public class Production : MonoBehaviour
{
    [Header("생산 설정")]
    [Tooltip("최대 생산량. 이 값에 도달하면 생산을 멈춥니다.")]
    public int productionMax = 10;
    [Tooltip("생산 주기(초).")]
    public float productionTime = 10f;
    [Tooltip("현재 생산된 아이템 개수.")]
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

            // ★★★ 추가된 로직: 일일 우유 생산량 기록 ★★★
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