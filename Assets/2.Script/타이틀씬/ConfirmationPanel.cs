using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class ConfirmationPanel : MonoBehaviour
{
    // ★★★ UI 컴포넌트 변수들 ★★★
    public TextMeshProUGUI messageText;
    public Button confirmButton;
    public Button closeButton;

    // ★★★ 확인창을 띄우는 함수 (TitleScreenManager에서 호출) ★★★
    public void Show(string message, UnityAction onConfirmAction)
    {
        gameObject.SetActive(true); // 패널 활성화
        messageText.text = message; // 메시지 설정

        // 기존 리스너 제거 및 새로운 리스너 추가
        confirmButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(onConfirmAction); // 확인 버튼에 로직 연결
        confirmButton.onClick.AddListener(Hide); // 확인 버튼 클릭 시 패널 숨기기
        closeButton.onClick.AddListener(Hide); // 닫기 버튼 클릭 시 패널 숨기기
    }

    // ★★★ 확인창을 숨기는 함수 ★★★
    public void Hide()
    {
        SoundManager.Instance.PlaySFX(SFXType.Button_Click);
        gameObject.SetActive(false); // 패널 비활성화
    }
}