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
            Debug.Log(gameObject.name + "가 생산했습니다. 현재 " + currentProductionCount + "개");
        }
    }
}