using UnityEngine;

public class ProductionSystemLinker : MonoBehaviour
{
    public ProductionManager productionManager;
    public DistributionQueueManager distributionQueue;

    void Start()
    {
        // Register the callback so we know when a movie is done
        productionManager.OnProductionCompleted.AddListener(HandleProductionComplete);
    }

    void HandleProductionComplete(MovieRecipe recipe)
    {
        Debug.Log(" Production complete! Forwarding movie to distribution queue.");
        distributionQueue.QueueDistribution(recipe); //  The key line
    }
}
