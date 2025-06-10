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
        if (RewardManager.Instance != null && currentRecipe != null)
        {
            int money = Mathf.RoundToInt(currentRecipe.moneyReward * currentRecipe.rewardMultiplier);
            int fans = Mathf.RoundToInt(currentRecipe.fanReward * currentRecipe.rewardMultiplier);
            RewardManager.Instance.GrantRewards(money, fans);
        }
        ClosePanel();
    }

    private void HandleTheatricalRelease()
    {
        Debug.Log(" Theatrical release selected!");
        if (RewardManager.Instance != null && currentRecipe != null)
        {
            int money = Mathf.RoundToInt(currentRecipe.moneyReward * currentRecipe.rewardMultiplier);
            int fans = Mathf.RoundToInt(currentRecipe.fanReward * currentRecipe.rewardMultiplier);
            StartCoroutine(TheatricalPayoutRoutine(money, fans));
        }
        ClosePanel();
    }

    private System.Collections.IEnumerator TheatricalPayoutRoutine(int totalMoney, int totalFans)
    {
        float duration = 5f; // simple placeholder duration
        float elapsed = 0f;
        int moneyPaid = 0;
        int fansPaid = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);

            int targetMoney = Mathf.RoundToInt(totalMoney * progress);
            int targetFans = Mathf.RoundToInt(totalFans * progress);

            int deltaMoney = targetMoney - moneyPaid;
            int deltaFans = targetFans - fansPaid;

            if (deltaMoney > 0 || deltaFans > 0)
            {
                RewardManager.Instance.GrantRewards(deltaMoney, deltaFans);
                moneyPaid = targetMoney;
                fansPaid = targetFans;
            }

            yield return null;
        }
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
