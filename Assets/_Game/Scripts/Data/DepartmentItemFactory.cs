using UnityEngine;
using System.Linq;


public static class DepartmentItemFactory
{
    /// <summary>
    /// Returns a random <see cref="DepartmentItemData"/> matching the given department and tier.
    /// </summary>
    public static DepartmentItemData Create(DepartmentType department, DepartmentItemTier tier)
    {
        var allItems = Resources.LoadAll<DepartmentItemData>("DepartmentItems");

        var matchingItems = allItems
            .Where(item => item.department == department && item.tier == tier)
            .ToList();

        if (matchingItems.Count == 0)
        {
            Debug.LogWarning($"No DepartmentItemData found for {department} tier {tier} in Resources/DepartmentItems");
            return null;
        }

        return matchingItems[Random.Range(0, matchingItems.Count)];
    }

    /// <summary>
    /// Convenience overload that always returns a tier 1 item for the department.
    /// </summary>
    public static DepartmentItemData Create(DepartmentType department)
    {
        return Create(department, DepartmentItemTier.Tier1);
    }
}
