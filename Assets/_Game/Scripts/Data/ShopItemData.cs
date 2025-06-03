using UnityEngine;

public enum ShopItemType { Crate, DepartmentUpgrade }

[CreateAssetMenu(menuName = "Studio/Shop Item")]
public class ShopItemData : ScriptableObject
{
    public string itemName;
    public ShopItemType itemType;
    public int price = 100;

    [Header("Crate")]
    public CrateData crate;

    [Header("Upgrade")]
    public DepartmentType upgradeDepartment;
}
