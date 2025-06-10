using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public GameObject settingsPanel;
    public Button playButton;
    public Button settingsButton;
    public Button closeSettingsButton;

    private void Awake()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlay);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
        if (closeSettingsButton != null)
            closeSettingsButton.onClick.AddListener(CloseSettings);
    }

    public void OnPlay()
    {
        SceneManager.LoadScene("GameMain");
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
}
