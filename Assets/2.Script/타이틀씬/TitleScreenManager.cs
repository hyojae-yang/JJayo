using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class TitleScreenManager : MonoBehaviour
{
    // === UI Elements ===
    [Header("UI Elements")]
    [Tooltip("Connect the 'How To Play' panel here.")]
    public GameObject howToPlayPanel;

    [Tooltip("Connect the 'Continue' button here.")]
    public Button continueButton;

    // 저장 파일 이름은 GameManager와 동일하게 사용합니다.
    private const string saveFileName = "default_save.json";

    private void Start()
    {
        if (SaveLoadManager.Instance != null && continueButton != null)
        {
            // 수정: HasSaveFile() 메서드에 파일 이름 인자를 전달합니다.
            continueButton.interactable = SaveLoadManager.Instance.HasSaveFile(saveFileName);
        }
    }

    // "Start New Game" button function
    public void StartNewGame()
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.DeleteSaveFile(saveFileName);
        }
        SceneManager.LoadScene("MainScene");
    }

    // "Continue" button function
    public void OnContinueClicked()
    {
        // 수정: 데이터를 직접 로드하는 로직을 제거합니다.
        // 이제 MainScene으로 전환하는 역할만 담당합니다.
        // GameManager가 OnSceneLoaded에서 데이터를 로드하게 됩니다.
        SceneManager.LoadScene("MainScene");
    }

    // "How to Play" button function
    public void ShowHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }
    }

    // "Close Panel" button function
    public void CloseHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }

    // "Quit Game" button function
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}