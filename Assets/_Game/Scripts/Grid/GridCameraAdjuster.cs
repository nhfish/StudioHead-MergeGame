using UnityEngine;

/// <summary>
/// Automatically adjusts camera orthographic size to properly frame the grid
/// </summary>
public class GridCameraAdjuster : MonoBehaviour
{
    [Header("Grid Reference")]
    public GridManager gridManager;
    
    [Header("Camera Settings")]
    public Camera targetCamera;
    public float margin = 1f; // Extra space around the grid
    
    [Header("Aspect Ratio Settings")]
    public bool autoAdjustForAspectRatio = true;
    public float targetAspectRatio = 9f/16f; // 9:16 portrait
    
    void Start()
    {
        if (gridManager == null)
            gridManager = FindFirstObjectByType<GridManager>();
            
        if (targetCamera == null)
            targetCamera = Camera.main;
            
        AdjustCamera();
    }
    
    void AdjustCamera()
    {
        if (gridManager == null || targetCamera == null)
            return;
            
        // Calculate grid dimensions
        float gridWidth = (gridManager.columns - 1) * gridManager.tileSpacing;
        float gridHeight = (gridManager.rows - 1) * gridManager.tileSpacing;
        
        // Calculate required orthographic size
        float requiredHeight = gridHeight / 2f + margin;
        float requiredWidth = gridWidth / 2f + margin;
        
        if (autoAdjustForAspectRatio)
        {
            // Get current screen aspect ratio
            float screenAspect = (float)Screen.width / Screen.height;
            
            // Calculate orthographic size based on aspect ratio
            float orthographicSize;
            
            if (screenAspect < targetAspectRatio)
            {
                // Screen is wider than target - fit to width
                orthographicSize = requiredWidth / screenAspect;
            }
            else
            {
                // Screen is taller than target - fit to height
                orthographicSize = requiredHeight;
            }
            
            targetCamera.orthographicSize = orthographicSize;
            
            Debug.Log($"Grid: {gridManager.columns}x{gridManager.rows} ({gridWidth:F1}x{gridHeight:F1})");
            Debug.Log($"Screen: {Screen.width}x{Screen.height} (aspect: {screenAspect:F3})");
            Debug.Log($"Camera orthographic size: {orthographicSize:F2}");
        }
        else
        {
            // Simple height-based adjustment
            targetCamera.orthographicSize = requiredHeight;
        }
    }
    
    // Call this method to manually adjust the camera
    [ContextMenu("Adjust Camera")]
    public void ManualAdjust()
    {
        AdjustCamera();
    }
    
    // Call this when screen orientation changes
    void OnRectTransformDimensionsChange()
    {
        AdjustCamera();
    }
} 