using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class MergeTile : MonoBehaviour, IGridOccupant
{
    public DepartmentItemData data;
    private Vector3 originalPosition;
    private bool isDragging;
    private GridManager gridManager;
    private SpriteRenderer spriteRenderer;
    private Vector2Int currentGridPos;

    [SerializeField] private UnityEngine.UI.Image iconImage;
    [SerializeField] private TextMeshProUGUI tierLabel;
    [SerializeField] private UnityEngine.UI.Image background;

    void Awake()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (gridManager == null)
        {
            UnityEngine.Debug.LogError("GridManager not found!");
        }
    }

    void Start()
    {
        ApplyVisuals();

        currentGridPos = gridManager.GetNearestGridCell(transform.position);
        transform.position = gridManager.GetWorldPosition(currentGridPos);
        gridManager.RegisterTile(currentGridPos, this);
    }

    void OnDestroy()
    {
        if (gridManager != null)
            gridManager.UnregisterTile(currentGridPos);
    }

    void OnMouseDown()
    {
        originalPosition = transform.position;
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;

        Vector3 worldPos = transform.position;

        // Always force snap no matter what
        Vector2Int targetGridPos = gridManager.GetNearestGridCell(worldPos);
        Vector3 snapPos = gridManager.GetWorldPosition(targetGridPos);

        IGridOccupant occupant = gridManager.GetTileAt(targetGridPos);

        if (occupant is not null && !ReferenceEquals(occupant, this))
        {
            // If the occupant is a MergeTile � attempt merge
            MergeTile occupyingTile = occupant as MergeTile;

            if (occupyingTile != null)
            {
                bool merged = TryMergeWith(occupyingTile);
                if (!merged)
                {
                    // Revert to original position if merge failed
                    transform.position = gridManager.GetWorldPosition(currentGridPos);
                }
            }
            else
            {
                // Occupied by something else (e.g. Crate) cannot move into it revert
                transform.position = gridManager.GetWorldPosition(currentGridPos);
            }

            return;
        }

        // If cell is empty valid move update position
        gridManager.UnregisterTile(currentGridPos);

        transform.position = snapPos;
        currentGridPos = targetGridPos;

        gridManager.RegisterTile(currentGridPos, this);
    }

    private Vector3 ClampToScreen(Vector3 worldPos)
    {
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(worldPos);

        // Clamp between 0..1 in viewport space (screen edges)
        viewportPoint.x = Mathf.Clamp(viewportPoint.x, 0.05f, 0.95f); // Add margin if desired
        viewportPoint.y = Mathf.Clamp(viewportPoint.y, 0.05f, 0.95f);

        return Camera.main.ViewportToWorldPoint(viewportPoint);
    }

    void Update()
    {
        if (isDragging)
        {
#if UNITY_IOS || UNITY_ANDROID
            if (Input.touchCount > 0)
            {
                Vector3 touchPosition = Input.GetTouch(0).position;
                touchPosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
                Vector3 targetPos = Camera.main.ScreenToWorldPoint(touchPosition);

                // Clamp position
                targetPos = ClampToScreen(targetPos);

                transform.position = targetPos;
            }
#else
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(mousePosition);

            // Clamp position
            targetPos = ClampToScreen(targetPos);

            transform.position = targetPos;
#endif
        }
    }

    private Vector2Int GetNearestGridCell(Vector3 worldPos)
    {
        Vector2 localPos = worldPos - (Vector3)gridManager.tileParent.position;
        int x = Mathf.RoundToInt((localPos.x - gridManager.startPos.x) / gridManager.tileSpacing);
        int y = Mathf.RoundToInt((localPos.y - gridManager.startPos.y) / gridManager.tileSpacing);

        return new Vector2Int(
            Mathf.Clamp(x, 0, gridManager.columns - 1),
            Mathf.Clamp(y, 0, gridManager.rows - 1)
        );
    }

    private bool TryMergeWith(MergeTile other)
    {
        if (data == null || other.data == null) return false;

        if (!data.IsMergeableWith(other.data))
        {
            UnityEngine.Debug.Log("Merge failed � not compatible");
            return false;
        }

        Vector3 spawnPos = transform.position;

        // Create new tile
        GameObject newTile = Instantiate(gameObject, spawnPos, Quaternion.identity, transform.parent);
        MergeTile newMergeTile = newTile.GetComponent<MergeTile>();
        newMergeTile.data = data.nextTierItem;
        newMergeTile.ApplyVisuals();

        Vector2Int newGridPos = gridManager.GetNearestGridCell(spawnPos);
        newMergeTile.currentGridPos = newGridPos;
        gridManager.RegisterTile(newGridPos, newMergeTile);

        // Cleanup
        gridManager.UnregisterTile(currentGridPos);
        gridManager.UnregisterTile(other.currentGridPos);

        Destroy(other.gameObject);
        Destroy(this.gameObject);

        return true;
    }

    public void ApplyVisuals()
    {
        if (iconImage != null && data != null)
            iconImage.sprite = DepartmentItemLibraryInstance.GetIcon(data.department);

        if (tierLabel != null && data != null)
            tierLabel.text = data.tier.ToString();

        if (background != null && data != null)
            background.color = GetTierColor(data.tier);
    }

    private Color GetTierColor(DepartmentItemTier tier)
    {
        Color[] tierColors = new Color[]
        {
            new Color(0.7f, 0.7f, 0.7f),  // Gray
            Color.white,                  // White
            Color.yellow,                 // Yellow
            Color.green,                  // Green
            new Color(0f, 0f, 0.5f),      // Dark Blue
            new Color(0.5f, 0f, 0.5f),    // Purple
            new Color(1f, 0.5f, 0f),      // Orange
            Color.red,                    // Red
            new Color(0.5f, 0.8f, 1f),    // Light Blue
            new Color(1f, 0.84f, 0f)      // Gold
        };

        int tierIndex = Mathf.Clamp((int)tier, 0, tierColors.Length - 1);
        return tierColors[tierIndex];
    }

    private void SnapToCurrentGridCell()
    {
        Vector3 snapPos = gridManager.GetWorldPosition(currentGridPos);
        transform.position = snapPos;
    }

    // For crates allows them to tell the grid what position they occupy
    public void SetCurrentGridPosition(Vector2Int newPos)
    {
        currentGridPos = newPos;
    }

    // Public property to get current grid position for save system
    public Vector2Int CurrentGridPosition => currentGridPos;
}
