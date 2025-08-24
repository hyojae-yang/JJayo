using UnityEngine;

public class Freshness : MonoBehaviour
{
    [Header("신선도 설정")]
    [Tooltip("현재 우유의 신선도 (0-100).")]
    public float currentFreshness;

    // 생산 시 신선도를 설정하는 함수
    public void SetFreshnessBasedOnPasture()
    {
        int currentLevel = GameManager.Instance.CurrentPastureLevel;

        if (GameManager.Instance.pastureUpgradeData == null)
        {
            Debug.LogError("PastureUpgradeData가 GameManager에 할당되지 않았습니다.");
            return;
        }

        (int min, int max) range = GameManager.Instance.pastureUpgradeData.GetFreshnessRange(currentLevel);

        currentFreshness = Random.Range(range.min, range.max + 1);
    }
}