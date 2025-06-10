using UnityEngine;

public class ProductionSystemLinker : MonoBehaviour
{
    public ProductionManager productionManager;
    public DailiesManager dailiesManager;

    void Start()
    {
        if (dailiesManager != null)
            dailiesManager.Initialize(productionManager);
    }
}
