using System;

public static class GameEvents
{
    public static event Action<int, int, int> PlayerStatsChanged;
    public static event Action<int> CoinsChanged;
    public static event Action<string, int, int> EnemyKilled;

    public static event Action<QuestDefinition, int, bool, bool> QuestProgressChanged;

    public static event Action<ItemDefinition, int> ItemCollected;
    public static event Action<ItemDefinition, int> InventoryChanged;

    public static event Action<int, float> CombatStatsChanged;
    public static event Action<UpgradeDefinition, int> UpgradeLevelChanged;

    public static event Action<OfflineProgressResult> OfflineProgressApplied;

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

    public static void RaiseQuestProgressChanged(
        QuestDefinition quest,
        int currentKills,
        bool isComplete,
        bool rewardClaimed)
    {
        QuestProgressChanged?.Invoke(quest, currentKills, isComplete, rewardClaimed);
    }

    public static void RaiseItemCollected(ItemDefinition item, int amount)
    {
        ItemCollected?.Invoke(item, amount);
    }

    public static void RaiseInventoryChanged(ItemDefinition item, int newAmount)
    {
        InventoryChanged?.Invoke(item, newAmount);
    }

    public static void RaiseCombatStatsChanged(int damage, float attacksPerSecond)
    {
        CombatStatsChanged?.Invoke(damage, attacksPerSecond);
    }

    public static void RaiseUpgradeLevelChanged(UpgradeDefinition upgrade, int newLevel)
    {
        UpgradeLevelChanged?.Invoke(upgrade, newLevel);
    }

    public static void RaiseOfflineProgressApplied(OfflineProgressResult result)
    {
        OfflineProgressApplied?.Invoke(result);
    }
}