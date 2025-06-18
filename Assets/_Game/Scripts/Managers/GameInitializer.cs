using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    [Header("Save System")]
    public GameObject saveSystemPrefab;
    public GameObject fameManagerPrefab;
    public GameObject saveIndicatorPrefab;

    [Header("Initialization")]
    public bool loadSaveOnStart = true;
    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "GameMain";

    void Start()
    {
        InitializeManagers();
        
        if (loadSaveOnStart)
        {
            LoadGameData();
        }
    }

    private void InitializeManagers()
    {
        // Initialize SaveSystem if it doesn't exist
        if (SaveSystem.Instance == null && saveSystemPrefab != null)
        {
            GameObject saveSystem = Instantiate(saveSystemPrefab);
            DontDestroyOnLoad(saveSystem);
        }

        // Initialize FameManager if it doesn't exist
        if (FameManager.Instance == null && fameManagerPrefab != null)
        {
            GameObject fameManager = Instantiate(fameManagerPrefab);
            DontDestroyOnLoad(fameManager);
        }

        // Initialize SaveIndicator if it doesn't exist
        if (saveIndicatorPrefab != null)
        {
            GameObject saveIndicator = Instantiate(saveIndicatorPrefab);
            DontDestroyOnLoad(saveIndicator);
            
            // Assign to SaveSystem if it exists
            if (SaveSystem.Instance != null)
            {
                SaveSystem.Instance.saveIndicatorPrefab = saveIndicator;
            }
        }
    }

    private void LoadGameData()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.LoadCompleted += OnLoadCompleted;
            SaveSystem.Instance.LoadError += OnLoadError;
            SaveSystem.Instance.LoadGame();
        }
    }

    private void OnLoadCompleted()
    {
        Debug.Log("Game data loaded successfully");
        
        // Unsubscribe from events
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.LoadCompleted -= OnLoadCompleted;
            SaveSystem.Instance.LoadError -= OnLoadError;
        }
    }

    private void OnLoadError(string error)
    {
        Debug.LogWarning($"Failed to load game data: {error}. Starting fresh game.");
        
        // Unsubscribe from events
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.LoadCompleted -= OnLoadCompleted;
            SaveSystem.Instance.LoadError -= OnLoadError;
        }
    }

    // Public method to manually save game
    public void SaveGame()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGame();
        }
    }

    // Public method to manually load game
    public void LoadGame()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.LoadGame();
        }
    }

    // Public method to check if save data exists
    public bool HasSaveData()
    {
        return SaveSystem.Instance != null && SaveSystem.Instance.HasSaveData();
    }
} 