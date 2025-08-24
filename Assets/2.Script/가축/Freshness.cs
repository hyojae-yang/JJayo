using UnityEngine;

public class Freshness : MonoBehaviour
{
    [Header("�ż��� ����")]
    [Tooltip("���� ������ �ż��� (0-100).")]
    public float currentFreshness;

    // ���� �� �ż����� �����ϴ� �Լ�
    public void SetFreshnessBasedOnPasture()
    {
        int currentLevel = GameManager.Instance.CurrentPastureLevel;

        if (GameManager.Instance.pastureUpgradeData == null)
        {
            Debug.LogError("PastureUpgradeData�� GameManager�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        (int min, int max) range = GameManager.Instance.pastureUpgradeData.GetFreshnessRange(currentLevel);

        currentFreshness = Random.Range(range.min, range.max + 1);
    }
}