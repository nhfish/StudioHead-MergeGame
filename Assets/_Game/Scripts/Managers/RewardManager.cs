using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void GrantRewards(int money, int fans)
    {
        if (EconomyManager.Instance != null)
            EconomyManager.Instance.Add(CurrencyType.Money, money);
        // Fans reward system not implemented yet
        Debug.Log($" Granted {money} money and {fans} fans");
    }
}
