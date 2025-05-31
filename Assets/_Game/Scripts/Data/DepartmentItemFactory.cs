using UnityEngine;
using System.Linq;


public static class DepartmentItemFactory
{
    public static DepartmentItemData Create(DepartmentType department)
    {
        var allItems = Resources.LoadAll<DepartmentItemData>("DepartmentItems");

        // Filter for the desired department & tier
        var matchingItems = allItems
            .Where(item => item.department == department && item.tier == DepartmentItemTier.Basic)
            .ToList();

        if (matchingItems.Count == 0)
        {
            Debug.LogWarning($"No DepartmentItemData found for {department} in Resources/DepartmentItems");
            return null;
        }

        return matchingItems[Random.Range(0, matchingItems.Count)];
    }
}
