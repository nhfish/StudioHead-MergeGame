using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns department crates on a timed interval using weighted tier chances.
/// </summary>
[Serializable]
public class DepartmentDropWeights
{
    public DepartmentType department;
    public List<float> tierWeights = new();
}

public class UniversalCrateSystem : MonoBehaviour
{
    [Header("Spawn Settings")]
    public float spawnIntervalHours = 4f;
    public float timeSinceLastSpawn = 0f;

    [Header("References")]
    public GameObject cratePrefab;
    public List<DepartmentDropWeights> departmentWeights = new();

    private GridManager gridManager;

    void Awake()
    {
        gridManager = FindFirstObjectByType<GridManager>();
    }

    /// <summary>
    /// Call each frame to advance the universal crate timer.
    /// </summary>
    public void UpdateUniversalCrateTimer(float deltaTime)
    {
        timeSinceLastSpawn += deltaTime / 3600f;
        if (timeSinceLastSpawn >= spawnIntervalHours)
        {
            SpawnUniversalCrateItem();
            timeSinceLastSpawn = 0f;
        }
    }

    /// <summary>
    /// Spawns a DepartmentCrateSpawner in a free grid cell.
    /// </summary>
    public void SpawnUniversalCrateItem()
    {
        if (cratePrefab == null || gridManager == null)
        {
            Debug.LogWarning("UniversalCrateSystem missing references.");
            return;
        }

        Vector2Int? freeCell = gridManager.GetRandomFreeCell();
        if (freeCell == null)
        {
            Debug.LogWarning("No free grid cell available for universal crate.");
            return;
        }

        DepartmentItemData item = GetWeightedItem();
        if (item == null)
        {
            Debug.LogWarning("Failed to get item for universal crate.");
            return;
        }

        Vector3 worldPos = gridManager.GetWorldPosition(freeCell.Value);
        GameObject crateObj = Instantiate(cratePrefab, worldPos, Quaternion.identity, gridManager.tileParent);

        DepartmentCrateSpawner spawner = crateObj.GetComponent<DepartmentCrateSpawner>();
        if (spawner != null)
        {
            CrateData crate = ScriptableObject.CreateInstance<CrateData>();
            crate.department = item.department;
            crate.maxUses = 1;
            crate.possibleItems = new DepartmentItemData[] { item };
            spawner.crateData = crate;
            spawner.RefillCrate();
        }
    }

    private DepartmentItemData GetWeightedItem()
    {
        if (departmentWeights.Count == 0)
            return null;

        DepartmentDropWeights chosenDept = departmentWeights[UnityEngine.Random.Range(0, departmentWeights.Count)];
        DepartmentItemTier tier = GetWeightedTier(chosenDept.tierWeights);

        // Try loading items from Resources/DepartmentItems
        var allItems = Resources.LoadAll<DepartmentItemData>("DepartmentItems");
        List<DepartmentItemData> matches = new();
        foreach (var item in allItems)
        {
            if (item.department == chosenDept.department && item.tier == tier)
                matches.Add(item);
        }

        if (matches.Count == 0)
            return null;

        return matches[UnityEngine.Random.Range(0, matches.Count)];
    }

    private DepartmentItemTier GetWeightedTier(List<float> weights)
    {
        if (weights == null || weights.Count == 0)
            return DepartmentItemTier.Tier1;

        float total = 0f;
        foreach (var w in weights)
            total += Mathf.Max(0f, w);

        float roll = UnityEngine.Random.Range(0f, total);
        for (int i = 0; i < weights.Count; i++)
        {
            roll -= Mathf.Max(0f, weights[i]);
            if (roll <= 0f)
                return (DepartmentItemTier)i;
        }

        return DepartmentItemTier.Tier1;
    }
}
