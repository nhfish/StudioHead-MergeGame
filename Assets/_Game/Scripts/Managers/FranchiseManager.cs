using System.Collections.Generic;
using UnityEngine;

public class FranchiseManager : MonoBehaviour
{
    public static FranchiseManager Instance { get; private set; }

    // Tracks how many films exist for each franchise title
    private readonly Dictionary<string, int> franchiseCounts = new();

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

    /// <summary>
    /// Registers a movie by title. If the title already exists,
    /// it increments the sequel count.
    /// </summary>
    public void RegisterMovie(string title)
    {
        if (string.IsNullOrEmpty(title))
            return;

        if (franchiseCounts.ContainsKey(title))
            franchiseCounts[title]++;
        else
            franchiseCounts[title] = 1;
    }

    /// <summary>
    /// Returns how many movies are in the given franchise so far.
    /// </summary>
    public int GetSequelNumber(string title)
    {
        return franchiseCounts.TryGetValue(title, out int count) ? count : 0;
    }
}
