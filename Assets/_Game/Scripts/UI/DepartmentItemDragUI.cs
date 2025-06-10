using UnityEngine;
using UnityEngine.EventSystems;

public class DepartmentItemDragUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public DepartmentItemData itemData;

    private CanvasGroup canvasGroup;
    private Transform originalParent;

    public DepartmentItemData ItemData => itemData;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root, false);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent, false);
        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Optional helper when the dragged item is consumed by a drop target.
    /// </summary>
    public void Consume()
    {
        Destroy(gameObject);
    }
}
