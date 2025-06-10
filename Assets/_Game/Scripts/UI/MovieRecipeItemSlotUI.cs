using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MovieRecipeItemSlotUI : MonoBehaviour, IDropHandler, IPointerClickHandler
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

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        var dragUI = eventData.pointerDrag.GetComponent<DepartmentItemDragUI>();
        if (dragUI == null)
            return;

        SetItem(dragUI.ItemData);
        dragUI.Consume();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        ReturnItemToInventory();
    }

    /// <summary>
    /// Returns the assigned item back to the overflow inventory and clears the slot.
    /// </summary>
    public void ReturnItemToInventory()
    {
        if (AssignedItem == null)
            return;

        var mgr = InventoryOverflowManager.Instance;
        if (mgr != null)
            mgr.Store(new Item(AssignedItem));

        ClearItem();
    }

    public void ClearItem()
    {
        AssignedItem = null;
        if (iconImage != null)
            iconImage.sprite = DepartmentItemLibraryInstance.GetIcon(Department);
    }
}
