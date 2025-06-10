using UnityEngine;

[CreateAssetMenu(fileName = "NewDepartmentItem", menuName = "Studio/Department Item")]
public class DepartmentItemData : ScriptableObject
{
    [Header("Core Info")]
    public DepartmentType department;
    public DepartmentItemTier tier;

    [Header("Visuals")]
    public string itemName;
    public Sprite icon;
    public Color tileColor = Color.white;

    [Header("Tier Evolution")]
    public DepartmentItemData nextTierItem;

    [Header("Production Settings")]
    [Range(0f, 2f)]
    public float rewardMultiplier = 1f;

    [Header("Economy")]
    public int baseValue = 0;

    public bool IsRequired =>
        department == DepartmentType.Camera ||
        department == DepartmentType.Sound ||
        department == DepartmentType.Production;

    public bool IsMergeableWith(DepartmentItemData other)
{
    if (other == null) return false;

    return this.department == other.department &&
           this.tier == other.tier &&
           this.nextTierItem != null;
}

}
