using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ProductionManager : MonoBehaviour
{
    public UnityEvent<MovieRecipe> OnProductionCompleted;

    private MovieRecipe currentRecipe;
    private float remainingTime;
    private bool isProducing = false;

    public void StartProduction(MovieRecipe recipe, MovieRecipeData recipeData, float productionDuration)
    {
        if (isProducing)
        {
            Debug.LogWarning("Production already in progress.");
            return;
        }

        if (!recipe.HasAllRequiredDepartments())
        {
            Debug.LogWarning("Missing required departments: Camera, Sound, Production.");
            return;
        }

        currentRecipe = recipe;
        isProducing = true;

        int missingOptional = 0;
        foreach (var dept in recipeData.bonusDepartments)
        {
            bool hasItem = recipe.submittedItems.Exists(i => i.department == dept);
            if (!hasItem)
                missingOptional++;
        }

        float adjustedDuration = productionDuration + (productionDuration * recipeData.optionalDepartmentTimePenalty * missingOptional);
        remainingTime = adjustedDuration;

        recipe.moneyReward = Mathf.RoundToInt(recipeData.baseMoneyReward * (1f - recipeData.optionalDepartmentMoneyPenalty * missingOptional));
        recipe.fanReward = Mathf.RoundToInt(recipeData.baseFanReward * (1f - recipeData.optionalDepartmentFanPenalty * missingOptional));

        LockResources(recipe);

        StartCoroutine(ProductionTimer());
        Debug.Log($" Production started. Time: {adjustedDuration} seconds (base {productionDuration})");
    }

    IEnumerator ProductionTimer()
    {
        while (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        isProducing = false;
        Debug.Log(" Production complete!");

        OnProductionCompleted?.Invoke(currentRecipe);
        if (RewardManager.Instance != null)
            RewardManager.Instance.GrantRewards(currentRecipe.moneyReward, currentRecipe.fanReward);
        UnlockTalents(currentRecipe);
        // Items are not unlocked; they are consumed
    }

    void LockResources(MovieRecipe recipe)
    {
        recipe.writer?.Lock();
        recipe.director?.Lock();
        recipe.actor?.Lock();
    }

    void UnlockTalents(MovieRecipe recipe)
    {
        recipe.writer?.Use(); recipe.writer?.Unlock();
        recipe.director?.Use(); recipe.director?.Unlock();
        recipe.actor?.Use(); recipe.actor?.Unlock();
    }
}
