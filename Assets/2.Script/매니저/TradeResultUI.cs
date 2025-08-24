using UnityEngine;
using TMPro;

public class TradeResultUI : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static TradeResultUI Instance { get; private set; }

    [Header("Dependencies")]
    [Tooltip("결과를 표시할 UI 패널 자체")]
    public GameObject resultPanel;
    [Tooltip("결과 제목 텍스트 (성공/실패)")]
    public TextMeshProUGUI titleText;
    [Tooltip("돈 변화량 텍스트")]
    public TextMeshProUGUI moneyText;
    [Tooltip("명성도 변화량 텍스트")]
    public TextMeshProUGUI reputationText;
    [Tooltip("상세 메시지 텍스트")]
    public TextMeshProUGUI detailMessageText;

    // 상인 매니저 스크립트를 연결하여 패널을 닫을 수 있게 합니다.
    [Tooltip("상인 매니저 스크립트")]
    public TraderManager traderManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (resultPanel != null)
        {
            // 시작 시에는 비활성화 상태로 둡니다.
            resultPanel.SetActive(false);
        }
    }

    /// <summary>
    /// TraderManager로부터 결과를 받아 UI에 표시합니다.
    /// </summary>
    /// <param name="success">거래 성공 여부</param>
    /// <param name="moneyChange">돈 변화량</param>
    /// <param name="reputationChange">명성도 변화량</param>
    /// <param name="message">상세 메시지</param>
    public void DisplayResult(bool success, int moneyChange, int reputationChange, string message)
    {
        // 1. 제목 설정
        titleText.text = success ? "거래 성공!" : "거래 실패";

        // 2. 돈 변화 표시 (+/- 부호와 함께)
        string moneySign = moneyChange >= 0 ? "+" : "-";
        moneyText.text = $"획득 자금: {moneySign}{Mathf.Abs(moneyChange)} 골드";

        // 3. 명성도 변화 표시 (+/- 부호와 함께)
        string repSign = reputationChange >= 0 ? "+" : "";
        reputationText.text = $"명성도 변화: {repSign}{reputationChange} 점";

        // 4. 상세 메시지 설정
        detailMessageText.text = message;

        // 5. 패널 활성화
        resultPanel.SetActive(true);
    }

    /// <summary>
    /// UI의 '확인' 버튼 클릭 시 호출됩니다.
    /// </summary>
    public void OnConfirmButtonClicked()
    {
        // 1. 결과 패널 비활성화
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
        // 3. 상인 패널 닫기
        if (traderManager != null && traderManager.traderUIPanel != null)
        {
            traderManager.traderUIPanel.SetActive(false);
        }

        // 2. 게임 시간 재개
        Time.timeScale = 1;

        
        
    }
}