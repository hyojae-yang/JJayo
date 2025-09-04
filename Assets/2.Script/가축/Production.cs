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

    // ★★★ 변경된 변수: 이제 AnimalData를 직접 참조합니다. ★★★
    private AnimalData animalData;

    private bool isInitialized = false;

    // AnimalHandler가 호출하는 초기화 메서드
    // ★★★ 매개변수가 변경되었습니다. ★★★
    public void Initialize(AnimalData data)
    {
        this.animalData = data;

        currentProductionCount = 0;
        productionTimer = 0f;
        isInitialized = true;

        SetFreshnessBasedOnPasture();
    }

    void Update()
    {
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
        // ★★★ 신선도 계산 로직이 전면 교체되었습니다. ★★★
        if (animalData == null)
        {
            Debug.LogError("AnimalData가 할당되지 않았습니다. 데이터를 전달하는 코드를 확인해주세요.");
            return;
        }

        // 젖소의 기본 신선도를 가져옵니다.
        float baseFreshness = animalData.baseFreshness;

        // 목초지 매니저로부터 무작위 보너스 값을 가져옵니다.
        float pastureBonus = PastureManager.Instance.GetFreshnessBonus();

        // 두 값을 더하여 최종 신선도를 결정합니다.
        currentFreshness = baseFreshness + pastureBonus;
    }
}