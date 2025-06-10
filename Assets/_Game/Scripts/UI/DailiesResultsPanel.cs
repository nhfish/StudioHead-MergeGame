using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DailiesResultsPanel : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelRoot;
    public TextMeshProUGUI budgetText;
    public TextMeshProUGUI multiplierText;
    public Button continueButton;

    private UnityAction onContinue;

    private void Awake()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(HandleContinue);
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    public void Show(int remainingBudget, float multiplier, UnityAction onContinueAction)
    {
        onContinue = onContinueAction;
        if (budgetText != null)
            budgetText.text = $"Budget Left: {remainingBudget}";
        if (multiplierText != null)
            multiplierText.text = $"Multiplier +{multiplier:0.##}";
        if (panelRoot != null)
            panelRoot.SetActive(true);
    }

    private void HandleContinue()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
        onContinue?.Invoke();
    }
}
