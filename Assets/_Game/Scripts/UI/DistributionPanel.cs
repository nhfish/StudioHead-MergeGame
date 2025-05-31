using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class DistributionPanel : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelRoot;
    public TextMeshProUGUI movieTitleText;
    public Button flatPayoutButton;
    public Button theatricalReleaseButton;

    private MovieRecipe currentRecipe;

    public void OpenWithRecipe(MovieRecipe recipe)
    {
        currentRecipe = recipe;
        panelRoot.SetActive(true);

        // Example text — customize with your actual recipe data
        movieTitleText.text = $" {GetMovieTitleFromRecipe(recipe)} is ready for distribution!";
        
        flatPayoutButton.onClick.RemoveAllListeners();
        flatPayoutButton.onClick.AddListener(() => HandleFlatPayout());

        theatricalReleaseButton.onClick.RemoveAllListeners();
        theatricalReleaseButton.onClick.AddListener(() => HandleTheatricalRelease());
    }

    private void HandleFlatPayout()
    {
        Debug.Log(" Flat payout selected!");
        // TODO: Give immediate money/fans rewards
        ClosePanel();
    }

    private void HandleTheatricalRelease()
    {
        Debug.Log(" Theatrical release selected!");
        // TODO: Start long-term release, tie to RCU system
        ClosePanel();
    }

    public UnityAction OnDistributionComplete;

public void ClosePanel()
{
    panelRoot.SetActive(false);
    OnDistributionComplete?.Invoke();
}

public void SetQueueCount(int count)
{
    // Optional: display count somewhere on screen
    Debug.Log($" Movies remaining in queue: {count}");
}


    private string GetMovieTitleFromRecipe(MovieRecipe recipe)
    {
        // Optional: return custom name, genre, or combination
        return $"Untitled {recipe.writer?.baseData.genre} Film";
    }
}
