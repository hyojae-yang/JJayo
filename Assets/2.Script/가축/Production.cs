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

    [Header("신선도 설정")]
    [Tooltip("현재 우유의 신선도 (0-100).")]
    public float currentFreshness;

    private float productionTimer = 0f;
    private int pastureLevel;
    private PastureUpgradeData pastureUpgradeData;

    // 추가된 부분: 초기화 여부를 확인하는 플래그
    private bool isInitialized = false;

    // AnimalHandler가 호출하는 초기화 메서드
    public void Initialize(int level, PastureUpgradeData data)
    {
        this.pastureLevel = level;
        this.pastureUpgradeData = data;

        currentProductionCount = 0;
        productionTimer = 0f;

        // 데이터 할당 후 초기화 완료 플래그를 true로 설정
        isInitialized = true;

        SetFreshnessBasedOnPasture();
    }

    void Update()
    {
        // 수정된 부분: 초기화가 완료된 후에만 로직 실행
        if (!isInitialized) return;

        if (currentProductionCount >= productionMax)
        {
            return;
        }

        productionTimer += Time.deltaTime;
        if (productionTimer >= productionTime)
        {
            currentProductionCount++;
            productionTimer = 0f;

            // 일일 우유 생산량 기록
            if (GameManager.Instance != null && GameManager.Instance.CurrentGameData != null)
            {
                GameManager.Instance.CurrentGameData.dailyMilkProduced++;
            }

            SetFreshnessBasedOnPasture();
        }
    }

    // 신선도를 설정하는 함수
    private void SetFreshnessBasedOnPasture()
    {
        // isInitialized 체크가 추가되었으므로, pastureUpgradeData가 null일 가능성이 사라짐
        if (pastureUpgradeData == null)
        {
            Debug.LogError("PastureUpgradeData가 할당되지 않았습니다. 데이터를 전달하는 코드를 확인해주세요.");
            return;
        }

        (int min, int max) range = pastureUpgradeData.GetFreshnessRange(pastureLevel);

        currentFreshness = Random.Range(range.min, range.max + 1);
    }
}