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

        if (!EconomyManager.Instance.Spend(item.price))

        {
            Debug.Log("Purchase failed  not enough funds.");
            return;
        }

        switch (item.itemType)
        {
            case ShopItemType.Crate:
                GiveCrate(item.crate);
                break;
            case ShopItemType.DepartmentUpgrade:
                UpgradeDepartment(item.upgradeDepartment);
                break;
        }
    }

    private void GiveCrate(CrateData crate)
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

        var crateObj = Instantiate(cratePrefab, Vector3.zero, Quaternion.identity);
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
