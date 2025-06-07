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
    public Button streamingButton;
    public Button festivalButton;
    public Button filmMarketButton;

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

        streamingButton.onClick.RemoveAllListeners();
        streamingButton.onClick.AddListener(() => HandleStreamingRelease());
        streamingButton.interactable = false;

        festivalButton.onClick.RemoveAllListeners();
        festivalButton.onClick.AddListener(() => HandleFestivalRelease());
        festivalButton.interactable = false;

        filmMarketButton.onClick.RemoveAllListeners();
        filmMarketButton.onClick.AddListener(() => HandleFilmMarketRelease());
        filmMarketButton.interactable = false;
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

    private void HandleStreamingRelease()
    {
        Debug.Log(" Streaming release selected! (Not yet implemented)");
        ClosePanel();
    }

    private void HandleFestivalRelease()
    {
        Debug.Log(" Festival distribution selected! (Not yet implemented)");
        ClosePanel();
    }

    private void HandleFilmMarketRelease()
    {
        Debug.Log(" Film market distribution selected! (Not yet implemented)");
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
