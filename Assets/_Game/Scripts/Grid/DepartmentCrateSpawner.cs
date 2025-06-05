using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DepartmentCrateSpawner : MonoBehaviour
{
    [Header("Crate Data")]
    public CrateData crateData;

    [Header("Visuals")]
    public Image iconImage;
    public Image backgroundImage;
    public TextMeshProUGUI useLabel;

    private GridManager gridManager;
    private int usesRemaining;
    private Vector2Int currentGridPos;

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        usesRemaining = crateData.maxUses;
        currentGridPos = gridManager.GetNearestGridCell(transform.position);
        transform.position = gridManager.GetWorldPosition(currentGridPos);
        gridManager.RegisterTile(currentGridPos, null);

        UpdateVisuals();
    }
    void OnDestroy()
    {
        if (gridManager != null)
            gridManager.UnregisterTile(currentGridPos);
    }


    void OnMouseDown()
    {
        TrySpawnItem();
    }

    private void TrySpawnItem()
    {
        if (usesRemaining <= 0)
        {
            Debug.Log("Crate is empty.");
            return;
        }

        Vector2Int? emptyCell = gridManager.GetRandomFreeCell();

        if (emptyCell == null)
        {
            Debug.LogWarning("No free grid cell available.");
            return;
        }

        DepartmentItemData itemData = GetRandomItem();
        if (itemData == null)
        {
            Debug.LogError("Item spawn failed — itemData was null.");
            return;
        }

        SpawnTile(itemData, emptyCell.Value);

        usesRemaining--;
        UpdateVisuals();

        if (usesRemaining == 0)
        {
            // Optional: Animate depletion, disable collider, prompt refill, etc.
            Debug.Log("Crate depleted.");
        }
    }

    private DepartmentItemData GetRandomItem()
    {
        var items = crateData.possibleItems;
        if (items == null || items.Length == 0)
        {
            Debug.LogWarning("No possibleItems assigned to CrateData.");
            return null;
        }

        return items[Random.Range(0, items.Length)];
    }

    private void SpawnTile(DepartmentItemData itemData, Vector2Int gridPos)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/MergeTile");
        if (prefab == null)
        {
            Debug.LogError("MergeTile prefab not found in Resources/Prefabs!");
            return;
        }

        Vector3 worldPos = gridManager.GetWorldPosition(gridPos);
        GameObject newTile = Instantiate(prefab, worldPos, Quaternion.identity, gridManager.tileParent);

        MergeTile mergeTile = newTile.GetComponent<MergeTile>();
        mergeTile.data = itemData;
        mergeTile.ApplyVisuals();
        mergeTile.SetCurrentGridPosition(gridPos); // Add this method if you haven’t

        gridManager.RegisterTile(gridPos, mergeTile);
    }

    private void UpdateVisuals()
    {
        if (iconImage) iconImage.sprite = crateData.icon;
        if (backgroundImage) backgroundImage.color = usesRemaining > 0 ? crateData.crateColor : Color.gray;
        if (useLabel) useLabel.text = usesRemaining.ToString();
    }

    public void RefillCrate()
    {
        usesRemaining = crateData.maxUses;
        UpdateVisuals();
    }
}
