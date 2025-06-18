using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages grid snapping and tile registration
/// </summary>
public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int columns = 9;
    public int rows = 16;
    public float tileSpacing = 1.1f; // distance between tiles
    public Vector2 startPos = Vector2.zero;
    public GameObject visualGridTile;
    public Transform tileParent;

    // Internal dictionary
    private Dictionary<Vector2Int, IGridOccupant> tileDictionary = new();

    void Start()
    {
        GenerateVisualGrid();

        // Optional: center grid in the world
        float gridWidth = (columns - 1) * tileSpacing;
        float gridHeight = (rows - 1) * tileSpacing;
        tileParent.position = new Vector3(-gridWidth / 2f, -gridHeight / 2f, 0f);
        visualGridTile.transform.position = tileParent.position; // Sync the visual parent

    }

    private void GenerateVisualGrid()
    {
        // Clear existing visual tiles first
        foreach (Transform child in tileParent)
        {
            Destroy(child.gameObject);
        }

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                Vector3 worldPos = GetWorldPosition(gridPos);

                GameObject tileVisual = Instantiate(visualGridTile, worldPos, Quaternion.identity, tileParent);
                tileVisual.name = $"VisualTile ({x},{y})";
            }
        }
    }

    void Awake()
    {
        tileDictionary = new Dictionary<Vector2Int, IGridOccupant>();
    }

    /// <summary>
    /// Converts grid coordinates to world position
    /// </summary>
    public Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        Vector3 worldPos = (Vector3)startPos + new Vector3(gridPos.x * tileSpacing, gridPos.y * tileSpacing, 0f);
        return tileParent.position + worldPos;
    }

    /// <summary>
    /// Converts world position to nearest grid cell
    /// </summary>
    public Vector2Int GetNearestGridCell(Vector3 worldPos)
    {
        Vector2 localPos = worldPos - (Vector3)tileParent.position;
        int x = Mathf.RoundToInt((localPos.x - startPos.x) / tileSpacing);
        int y = Mathf.RoundToInt((localPos.y - startPos.y) / tileSpacing);

        return new Vector2Int(
            Mathf.Clamp(x, 0, columns - 1),
            Mathf.Clamp(y, 0, rows - 1)
        );
    }

    /// <summary>
    /// Registers an occupant to a grid cell
    /// </summary>
    public void RegisterTile(Vector2Int gridPos, IGridOccupant occupant)
    {
        tileDictionary[gridPos] = occupant;
    }

    /// <summary>
    /// Unregisters a grid cell
    /// </summary>
    public void UnregisterTile(Vector2Int gridPos)
    {
        if (tileDictionary.ContainsKey(gridPos))
            tileDictionary.Remove(gridPos);
    }

    /// <summary>
    /// Gets the occupant at a given grid cell
    /// </summary>
    public IGridOccupant GetTileAt(Vector2Int gridPos)
    {
        tileDictionary.TryGetValue(gridPos, out IGridOccupant occupant);
        return occupant;
    }

    /// <summary>
    /// Finds a random empty grid cell
    /// </summary>
    public Vector2Int? GetRandomFreeCell()
    {
        List<Vector2Int> freeCells = new();

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (!tileDictionary.ContainsKey(pos))
                    freeCells.Add(pos);
            }
        }

        if (freeCells.Count == 0)
            return null;

        return freeCells[UnityEngine.Random.Range(0, freeCells.Count)];
    }

    /// <summary>
    /// Checks if a grid cell is occupied
    /// </summary>
    public bool IsOccupied(Vector2Int gridPos)
    {
        return tileDictionary.ContainsKey(gridPos);
    }
}
