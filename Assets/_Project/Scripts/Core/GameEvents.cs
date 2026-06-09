using System;

public static class GameEvents
{
    public static event Action<int, int, int> PlayerStatsChanged;
    public static event Action<int> CoinsChanged;
    public static event Action<string, int, int> EnemyKilled;

    public static void RaisePlayerStatsChanged(int level, int currentXp, int requiredXp)
    {
        PlayerStatsChanged?.Invoke(level, currentXp, requiredXp);
    }

    public static void RaiseCoinsChanged(int coins)
    {
        CoinsChanged?.Invoke(coins);
    }

    public static void RaiseEnemyKilled(string enemyId, int xpReward, int coinReward)
    {
        EnemyKilled?.Invoke(enemyId, xpReward, coinReward);
    }
}
