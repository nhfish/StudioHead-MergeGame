using UnityEngine;

public class MergeTile : MonoBehaviour
{
    public DepartmentItemData data;

    private Vector3 originalPosition;
    private Vector3 dragOffset;
    private bool isDragging;

    private Vector2Int currentGridPos;
    private GridManager gridManager;
    private SpriteRenderer spriteRenderer;

    // Smooth snap animation
    private Vector3 targetPosition;
    private bool isSnapping = false;
    [SerializeField] private float snapSpeed = 10f;

    void Awake()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (gridManager == null)
            Debug.LogError(" GridManager not found in scene.");
    }

    void Start()
    {
        ApplyVisuals();

        currentGridPos = gridManager.GetNearestGridCell(transform.position);
        transform.position = gridManager.GetWorldPosition(currentGridPos);
        gridManager.RegisterTile(currentGridPos, this);
    }

    void OnMouseDown()
{
    originalPosition = transform.position;
    isDragging = true;
    isSnapping = false;

    Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mouseWorld.z = transform.position.z;

    dragOffset = transform.position - mouseWorld;
}

   void OnMouseUp()
{
    isDragging = false;

    Vector3 worldPos = transform.position;
    Vector2Int targetGridPos = gridManager.GetNearestGridCell(worldPos);

    // Clamp to valid grid bounds
    targetGridPos.x = Mathf.Clamp(targetGridPos.x, 0, gridManager.columns - 1);
    targetGridPos.y = Mathf.Clamp(targetGridPos.y, 0, gridManager.rows - 1);

    Vector3 snapPos = gridManager.GetWorldPosition(targetGridPos);

    // Force snap back if drag was too small (tiny twitch move)
    if (Vector3.Distance(transform.position, originalPosition) < 0.2f)
    {
        SnapTo(gridManager.GetWorldPosition(currentGridPos));
        return;
    }

    MergeTile occupyingTile = gridManager.GetTileAt(targetGridPos);

    if (occupyingTile != null && occupyingTile != this)
    {
        bool merged = TryMergeWith(occupyingTile);
        if (!merged)
        {
            // Merge failed — snap back to where it came from
            SnapTo(gridManager.GetWorldPosition(currentGridPos));
        }

        return;
    }

    if (occupyingTile == null)
    {
        // Only unregister if actually moving to a new cell
        if (targetGridPos != currentGridPos)
            gridManager.UnregisterTile(currentGridPos);

        currentGridPos = targetGridPos;
        gridManager.RegisterTile(currentGridPos, this);

        // Always snap to aligned position
        SnapTo(snapPos);
    }
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
                transform.position = Camera.main.ScreenToWorldPoint(touchPosition) + dragOffset;
            }
#else
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;

            mousePosition.x = Mathf.Clamp(mousePosition.x, 0, Screen.width);
            mousePosition.y = Mathf.Clamp(mousePosition.y, 0, Screen.height);

            transform.position = Camera.main.ScreenToWorldPoint(mousePosition) + dragOffset;
#endif
        }

        // Smooth movement to snapped position
        if (isSnapping)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * snapSpeed);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isSnapping = false;
            }
        }
    }

    private void SnapTo(Vector3 position)
    {
        targetPosition = position;
        isSnapping = true;
    }

    private bool TryMergeWith(MergeTile other)
    {
        if (data == null || other.data == null) return false;

        if (!data.IsMergeableWith(other.data))
        {
            Debug.Log(" Merge failed — incompatible tiles");
            return false;
        }

        Vector3 spawnPos = transform.position;

        GameObject newTile = Instantiate(gameObject, spawnPos, Quaternion.identity, transform.parent);
        MergeTile newMergeTile = newTile.GetComponent<MergeTile>();
        newMergeTile.data = data.nextTierItem;
        newMergeTile.ApplyVisuals();

        Vector2Int newGridPos = gridManager.GetNearestGridCell(spawnPos);
        newMergeTile.currentGridPos = newGridPos;
        gridManager.RegisterTile(newGridPos, newMergeTile);

        gridManager.UnregisterTile(currentGridPos);
        gridManager.UnregisterTile(other.currentGridPos);

        Destroy(other.gameObject);
        Destroy(this.gameObject);

        return true;
    }

    public void ApplyVisuals()
    {
        if (data != null)
        {
            if (data.icon != null)
                spriteRenderer.sprite = data.icon;

            spriteRenderer.color = data.tileColor;
        }
    }

    public void SetCurrentGridPosition(Vector2Int pos)
{
    currentGridPos = pos;
}

}
