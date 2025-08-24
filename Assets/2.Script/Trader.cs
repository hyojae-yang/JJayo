using UnityEngine;

// 상인의 요구사항 데이터를 담는 클래스
// 이 스크립트를 GameManager에 연결하여 사용합니다.
public class Trader : MonoBehaviour
{
    // 상인이 요구하는 우유의 개수
    [Tooltip("상인이 요구하는 우유의 개수")]
    public int requiredMilkAmount;

    // 상인이 요구하는 우유의 최소 신선도 (100점 만점)
    [Tooltip("상인이 요구하는 우유의 최소 신선도")]
    public int requiredFreshness;

    // 상인이 제시하는 우유 판매 금액
    [Tooltip("상인이 제시하는 우유 판매 금액")]
    public int offeredPrice;

    // 명성도를 관리하는 변수 (추후 추가)
    // public int playerReputation;

    // 필요한 경우, 더 많은 변수를 추가할 수 있습니다.
    // 예: 흥정 성공 확률 등
}