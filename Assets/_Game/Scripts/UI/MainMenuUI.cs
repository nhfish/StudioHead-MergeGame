using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsPanel;
    public Button playButton;
    public Button continueButton;
    public Button newGameButton;
    public Button settingsButton;
    public Button closeSettingsButton;

    [Header("Save System")]
    public GameInitializer gameInitializer;

    private void Awake()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlay);
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinue);
        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGame);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
        if (closeSettingsButton != null)
            closeSettingsButton.onClick.AddListener(CloseSettings);
    }

    private void Start()
    {
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        bool hasSaveData = gameInitializer != null && gameInitializer.HasSaveData();
        
        if (continueButton != null)
            continueButton.gameObject.SetActive(hasSaveData);
        
        if (newGameButton != null)
            newGameButton.gameObject.SetActive(hasSaveData);
        
        if (playButton != null)
            playButton.gameObject.SetActive(!hasSaveData);
    }

    public void OnPlay()
    {
        // Start new game
        SceneManager.LoadScene("GameMain");
    }

    public void OnContinue()
    {
        // Continue existing game
        if (gameInitializer != null)
        {
            gameInitializer.LoadGame();
        }
        SceneManager.LoadScene("GameMain");
    }

    public void OnNewGame()
    {
        // Start fresh game (clear save data)
        if (SaveSystem.Instance != null)
        {
            // Delete save files
            string savePath = System.IO.Path.Combine(Application.persistentDataPath, SaveSystem.Instance.saveFileName);
            string backupPath = System.IO.Path.Combine(Application.persistentDataPath, SaveSystem.Instance.backupFileName);
            
            if (System.IO.File.Exists(savePath))
                System.IO.File.Delete(savePath);
            if (System.IO.File.Exists(backupPath))
                System.IO.File.Delete(backupPath);
        }
        
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
