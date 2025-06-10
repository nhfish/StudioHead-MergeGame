using UnityEngine;

public class InventoryOverflowManager : MonoBehaviour
{
    public static InventoryOverflowManager Instance { get; private set; }

    [Header("Overflow Config")]
    public int startingSlots = 4;
    public int maxSlots = 12;

    [Header("Slot Expansion")]
    public int baseExpandCost = 100;

    private InventoryOverflow overflow;

    public System.Action OverflowUpdated;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        overflow = new InventoryOverflow
        {
            currentSlots = startingSlots,
            maxSlots = maxSlots
        };
    }

    public int SlotsUsed => overflow.storedItems.Count;
    public int MaxSlots => overflow.currentSlots;
    public IReadOnlyList<Item> StoredItems => overflow.storedItems;

    public bool AddItem(Item item)
    {
        if (overflow.CanStore(item))
        {
            overflow.Store(item);
            OverflowUpdated?.Invoke();
            return true;
        }
        return false;
    }

    public bool DiscardItem(Item item)
    {
        if (overflow.Remove(item))
        {
            int refund = Mathf.RoundToInt(item.baseValue * 0.1f);
            if (EconomyManager.Instance != null && refund > 0)
                EconomyManager.Instance.Add(CurrencyType.Money, refund);

            OverflowUpdated?.Invoke();
            return true;
        }
        return false;
    }

    public bool TryPurchaseSlots(int amount)
    {
        if (amount <= 0)
            return false;

        int available = overflow.maxSlots - overflow.currentSlots;
        if (available <= 0)
            return false;

        int toBuy = Mathf.Min(amount, available);
        int cost = CalculateCost(toBuy);

        if (EconomyManager.Instance == null || !EconomyManager.Instance.Spend(CurrencyType.Money, cost))
            return false;

        overflow.ExpandSlots(toBuy);
        OverflowUpdated?.Invoke();
        return true;
    }

    private int CalculateCost(int amount)
    {
        int cost = 0;
        for (int i = 0; i < amount; i++)
        {
            int level = overflow.currentSlots + i - startingSlots;
            cost += Mathf.RoundToInt(baseExpandCost * Mathf.Pow(2f, level));
        }
        return cost;
    }
}
