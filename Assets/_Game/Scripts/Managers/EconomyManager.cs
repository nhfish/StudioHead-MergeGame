using UnityEngine;
using System.Collections.Generic;

public enum CurrencyType { Money, Gems, Tickets }

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [Header("Starting Funds")]
    public int startingMoney = 1000;
    public int startingGems = 0;
    public int startingTickets = 0;

    private readonly Dictionary<CurrencyType, int> currency = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        currency[CurrencyType.Money] = startingMoney;
        currency[CurrencyType.Gems] = startingGems;
        currency[CurrencyType.Tickets] = startingTickets;
    }

    public bool Spend(CurrencyType type, int amount)
    {
        if (amount <= 0)
            return true;

        if (!currency.ContainsKey(type) || currency[type] < amount)
        {
            Debug.Log("Not enough funds.");
            return false;
        }

        currency[type] -= amount;
        OnCurrencyChanged(type);
        return true;
    }

    public void Add(CurrencyType type, int amount)
    {
        if (amount <= 0)
            return;

        if (!currency.ContainsKey(type))
            currency[type] = 0;

        currency[type] += amount;
        OnCurrencyChanged(type);
    }

    public int GetAmount(CurrencyType type)
    {
        return currency.TryGetValue(type, out int value) ? value : 0;
    }

    public System.Action<CurrencyType, int> CurrencyChanged;

    private void OnCurrencyChanged(CurrencyType type)
    {
        CurrencyChanged?.Invoke(type, GetAmount(type));
    }
}
