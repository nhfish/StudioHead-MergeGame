using UnityEngine;

public class GridItemSpawner : MonoBehaviour
{
    public static GridItemSpawner Instance;

    [Header("Prefab References")]
    public GameObject itemPrefab;

    [Header("Grid Logic")]
    public Transform gridParent; // Parent for placing items
    public float cellSize = 100f; // Or whatever your grid units are

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Spawns a new item at the specified world position.
    /// </summary>
    public void SpawnItemAt(Vector3 worldPos, DepartmentItemData itemData)
    {
        GameObject itemGO = Instantiate(itemPrefab, worldPos, Quaternion.identity, gridParent);
        var itemView = itemGO.GetComponent<GridItemView>();
        itemView.Initialize(itemData);
    }

    /// <summary>
    /// Spawns a new item near a given position (usually a generator).
    /// </summary>
    public void SpawnItemNear(Vector3 centerPos, DepartmentItemData itemData)
    {
        Vector3 targetPos = FindFreeSlotNear(centerPos);
        SpawnItemAt(targetPos, itemData);
    }

    /// <summary>
    /// Finds an open location near a source position (for now: return same spot).
    /// Later: check merge grid or valid nearby tiles.
    /// </summary>
    private Vector3 FindFreeSlotNear(Vector3 origin)
    {
        //  Later you can improve this with actual grid slot logic
        return origin;
    }
}
