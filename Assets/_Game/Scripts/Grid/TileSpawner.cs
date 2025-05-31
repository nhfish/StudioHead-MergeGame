using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public GridManager gridManager;
    public GameObject[] tilePrefabs; // Prefabs for Tier 1 items
    public Vector2Int gridSize = new Vector2Int(6, 9);

    private HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>();

    void Start()
    {
        SpawnInitialTiles(10); // Spawn 10 tiles on start (you can change this)
    }

    public void SpawnInitialTiles(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnRandomTile();
        }
    }

    public void SpawnRandomTile()
    {
        // Try 50 times to find an empty spot
        for (int attempt = 0; attempt < 50; attempt++)
        {
            Vector2Int randomPos = new Vector2Int(
                Random.Range(0, gridSize.x),
                Random.Range(0, gridSize.y)
            );

            if (!occupiedPositions.Contains(randomPos))
            {
                GameObject tilePrefab = tilePrefabs[Random.Range(0, tilePrefabs.Length)];
                GameObject tile = Instantiate(tilePrefab);
                tile.transform.position = gridManager.GetWorldPosition(randomPos);
                tile.transform.SetParent(gridManager.tileParent);
                occupiedPositions.Add(randomPos);
                return;
            }
        }

        Debug.LogWarning("No available positions to spawn a tile.");
    }

    public void ClearGrid()
    {
        foreach (Transform child in gridManager.tileParent)
        {
            Destroy(child.gameObject);
        }

        occupiedPositions.Clear();
    }
}
