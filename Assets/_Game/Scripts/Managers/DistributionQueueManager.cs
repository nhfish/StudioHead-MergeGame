using System.Collections.Generic;
using UnityEngine;

public class DistributionQueueManager : MonoBehaviour
{
    [Header("UI References")]
    public DistributionPanel distributionPanel;

    private Queue<MovieRecipe> recipeQueue = new();
    private bool isPanelActive = false;

    /// <summary>
    /// Adds a movie recipe to the queue.
    /// </summary>
    public void QueueDistribution(MovieRecipe recipe)
    {
        recipeQueue.Enqueue(recipe);
        TryProcessNext();
    }

    /// <summary>
    /// Starts showing the next recipe if panel isn't busy.
    /// </summary>
    private void TryProcessNext()
    {
        if (isPanelActive || recipeQueue.Count == 0)
            return;

        var nextRecipe = recipeQueue.Dequeue();
        isPanelActive = true;

        distributionPanel.OnDistributionComplete = HandlePanelClosed;
        distributionPanel.OpenWithRecipe(nextRecipe);
        distributionPanel.SetQueueCount(recipeQueue.Count); // Optional display
    }

    /// <summary>
    /// Called by DistributionPanel when a movie is handled.
    /// </summary>
    private void HandlePanelClosed()
    {
        isPanelActive = false;
        TryProcessNext();
    }
}
