using System.Collections.Generic;
using UnityEngine;

public class DepartmentCrateManager : MonoBehaviour
{
    [Header("Setup")]
    public GameObject cratePrefab;
    public List<Department> departments = new();
    public UniversalCrateSystem universalCrateSystem;

    private GridManager gridManager;

    void Awake()
    {
        gridManager = FindFirstObjectByType<GridManager>();
    }

    void Update()
    {
        if (universalCrateSystem != null)
            universalCrateSystem.UpdateUniversalCrateTimer(Time.deltaTime);
    }

    public void PurchaseDepartmentCrate(Department dept)
    {
        if (dept == null || EconomyManager.Instance == null || gridManager == null || cratePrefab == null)
            return;

        if (dept.crateCostSoft > 0 && !EconomyManager.Instance.Spend(CurrencyType.Money, dept.crateCostSoft))
            return;
        if (dept.crateCostPremium > 0 && !EconomyManager.Instance.Spend(CurrencyType.Gems, dept.crateCostPremium))
        {
            if (dept.crateCostSoft > 0)
                EconomyManager.Instance.Add(CurrencyType.Money, dept.crateCostSoft);
            return;
        }

        Vector2Int? freeCell = gridManager.GetRandomFreeCell();
        if (freeCell == null)
        {
            Debug.LogWarning("No free grid cell available for department crate.");
            if (dept.crateCostSoft > 0) EconomyManager.Instance.Add(CurrencyType.Money, dept.crateCostSoft);
            if (dept.crateCostPremium > 0) EconomyManager.Instance.Add(CurrencyType.Gems, dept.crateCostPremium);
            return;
        }

        DepartmentItemData item = GetRandomItem(dept);
        if (item == null)
        {
            Debug.LogWarning("Failed to find item for department crate.");
            if (dept.crateCostSoft > 0) EconomyManager.Instance.Add(CurrencyType.Money, dept.crateCostSoft);
            if (dept.crateCostPremium > 0) EconomyManager.Instance.Add(CurrencyType.Gems, dept.crateCostPremium);
            return;
        }

        Vector3 worldPos = gridManager.GetWorldPosition(freeCell.Value);
        GameObject crateObj = Instantiate(cratePrefab, worldPos, Quaternion.identity, gridManager.tileParent);
        DepartmentCrateSpawner spawner = crateObj.GetComponent<DepartmentCrateSpawner>();
        if (spawner != null)
        {
            CrateData crate = ScriptableObject.CreateInstance<CrateData>();
            crate.department = dept.type;
            crate.icon = dept.crateVisual;
            crate.maxUses = 1;
            crate.possibleItems = new DepartmentItemData[] { item };
            spawner.crateData = crate;
            spawner.RefillCrate();
        }
    }

    DepartmentItemData GetRandomItem(Department dept)
    {
        DepartmentItemTier tier = GetWeightedTier(dept.dropWeights);
        var allItems = Resources.LoadAll<DepartmentItemData>("DepartmentItems");
        List<DepartmentItemData> matches = new();
        foreach (var data in allItems)
        {
            if (data.department == dept.type && data.tier == tier)
                matches.Add(data);
        }
        if (matches.Count == 0) return null;
        return matches[Random.Range(0, matches.Count)];
    }

    DepartmentItemTier GetWeightedTier(List<Department.TierWeight> weights)
    {
        if (weights == null || weights.Count == 0)
            return DepartmentItemTier.Tier1;

        float total = 0f;
        foreach (var w in weights)
            total += Mathf.Max(0f, w.weight);

        float roll = Random.Range(0f, total);
        foreach (var w in weights)
        {
            roll -= Mathf.Max(0f, w.weight);
            if (roll <= 0f)
                return (DepartmentItemTier)w.tier;
        }
        return DepartmentItemTier.Tier1;
    }

    public void UpgradeDepartment(Department dept)
    {
        if (dept == null)
            return;

        dept.currentEra++;
        dept.crateCostSoft = Mathf.RoundToInt(dept.crateCostSoft * 1.5f);
        dept.crateCostPremium = Mathf.RoundToInt(dept.crateCostPremium * 1.5f);

        for (int i = 0; i < dept.dropWeights.Count; i++)
        {
            var tw = dept.dropWeights[i];
            tw.weight *= 1.1f;
            dept.dropWeights[i] = tw;
        }
    }
}
