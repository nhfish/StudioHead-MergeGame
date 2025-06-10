using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

/// Department Crate that spawns Department Items onto the grid
public class DepartmentCrateSpawner : MonoBehaviour, IGridOccupant
{
    [Header("Prefabs")]
    public GameObject mergeTilePrefab;

    [Header("Crate Data")]
    public CrateData crateData;

    [Header("Visuals")]
    public UnityEngine.UI.Image iconImage;
    public UnityEngine.UI.Image backgroundImage;
    public TextMeshProUGUI useLabel;

    private GridManager gridManager;
    private int usesRemaining;
    private Vector2Int currentGridPos;

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        usesRemaining = crateData.maxUses;

        // Snap to grid and register as occupant
        currentGridPos = gridManager.GetNearestGridCell(transform.position);
        transform.position = gridManager.GetWorldPosition(currentGridPos);
        gridManager.RegisterTile(currentGridPos, this);

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
            UnityEngine.Debug.Log("Crate is empty.");
            return;
        }

        DepartmentItemData itemData = GetRandomItem();
        if (itemData == null)
        {
            UnityEngine.Debug.LogError("Item spawn failed — itemData was null.");
            return;
        }

        Vector2Int? emptyCell = gridManager.GetRandomFreeCell();

        // Avoid spawning into the crate's own cell
        int attempts = 0;
        const int maxAttempts = 50;
        while (emptyCell != null && emptyCell.Value == currentGridPos && attempts < maxAttempts)
        {
            emptyCell = gridManager.GetRandomFreeCell();
            attempts++;
        }

        if (emptyCell == null)
        {
            var mgr = InventoryOverflowManager.Instance;
            if (mgr != null)
                mgr.Store(new Item(itemData));
            else
                UnityEngine.Debug.LogWarning("No free grid cell and OverflowManager missing.");

            usesRemaining--;
            UpdateVisuals();

            if (usesRemaining == 0)
            {
                StartCoroutine(FadeAndDestroy());
            }
            return;
        }

        if (emptyCell.Value == currentGridPos)
        {
            UnityEngine.Debug.LogWarning("No free grid cell available.");
            return;
        }

        SpawnTile(itemData, emptyCell.Value);

        usesRemaining--;
        UpdateVisuals();

        if (usesRemaining == 0)
        {
            // Animate depletion and destroy self
            StartCoroutine(FadeAndDestroy());
        }
    }

    private DepartmentItemData GetRandomItem()
    {
        var items = crateData.possibleItems;
        if (items == null || items.Length == 0)
        {
            UnityEngine.Debug.LogWarning("No possibleItems assigned to CrateData.");
            return null;
        }

        return items[UnityEngine.Random.Range(0, items.Length)];
    }

    private void SpawnTile(DepartmentItemData itemData, Vector2Int gridPos)
    {
        if (mergeTilePrefab == null)
        {
            UnityEngine.Debug.LogError("MergeTilePrefab reference is not assigned in Inspector!");
            return;
        }

        Vector3 worldPos = gridManager.GetWorldPosition(gridPos);
        GameObject newTile = Instantiate(mergeTilePrefab, worldPos, Quaternion.identity, gridManager.tileParent);

        MergeTile mergeTile = newTile.GetComponent<MergeTile>();
        mergeTile.data = itemData;
        mergeTile.ApplyVisuals();
        mergeTile.SetCurrentGridPosition(gridPos);

        gridManager.RegisterTile(gridPos, mergeTile);
    }

    private void UpdateVisuals()
    {
        if (iconImage) iconImage.sprite = crateData.icon;
        if (backgroundImage) backgroundImage.color = usesRemaining > 0 ? crateData.crateColor : Color.gray;
        if (useLabel) useLabel.text = usesRemaining.ToString();
    }

    private System.Collections.IEnumerator FadeAndDestroy()
    {
        float duration = 0.25f;
        float t = 0;
        Vector3 startScale = transform.localScale;

        while (t < duration)
        {
            t += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 0f, t / duration);
            transform.localScale = startScale * scale;
            yield return null;
        }

        gridManager.UnregisterTile(currentGridPos); // Extra safety
        Destroy(gameObject);
    }

    public void RefillCrate()
    {
        usesRemaining = crateData.maxUses;
        UpdateVisuals();
    }
}
