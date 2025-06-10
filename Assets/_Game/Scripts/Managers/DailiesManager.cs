using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DailiesManager : MonoBehaviour
{
    [Header("References")]
    public DistributionQueueManager distributionQueue;
    public ProductionManager productionManager;

    private class RecipeState
    {
        public int pendingAttempts;
        public bool productionComplete;
    }

    private readonly Dictionary<MovieRecipe, RecipeState> recipeStates = new();

    /// <summary>
    /// Returns the first recipe that has a pending dailies attempt, or null if none.
    /// </summary>
    public MovieRecipe GetRecipeWithPendingDaily()
    {
        foreach (var kvp in recipeStates)
        {
            if (kvp.Value.pendingAttempts > 0)
                return kvp.Key;
        }
        return null;
    }

    /// <summary>
    /// Launches the dailies puzzle scene for the given recipe.
    /// </summary>
    public void LaunchPuzzle(MovieRecipe recipe)
    {
        if (recipe == null)
            return;

        StartCoroutine(LoadPuzzleRoutine(recipe));
    }

    IEnumerator LoadPuzzleRoutine(MovieRecipe recipe)
    {
        var load = SceneManager.LoadSceneAsync("DailiesPuzzle", LoadSceneMode.Additive);
        yield return load;

        Scene puzzleScene = SceneManager.GetSceneByName("DailiesPuzzle");
        if (puzzleScene.IsValid())
        {
            foreach (var root in puzzleScene.GetRootGameObjects())
            {
                var board = root.GetComponentInChildren<DailiesBoardManager>();
                if (board != null)
                {
                    board.dailiesManager = this;
                    board.currentRecipe = recipe;
                    break;
                }
            }
        }
    }

    public void Initialize(ProductionManager manager)
    {
        if (productionManager != null)
        {
            productionManager.OnDailiesAvailable.RemoveListener(HandleDailiesAvailable);
            productionManager.OnProductionCompleted.RemoveListener(HandleProductionCompleted);
        }

        productionManager = manager;

        if (productionManager != null)
        {
            productionManager.OnDailiesAvailable.AddListener(HandleDailiesAvailable);
            productionManager.OnProductionCompleted.AddListener(HandleProductionCompleted);
        }
    }

    private void HandleDailiesAvailable(float milestone, MovieRecipe recipe)
    {
        if (!recipeStates.TryGetValue(recipe, out var state))
            recipeStates[recipe] = state = new RecipeState();

        state.pendingAttempts++;
        Debug.Log($" Dailies milestone reached for recipe. Pending attempts: {state.pendingAttempts}");
    }

    private void HandleProductionCompleted(MovieRecipe recipe)
    {
        if (!recipeStates.TryGetValue(recipe, out var state))
            recipeStates[recipe] = state = new RecipeState();

        state.productionComplete = true;
        Debug.Log(" Production complete. Waiting for all dailies attempts to finish.");
        TryQueueRecipe(recipe, state);
    }

    public void PlayOrSkipDaily(MovieRecipe recipe, int score = -1)
    {
        if (!recipeStates.TryGetValue(recipe, out var state))
            return;

        if (state.pendingAttempts > 0)
            state.pendingAttempts--;

        recipe.dailiesPlayed++;

        if (score >= 0)
        {
            recipe.dailyScores.Add(score);
            recipe.rewardMultiplier += Mathf.Clamp01(score / 100f);
        }

        Debug.Log($" Dailies attempt resolved. Remaining: {state.pendingAttempts}");
        TryQueueRecipe(recipe, state);
    }

    private void TryQueueRecipe(MovieRecipe recipe, RecipeState state)
    {
        if (state.productionComplete && state.pendingAttempts == 0)
        {
            distributionQueue.QueueDistribution(recipe);
            recipeStates.Remove(recipe);
            Debug.Log(" All dailies handled. Movie queued for distribution.");
        }
    }
}
