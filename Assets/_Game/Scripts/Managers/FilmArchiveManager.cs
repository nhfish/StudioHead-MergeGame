using System.Collections.Generic;
using UnityEngine;

public class FilmArchiveManager : MonoBehaviour
{
    public static FilmArchiveManager Instance { get; private set; }

    private readonly List<MovieRecipe> archivedRecipes = new();

    [System.Serializable]
    private class ArchiveWrapper
    {
        public List<MovieRecipe> recipes = new();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadArchive();
    }

    public IReadOnlyList<MovieRecipe> ArchivedRecipes => archivedRecipes;

    public void AddRecipe(MovieRecipe recipe)
    {
        if (recipe == null)
            return;

        archivedRecipes.Add(recipe);
        SaveArchive();
    }

    public void SaveArchive()
    {
        var wrapper = new ArchiveWrapper { recipes = archivedRecipes };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString("film_archive", json);
        PlayerPrefs.Save();
    }

    public void LoadArchive()
    {
        archivedRecipes.Clear();

        if (!PlayerPrefs.HasKey("film_archive"))
            return;

        string json = PlayerPrefs.GetString("film_archive");
        var wrapper = JsonUtility.FromJson<ArchiveWrapper>(json);
        if (wrapper != null && wrapper.recipes != null)
            archivedRecipes.AddRange(wrapper.recipes);
    }
}
