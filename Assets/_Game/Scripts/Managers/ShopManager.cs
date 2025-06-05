using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject cratePrefab;

    public void Purchase(ShopItemData item)
    {
        if (item == null)
        {
            Debug.LogWarning("Tried to purchase null item.");
            return;
        }

        if (!EconomyManager.Instance.Spend(CurrencyType.Money, item.price))
        {
            Debug.Log("Purchase failed - not enough funds.");
            return;
        }

        switch (item.itemType)
        {
            case ShopItemType.Crate:
                GiveCrate(item.crate, item.price);
                break;
            case ShopItemType.DepartmentUpgrade:
                UpgradeDepartment(item.upgradeDepartment);
                break;
        }
    }

    private void GiveCrate(CrateData crate, int price)
    {
        if (crate == null)
        {
            Debug.LogWarning("Shop item missing crate reference.");
            return;
        }

        if (cratePrefab == null)
        {
            Debug.LogError("Crate prefab reference not set on ShopManager.");
            return;
        }

        GridManager gridManager = FindFirstObjectByType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("GridManager not found in scene.");
            EconomyManager.Instance.Add(CurrencyType.Money, price);
            return;
        }

        Vector2Int? freeCell = gridManager.GetRandomFreeCell();
        if (freeCell == null)
        {
            Debug.LogWarning("No free grid cell available. Purchase refunded.");
            EconomyManager.Instance.Add(CurrencyType.Money, price);
            return;
        }

        Vector3 worldPos = gridManager.GetWorldPosition(freeCell.Value);
        var crateObj = Instantiate(cratePrefab, worldPos, Quaternion.identity, gridManager.tileParent);
        var spawner = crateObj.GetComponent<DepartmentCrateSpawner>();
        if (spawner != null)
        {
            spawner.crateData = crate;
            spawner.RefillCrate();
        }
    }

    private void UpgradeDepartment(DepartmentType department)
    {
        Debug.Log($"Purchased upgrade for {department} department.");
        // TODO: integrate with actual department upgrade system
    }
}
