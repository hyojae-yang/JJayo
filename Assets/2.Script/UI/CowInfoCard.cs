using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CowInfoCard : MonoBehaviour
{
    // ★★★ UI 컴포넌트 변수들 ★★★
    [Header("UI Elements")]
    public Image cowImage;
    public TextMeshProUGUI cowNameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI productionRateText;
    public TextMeshProUGUI maxProductionText;

    // ★★★ 젖소 정보 설정 메서드 ★★★
    public void SetupCowInfo(Animal cow)
    {
        if (cow == null || cow.animalData == null) return;

        // ★★★ 수정된 부분: 모든 데이터는 AnimalData에서 가져옵니다. ★★★
        AnimalData data = cow.animalData;

        // 젖소의 이름과 이미지 설정
        if (cowNameText != null)
        {
            cowNameText.text = data.animalName;
        }
        if (cowImage != null)
        {
            cowImage.sprite = data.animalIcon;
        }

        // 젖소의 체력 정보 설정
        if (healthText != null)
        {
            // ★★★ 수정된 부분: 현재 체력은 Animal 스크립트에서 직접 가져옵니다. ★★★
            healthText.text = $"체력: {data.maxHealth:F0}";
        }

        // 젖소의 생산량 정보 설정
        if (productionRateText != null && maxProductionText != null)
        {
            // ★★★ 수정된 부분: 생산 속도와 최대 생산량도 AnimalData에서 가져옵니다. ★★★
            productionRateText.text = $"생산량: {data.productionInterval}초";
            maxProductionText.text = $"최대 보관량: {data.maxProductionCount}개";
        }
    }
}