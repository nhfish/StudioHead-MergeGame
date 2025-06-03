using UnityEngine;
using TMPro;

public class CurrencyPanel : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI gemsText;
    public TextMeshProUGUI ticketsText;

    void Start()
    {
        UpdateAll();
        if (EconomyManager.Instance != null)
            EconomyManager.Instance.CurrencyChanged += OnCurrencyChanged;
    }

    void OnDestroy()
    {
        if (EconomyManager.Instance != null)
            EconomyManager.Instance.CurrencyChanged -= OnCurrencyChanged;
    }

    private void OnCurrencyChanged(CurrencyType type, int amount)
    {
        switch (type)
        {
            case CurrencyType.Money:
                if (moneyText != null) moneyText.text = amount.ToString();
                break;
            case CurrencyType.Gems:
                if (gemsText != null) gemsText.text = amount.ToString();
                break;
            case CurrencyType.Tickets:
                if (ticketsText != null) ticketsText.text = amount.ToString();
                break;
        }
    }

    private void UpdateAll()
    {
        if (EconomyManager.Instance == null)
            return;

        OnCurrencyChanged(CurrencyType.Money, EconomyManager.Instance.GetAmount(CurrencyType.Money));
        OnCurrencyChanged(CurrencyType.Gems, EconomyManager.Instance.GetAmount(CurrencyType.Gems));
        OnCurrencyChanged(CurrencyType.Tickets, EconomyManager.Instance.GetAmount(CurrencyType.Tickets));
    }
}
