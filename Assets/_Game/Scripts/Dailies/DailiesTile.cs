using UnityEngine;

/// <summary>
/// Represents a single tile in the Dailies puzzle grid.
/// </summary>
public class DailiesTile : MonoBehaviour
{
    public int Level { get; private set; } = 1;

    /// <summary>
    /// Apply a new level to this tile.
    /// </summary>
    public void SetLevel(int level)
    {
        Level = Mathf.Max(1, level);
    }

    /// <summary>
    /// Upgrade this tile one level higher.
    /// </summary>
    public void Upgrade()
    {
        Level++;
    }
}
