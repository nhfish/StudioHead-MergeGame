using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages a small 4x4 merge board for the dailies mini game.
/// </summary>
public class DailiesBoardManager : MonoBehaviour
{
    [Header("References")]
    public DailiesTile tilePrefab;
    public DailiesConfig config;
    public RectTransform boardRoot;
    public TextMeshProUGUI budgetText;
    public TextMeshProUGUI scoreText;
    public Button skipButton;
    public Button confirmButton;

    public DailiesManager dailiesManager;
    public MovieRecipe currentRecipe;

    private DailiesTile[,] grid = new DailiesTile[4, 4];
    private int remainingBudget;
    private bool puzzleEnded;

    private readonly List<DailiesTile> spawnBuffer = new();

    void Start()
    {
        remainingBudget = config != null ? config.startingBudget : 100;
        UpdateBudgetUI();
        scoreText?.gameObject.SetActive(false);

        skipButton?.onClick.AddListener(SkipPuzzle);
        confirmButton?.onClick.AddListener(ConfirmPuzzle);
        confirmButton?.gameObject.SetActive(false);

        SpawnRandomTile();
        SpawnRandomTile();
    }

    void Update()
    {
        if (puzzleEnded)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) Move(Vector2Int.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) Move(Vector2Int.right);
        if (Input.GetKeyDown(KeyCode.UpArrow)) Move(Vector2Int.up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) Move(Vector2Int.down);
    }

    private void Move(Vector2Int dir)
    {
        bool moved = false;
        bool[,] merged = new bool[4, 4];

        var xs = dir.x > 0 ? new int[] { 3, 2, 1, 0 } : new int[] { 0, 1, 2, 3 };
        var ys = dir.y > 0 ? new int[] { 3, 2, 1, 0 } : new int[] { 0, 1, 2, 3 };

        foreach (int x in xs)
        {
            foreach (int y in ys)
            {
                var tile = grid[x, y];
                if (tile == null) continue;

                int nx = x;
                int ny = y;
                while (true)
                {
                    int tx = nx + dir.x;
                    int ty = ny + dir.y;
                    if (tx < 0 || tx >= 4 || ty < 0 || ty >= 4)
                        break;
                    if (grid[tx, ty] == null)
                    {
                        grid[tx, ty] = tile;
                        grid[nx, ny] = null;
                        nx = tx; ny = ty;
                        moved = true;
                        continue;
                    }
                    if (!merged[tx, ty] && grid[tx, ty].Level == tile.Level)
                    {
                        grid[tx, ty].Upgrade();
                        Destroy(tile.gameObject);
                        grid[nx, ny] = null;
                        merged[tx, ty] = true;
                        remainingBudget += config != null ? config.mergeSavings : 0;
                        moved = true;
                    }
                    break;
                }
                if (tile != null)
                {
                    tile.transform.position = GetCellPosition(nx, ny);
                }
            }
        }

        if (moved)
        {
            remainingBudget -= config != null ? config.moveCost : 1;
            SpawnRandomTile();
            UpdateBudgetUI();
            CheckEndConditions();
        }
    }

    private void SpawnRandomTile()
    {
        var empty = new List<Vector2Int>();
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                if (grid[x, y] == null)
                    empty.Add(new Vector2Int(x, y));

        if (empty.Count == 0) return;

        var cell = empty[Random.Range(0, empty.Count)];
        var tile = Instantiate(tilePrefab, boardRoot);
        tile.transform.localPosition = GetCellPosition(cell.x, cell.y);
        tile.SetLevel(Random.value < 0.9f ? 1 : 2);
        grid[cell.x, cell.y] = tile;
        spawnBuffer.Add(tile);
    }

    private Vector3 GetCellPosition(int x, int y)
    {
        if (boardRoot == null) return Vector3.zero;
        float size = boardRoot.rect.width / 4f;
        return new Vector3((x + 0.5f) * size - boardRoot.rect.width / 2f,
                           (y + 0.5f) * size - boardRoot.rect.height / 2f,
                           0f);
    }

    private void UpdateBudgetUI()
    {
        if (budgetText != null)
            budgetText.text = $"Budget: {remainingBudget}";
    }

    private void CheckEndConditions()
    {
        if (remainingBudget <= 0 || !CanMove())
        {
            EndPuzzle();
        }
    }

    private bool CanMove()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                var t = grid[x, y];
                if (t == null) return true;
                if (x < 3 && grid[x + 1, y] == null) return true;
                if (y < 3 && grid[x, y + 1] == null) return true;
                if (x < 3 && grid[x + 1, y] != null && grid[x + 1, y].Level == t.Level) return true;
                if (y < 3 && grid[x, y + 1] != null && grid[x, y + 1].Level == t.Level) return true;
            }
        }
        return false;
    }

    private int finalScore;
    private void EndPuzzle()
    {
        puzzleEnded = true;
        finalScore = Mathf.Max(0, remainingBudget);
        confirmButton?.gameObject.SetActive(true);
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(true);
            scoreText.text = $"Score: {finalScore}";
        }
    }

    private void SkipPuzzle()
    {
        if (dailiesManager != null && currentRecipe != null)
            dailiesManager.PlayOrSkipDaily(currentRecipe, -1);
    }

    private void ConfirmPuzzle()
    {
        if (dailiesManager != null && currentRecipe != null)
            dailiesManager.PlayOrSkipDaily(currentRecipe, finalScore);
    }
}
