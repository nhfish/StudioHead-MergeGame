using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ProductionManager : MonoBehaviour
{
    public UnityEvent<MovieRecipe> OnProductionCompleted;
    public UnityEvent<float, MovieRecipe> OnDailiesAvailable;
    public UnityEvent<float> OnProductionProgress;

    private MovieRecipe currentRecipe;
    private float remainingTime;
    private bool isProducing = false;

    private float totalTime;
    private float elapsedTime;
    private Queue<float> progressMilestones = new Queue<float>();

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

        if (!recipeData.allowPartialTalent && recipe.TalentCount < 3)
        {
            Debug.LogWarning("All talent slots must be filled to start production.");
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
        totalTime = adjustedDuration;
        elapsedTime = 0f;
        progressMilestones = new Queue<float>(new float[] { 0.33f, 0.66f, 1f });

        recipe.moneyReward = Mathf.RoundToInt(recipeData.baseMoneyReward * (1f - recipeData.optionalDepartmentMoneyPenalty * missingOptional));
        recipe.fanReward = Mathf.RoundToInt(recipeData.baseFanReward * (1f - recipeData.optionalDepartmentFanPenalty * missingOptional));

        if (recipeData.grantSynergyBonus && recipe.HasGenreSynergy())
        {
            FranchiseManager.Instance?.RegisterMovie(recipeData.movieTitle);

            if (recipeData.synergyBonusConfig != null)
            {
                int highestTier = GetHighestTier(recipe);
                float bonus = recipeData.synergyBonusConfig.GetBonusForTier(highestTier);
                recipe.moneyReward = Mathf.RoundToInt(recipe.moneyReward * (1f + bonus));
                recipe.fanReward = Mathf.RoundToInt(recipe.fanReward * (1f + bonus));
            }
        }

        LockResources(recipe);

        StartCoroutine(ProductionTimer());
        Debug.Log($" Production started. Time: {adjustedDuration} seconds (base {productionDuration})");
    }

    IEnumerator ProductionTimer()
    {
        while (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            elapsedTime += Time.deltaTime;

            float progress = totalTime > 0f ? Mathf.Clamp01(elapsedTime / totalTime) : 0f;
            OnProductionProgress?.Invoke(progress);

            if (progressMilestones.Count > 0 && progress >= progressMilestones.Peek())
            {
                float milestone = progressMilestones.Dequeue();
                OnDailiesAvailable?.Invoke(milestone, currentRecipe);
            }

            yield return null;
        }

        isProducing = false;
        Debug.Log(" Production complete!");

        OnProductionCompleted?.Invoke(currentRecipe);
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

    int GetHighestTier(MovieRecipe recipe)
    {
        int highestItemTier = 1;
        if (recipe.submittedItems != null && recipe.submittedItems.Count > 0)
            highestItemTier = recipe.submittedItems.Max(i => (int)i.tier + 1);

        int highestTalentTier = 0;
        if (recipe.writer != null)
            highestTalentTier = Mathf.Max(highestTalentTier, RarityToTier(recipe.writer.baseData.rarity));
        if (recipe.director != null)
            highestTalentTier = Mathf.Max(highestTalentTier, RarityToTier(recipe.director.baseData.rarity));
        if (recipe.actor != null)
            highestTalentTier = Mathf.Max(highestTalentTier, RarityToTier(recipe.actor.baseData.rarity));

        return Mathf.Max(highestItemTier, highestTalentTier);
    }

    int RarityToTier(TalentRarity rarity)
    {
        return rarity switch
        {
            TalentRarity.AList => 4,
            TalentRarity.BList => 3,
            TalentRarity.CList => 2,
            TalentRarity.DList => 1,
            _ => 1
        };
    }
}
