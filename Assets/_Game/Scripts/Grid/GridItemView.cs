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
        // Simple gradient from gray (Tier1) to gold (Tier10)
        var gray = new Color(0.8f, 0.8f, 0.8f);
        var gold = new Color(1f, 0.8f, 0.2f);
        float t = Mathf.Clamp01((int)tier / 9f);
        return Color.Lerp(gray, gold, t);
    }

    public DepartmentItemData GetData()
    {
        return itemData;
    }
}
