using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class OverflowPanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI References")]
    public RectTransform panelRoot;
    public Button toggleButton;
    public Transform itemGridParent;
    public GameObject itemTilePrefab;
    public TextMeshProUGUI slotLabel;

    [Header("Slide Settings")]
    public float collapsedY = -300f;
    public float expandedY = 0f;
    public float slideSpeed = 10f;

    private bool isExpanded = false;
    private Vector2 dragStart;
    private Vector2 panelStart;

    private void Awake()
    {
        if (toggleButton != null)
            toggleButton.onClick.AddListener(Toggle);
    }

    private void Start()
    {
        if (panelRoot != null)
            panelRoot.anchoredPosition = new Vector2(panelRoot.anchoredPosition.x, collapsedY);

        RefreshUI();

        if (InventoryOverflowManager.Instance != null)
            InventoryOverflowManager.Instance.OverflowUpdated += OnOverflowChanged;
    }

    private void OnDestroy()
    {
        if (InventoryOverflowManager.Instance != null)
            InventoryOverflowManager.Instance.OverflowUpdated -= OnOverflowChanged;
    }

    private void Update()
    {
        if (panelRoot == null)
            return;
        float target = isExpanded ? expandedY : collapsedY;
        Vector2 pos = panelRoot.anchoredPosition;
        pos.y = Mathf.Lerp(pos.y, target, Time.unscaledDeltaTime * slideSpeed);
        panelRoot.anchoredPosition = pos;
    }

    public void Toggle()
    {
        isExpanded = !isExpanded;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (panelRoot == null)
            return;
        dragStart = eventData.position;
        panelStart = panelRoot.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (panelRoot == null)
            return;
        Vector2 delta = eventData.position - dragStart;
        Vector2 pos = panelStart + new Vector2(0, delta.y);
        pos.y = Mathf.Clamp(pos.y, collapsedY, expandedY);
        panelRoot.anchoredPosition = pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (panelRoot == null)
            return;
        float midpoint = (collapsedY + expandedY) * 0.5f;
        isExpanded = panelRoot.anchoredPosition.y > midpoint;
    }

    private void OnOverflowChanged()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        RefreshItems();
        UpdateSlotLabel();
    }

    private void RefreshItems()
    {
        if (itemGridParent == null || itemTilePrefab == null)
            return;
        foreach (Transform child in itemGridParent)
            Destroy(child.gameObject);

        var mgr = InventoryOverflowManager.Instance;
        if (mgr == null)
            return;

        IReadOnlyList<Item> items = mgr.StoredItems;
        foreach (var item in items)
        {
            GameObject go = Instantiate(itemTilePrefab, itemGridParent);
            Image img = go.GetComponentInChildren<Image>();
            if (img != null)
                img.sprite = item.visual;
        }
    }

    private void UpdateSlotLabel()
    {
        if (slotLabel == null)
            return;
        var mgr = InventoryOverflowManager.Instance;
        if (mgr == null)
            return;
        slotLabel.text = $"{mgr.SlotsUsed}/{mgr.MaxSlots}";
    }
}
