using System.Collections.Generic;

public class InventoryOverflow
{
    public int currentSlots;
    public int maxSlots;
    public List<Item> storedItems = new();

    public bool CanStore(Item item)
    {
        if (item == null)
            return false;

        return storedItems.Count < currentSlots;
    }

    public void Store(Item item)
    {
        if (!CanStore(item))
            return;

        storedItems.Add(item);
    }

    public void ExpandSlots(int amount)
    {
        if (amount <= 0)
            return;

        currentSlots = System.Math.Min(currentSlots + amount, maxSlots);
    }
}
