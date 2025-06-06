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

    /// <summary>
    /// Initializes the item view with item data.
    /// </summary>
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

    /// <summary>
    /// Optional helper: sets color based on tier
    /// </summary>
    private Color GetTierColor(DepartmentItemTier tier)
    {
        // Define gradient endpoints
        Color tier1Color = new Color(0.7f, 0.7f, 0.7f);  // Gray for Tier 1
        Color tier10Color = new Color(1f, 0.84f, 0f);     // Gold for Tier 10

        int tierValue = (int)tier;

        // Support enums that might start at 1 instead of 0
        if (tierValue >= 1 && tierValue <= 10)
            tierValue -= 1;

        // Clamp tier range to a 0-9 index
        tierValue = Mathf.Clamp(tierValue, 0, 9);

        float t = tierValue / 9f;
        return Color.Lerp(tier1Color, tier10Color, t);
    }

    public DepartmentItemData GetData()
    {
        return itemData;
    }
}
