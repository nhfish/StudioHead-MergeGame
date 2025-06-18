using UnityEngine;

public class FameManager : MonoBehaviour
{
    public static FameManager Instance { get; private set; }

    [Header("Fame Settings")]
    public int currentFameLevel = 1;
    public float fameProgress = 0f;
    public float fameRequiredForNextLevel = 100f;

    public System.Action<int> FameLevelChanged;
    public System.Action<float> FameProgressChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddFame(float amount)
    {
        if (amount <= 0) return;

        fameProgress += amount;
        FameProgressChanged?.Invoke(fameProgress);

        // Check for level up
        while (fameProgress >= fameRequiredForNextLevel)
        {
            fameProgress -= fameRequiredForNextLevel;
            currentFameLevel++;
            FameLevelChanged?.Invoke(currentFameLevel);
            
            // Increase requirement for next level
            fameRequiredForNextLevel *= 1.5f;
        }
    }

    public float GetFameProgressPercentage()
    {
        return fameProgress / fameRequiredForNextLevel;
    }

    public void SetFameLevel(int level, float progress = 0f)
    {
        currentFameLevel = Mathf.Max(1, level);
        fameProgress = Mathf.Clamp(progress, 0f, fameRequiredForNextLevel);
        
        FameLevelChanged?.Invoke(currentFameLevel);
        FameProgressChanged?.Invoke(fameProgress);
    }
} 