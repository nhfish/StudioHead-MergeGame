using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple building that launches the dailies puzzle when a pending attempt is available.
/// </summary>
public class ScreeningRoom : MonoBehaviour
{
    [Header("References")]
    public Button puzzleButton;
    public DailiesManager dailiesManager;

    void Awake()
    {
        if (puzzleButton != null)
            puzzleButton.onClick.AddListener(HandleButton);
    }

    void Update()
    {
        if (puzzleButton != null)
            puzzleButton.gameObject.SetActive(dailiesManager != null && dailiesManager.GetRecipeWithPendingDaily() != null);
    }

    private void HandleButton()
    {
        if (dailiesManager == null)
            return;
        MovieRecipe recipe = dailiesManager.GetRecipeWithPendingDaily();
        if (recipe != null)
            dailiesManager.LaunchPuzzle(recipe);
    }
}
