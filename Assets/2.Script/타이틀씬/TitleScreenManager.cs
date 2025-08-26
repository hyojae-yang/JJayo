using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // For scene loading
using System.IO; // For file system operations

public class TitleScreenManager : MonoBehaviour
{
    // === UI Elements ===
    [Header("UI Elements")]
    [Tooltip("Connect the 'How To Play' panel here.")]
    public GameObject howToPlayPanel;

    [Tooltip("Connect the 'Continue' button here.")]
    public Button continueButton;

    private void Start()
    {
        // Check if a save file exists and enable/disable the continue button accordingly.
        // This check is crucial for a good user experience.
        if (SaveLoadManager.Instance != null && continueButton != null)
        {
            continueButton.interactable = SaveLoadManager.Instance.HasSaveFile();
        }
    }

    // "Start New Game" button function
    public void StartNewGame()
    {
        // Load the "MainScene" scene.
        SceneManager.LoadScene("MainScene");
    }

    // "Continue" button function
    public void OnContinueClicked()
    {
        // Load the saved game from the SaveLoadManager
        if (SaveLoadManager.Instance != null && SaveLoadManager.Instance.HasSaveFile())
        {
            SaveLoadManager.Instance.LoadGame();

            // Once the game is loaded, switch to the main scene.
            SceneManager.LoadScene("MainScene");
        }
    }

    // "How to Play" button function
    public void ShowHowToPlay()
    {
        // Activate the "How to Play" panel.
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }
    }

    // "Close Panel" button function
    public void CloseHowToPlay()
    {
        // Deactivate the "How to Play" panel.
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }

    // "Quit Game" button function
    public void QuitGame()
    {
        // Quit the application.
        Application.Quit();

#if UNITY_EDITOR
        // Code for testing in the Unity Editor.
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}