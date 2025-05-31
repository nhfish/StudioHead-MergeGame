using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ProductionManager : MonoBehaviour
{
    public UnityEvent<MovieRecipe> OnProductionCompleted;

    private MovieRecipe currentRecipe;
    private float remainingTime;
    private bool isProducing = false;

    public void StartProduction(MovieRecipe recipe, float productionDuration)
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
        remainingTime = productionDuration;

        LockResources(recipe);

        StartCoroutine(ProductionTimer());
        Debug.Log($" Production started. Time: {productionDuration} seconds");
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
