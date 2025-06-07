using UnityEngine;
using UnityEngine.UI;

public class CrateShopPanel : MonoBehaviour
{
    [System.Serializable]
    public class DepartmentButton
    {
        public DepartmentType department;
        public CrateData crate;
        public Button button;
        public int price = 100;
    }

    [Header("Setup")]
    public GameObject cratePrefab;
    public DepartmentButton[] departmentButtons;

    private void Start()
    {
        foreach (var entry in departmentButtons)
        {
            if (entry.button == null || entry.crate == null)
                continue;

            var captured = entry;
            entry.button.onClick.AddListener(() => PurchaseCrate(captured));
        }
    }

    private void PurchaseCrate(DepartmentButton entry)
    {
        if (EconomyManager.Instance == null)
        {
            Debug.LogError("EconomyManager not found.");
            return;
        }

        if (!EconomyManager.Instance.Spend(CurrencyType.Money, entry.price))
        {
            Debug.Log("Not enough money for crate.");
            return;
        }

        if (cratePrefab == null)
        {
            Debug.LogError("Crate prefab not assigned.");
            EconomyManager.Instance.Add(CurrencyType.Money, entry.price);
            return;
        }

        var gridManager = FindFirstObjectByType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("GridManager not found.");
            EconomyManager.Instance.Add(CurrencyType.Money, entry.price);
            return;
        }

        Vector2Int? freeCell = gridManager.GetRandomFreeCell();
        if (freeCell == null)
        {
            Debug.LogWarning("No free grid cell available.");
            EconomyManager.Instance.Add(CurrencyType.Money, entry.price);
            return;
        }

        Vector3 worldPos = gridManager.GetWorldPosition(freeCell.Value);
        var crateObj = Instantiate(cratePrefab, worldPos, Quaternion.identity, gridManager.tileParent);
        var spawner = crateObj.GetComponent<DepartmentCrateSpawner>();
        if (spawner != null)
        {
            spawner.crateData = entry.crate;
            spawner.RefillCrate();
        }
        else
        {
            Debug.LogError("Spawned object missing DepartmentCrateSpawner.");
            Destroy(crateObj);
            EconomyManager.Instance.Add(CurrencyType.Money, entry.price);
        }
    }
}
