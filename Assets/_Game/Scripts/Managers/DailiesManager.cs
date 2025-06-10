using System.Collections.Generic;
using UnityEngine;

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
