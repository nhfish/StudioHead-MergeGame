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
        Color lowTierColor = new Color(0.8f, 0.8f, 0.8f);  // Gray for Tier 1
        Color highTierColor = new Color(1f, 0.84f, 0f);     // Gold for Tier 10

        int tierValue = (int)tier; // Should be 0-9

        float t = Mathf.Clamp01(tierValue / 9f);

        return Color.Lerp(lowTierColor, highTierColor, t);
    }

    public DepartmentItemData GetData()
    {
        return itemData;
    }
}
