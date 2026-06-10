using UnityEngine;

public class CurrencyWallet : MonoBehaviour
{
    [SerializeField] private int coins = 0;

    public int Coins => coins;

    private void OnEnable()
    {
        GameEvents.EnemyKilled += HandleEnemyKilled;
    }

    private void OnDisable()
    {
        GameEvents.EnemyKilled -= HandleEnemyKilled;
    }

    private void Start()
    {
        GameEvents.RaiseCoinsChanged(coins);
    }

    private void HandleEnemyKilled(string enemyId, int xpReward, int coinReward)
    {
        AddCoins(coinReward);
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        coins += amount;
        GameEvents.RaiseCoinsChanged(coins);
    }

    public void SetCoins(int amount)
    {
        coins = Mathf.Max(0, amount);
        GameEvents.RaiseCoinsChanged(coins);
    }

    public bool TrySpendCoins(int amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        if (coins < amount)
        {
            return false;
        }

        coins -= amount;
        GameEvents.RaiseCoinsChanged(coins);
        return true;
    }
}