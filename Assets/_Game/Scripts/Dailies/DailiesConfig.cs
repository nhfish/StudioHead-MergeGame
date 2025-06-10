using UnityEngine;

/// <summary>
/// Configuration for the dailies mini puzzle.
/// </summary>
[CreateAssetMenu(menuName = "Dailies/Config", fileName = "DailiesConfig")]
public class DailiesConfig : ScriptableObject
{
    [Header("Budget Settings")]
    public int startingBudget = 100;
    public int moveCost = 5;
    public int mergeSavings = 3;

    [Header("Scoring")]
    public int baseScore = 100;
}
