using UnityEngine;

public class Production : MonoBehaviour
{
    [Header("생산 설정")]
    [Tooltip("최대 생산량. 이 값에 도달하면 생산을 멈춥니다.")]
    public int productionMax = 10;
    [Tooltip("생산 주기(초).")]
    public float productionTime = 10f;
    [Tooltip("현재 생산된 아이템 개수.")]
    public int currentProductionCount = 0; // 이 변수는 인스펙터에서 0으로 설정되어야 합니다.

    private float productionTimer = 0f;
    private Freshness freshness;

    void Awake()
    {
        freshness = GetComponent<Freshness>();

        // ★★★ 추가된 수정 부분 ★★★
        // 젖소가 새로 생성될 때마다 생산량을 0으로 초기화합니다.
        currentProductionCount = 0;
        productionTimer = 0f;
    }

    void Update()
    {
        // 최대 생산량에 도달하면 생산을 멈춥니다.
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