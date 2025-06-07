using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridItemView : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI tierLabel;
    public Image background; // Optional: tier color

    private DepartmentItemData itemData;

    public void Initialize(DepartmentItemData data)
    {
        itemData = data;

        // Set icon from DepartmentItemLibrary
        iconImage.sprite = DepartmentItemLibraryInstance.GetIcon(data.department);

        // Set tier label
        tierLabel.text = data.tier.ToString();

        // Optional: Color background based on tier
        background.color = GetTierColor(data.tier);
    }

    private Color GetTierColor(DepartmentItemTier tier)
    {
        switch (tier)
        {
            case DepartmentItemTier.Tier1:
                return new Color(0.6f, 0.6f, 0.6f); // Grey
            case DepartmentItemTier.Tier2:
                return new Color(1f, 1f, 1f); // White
            case DepartmentItemTier.Tier3:
                return new Color(1f, 0.92f, 0.016f); // Yellow
            case DepartmentItemTier.Tier4:
                return new Color(0.2f, 0.8f, 0.2f); // Green
            case DepartmentItemTier.Tier5:
                return new Color(0.1f, 0.2f, 0.6f); // Dark Blue
            case DepartmentItemTier.Tier6:
                return new Color(0.6f, 0.2f, 0.7f); // Purple
            case DepartmentItemTier.Tier7:
                return new Color(1f, 0.5f, 0f); // Orange
            case DepartmentItemTier.Tier8:
                return new Color(0.9f, 0.1f, 0.1f); // Red
            case DepartmentItemTier.Tier9:
                return new Color(0.3f, 0.8f, 1f); // Light Blue
            case DepartmentItemTier.Tier10:
                return new Color(1f, 0.84f, 0f); // Gold
            default:
                return Color.white; // fallback
        }
    }

    public DepartmentItemData GetData()
    {
        return itemData;
    }
}
