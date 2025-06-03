using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [Header("Starting Funds")]
    public int startingMoney = 1000;

    private int currentMoney;

    public int CurrentMoney => currentMoney;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        currentMoney = startingMoney;
    }

    public bool Spend(int amount)
    {
        if (amount <= 0)
            return true;

        if (currentMoney < amount)
        {
            Debug.Log("Not enough funds.");
            return false;
        }

        currentMoney -= amount;
        return true;
    }

    public void Add(int amount)
    {
        if (amount > 0)
            currentMoney += amount;
    }
}
