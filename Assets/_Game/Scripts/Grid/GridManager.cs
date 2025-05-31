using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int columns = 5;
    public int rows = 7;
    public float tileSpacing = 1.2f;

    [Header("References")]
    public Transform tileParent;
    public GameObject visualTilePrefab;

    [HideInInspector] public Vector2 startPos;
    private Dictionary<Vector2Int, MergeTile> gridDictionary;

    void Start()
    {
        ClearGridVisuals();
        GenerateGrid();
        CenterCameraOnGrid();
    }

    private void ClearGridVisuals()
    {
        foreach (Transform child in tileParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void GenerateGrid()
    {
        gridDictionary = new Dictionary<Vector2Int, MergeTile>();

        // Center the grid around world origin (0,0)
        float gridWidth = columns * tileSpacing;
        float gridHeight = rows * tileSpacing;

        startPos = new Vector2(-gridWidth / 2f + tileSpacing / 2f, -gridHeight / 2f + tileSpacing / 2f);

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector2Int coord = new Vector2Int(x, y);
                Vector3 worldPos = GetWorldPosition(coord);

                // Create visual grid tile (non-interactive)
                Instantiate(visualTilePrefab, worldPos, Quaternion.identity, tileParent);
            }
        }
    }

    private void CenterCameraOnGrid()
    {
        float gridWidth = columns * tileSpacing;
        float gridHeight = rows * tileSpacing;

        float centerX = tileParent.position.x + startPos.x + (gridWidth - tileSpacing) / 2f;
        float centerY = tileParent.position.y + startPos.y + (gridHeight - tileSpacing) / 2f;

        Vector3 cameraPos = new Vector3(centerX, centerY, -10f);
        Camera.main.transform.position = cameraPos;

        // Zoom to fit the grid vertically
        Camera.main.orthographicSize = (gridHeight / 2f) + 1f;
    }

    public Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        return tileParent.position + (Vector3)startPos + new Vector3(gridPos.x * tileSpacing, gridPos.y * tileSpacing, 0);
    }

    public Vector2Int GetNearestGridCell(Vector3 worldPos)
    {
        
        Vector3 offset = worldPos - tileParent.position;
        Vector2 localPos = new Vector2(offset.x, offset.y) - startPos;

        int x = Mathf.RoundToInt(localPos.x / tileSpacing);
        int y = Mathf.RoundToInt(localPos.y / tileSpacing);

        return new Vector2Int(
            Mathf.Clamp(x, 0, columns - 1),
            Mathf.Clamp(y, 0, rows - 1)
        );
    }

    public void RegisterTile(Vector2Int gridPos, MergeTile tile)
    {
        gridDictionary[gridPos] = tile;
    }

    public void UnregisterTile(Vector2Int gridPos)
    {
        if (gridDictionary.ContainsKey(gridPos))
        {
            gridDictionary.Remove(gridPos);
        }
    }

    public MergeTile GetTileAt(Vector2Int gridPos)
    {
        gridDictionary.TryGetValue(gridPos, out MergeTile tile);
        return tile;
    }

    public Vector2Int? GetRandomFreeCell()
{
    List<Vector2Int> freeCells = new List<Vector2Int>();

    for (int x = 0; x < columns; x++)
    {
        for (int y = 0; y < rows; y++)
        {
            Vector2Int cell = new Vector2Int(x, y);
            if (!gridDictionary.ContainsKey(cell))
            {
                freeCells.Add(cell);
            }
        }
    }

    if (freeCells.Count == 0)
        return null;

    return freeCells[Random.Range(0, freeCells.Count)];
}

}
