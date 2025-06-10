using UnityEngine;
using UnityEngine.UI;

public class MovieRecipeItemSlotUI : MonoBehaviour
{
    public Image iconImage;

    public DepartmentType Department { get; private set; }
    public DepartmentItemData AssignedItem { get; private set; }

    public void SetDepartment(DepartmentType dept)
    {
        Department = dept;
        if (iconImage != null)
            iconImage.sprite = DepartmentItemLibraryInstance.GetIcon(dept);
    }

    public void SetItem(DepartmentItemData item)
    {
        AssignedItem = item;
        if (iconImage != null)
            iconImage.sprite = item != null ? item.icon : DepartmentItemLibraryInstance.GetIcon(Department);
    }

    public void ClearItem()
    {
        AssignedItem = null;
        if (iconImage != null)
            iconImage.sprite = DepartmentItemLibraryInstance.GetIcon(Department);
    }
}
