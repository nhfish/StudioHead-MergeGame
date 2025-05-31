using UnityEngine;

[CreateAssetMenu(fileName = "NewCrateData", menuName = "Studio/Department Crate")]
public class CrateData : ScriptableObject
{
    public DepartmentType department;
    public Sprite icon;
    public Color crateColor = Color.white;
    public int maxUses = 5;

    public DepartmentItemData[] possibleItems; // Used to randomly select one when spawning
}
