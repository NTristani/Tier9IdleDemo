using System;
using UnityEngine;

public class OfflineProgressManager : MonoBehaviour
{
    [Header("Combat Source")]
    [SerializeField] private EnemyDefinition offlineEnemyDefinition;

    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private AutoCombatController autoCombatController;
    [SerializeField] private CurrencyWallet currencyWallet;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private QuestManager questManager;

    [Header("Offline Balance")]
    [SerializeField] private double minimumOfflineSeconds = 10;
    [SerializeField] private double maxOfflineHours = 8;
    [SerializeField] private float offlineEfficiency = 0.75f;
    [SerializeField] private float respawnDelaySeconds = 0.5f;

    [Header("Demo Testing")]
    [SerializeField] private double demoSimulatedSeconds = 600;

    public bool ApplyOfflineProgressFromTimestamp(string savedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(savedAtUtc))
        {
            return false;
        }

        if (!DateTime.TryParse(
                savedAtUtc,
                null,
                System.Globalization.DateTimeStyles.RoundtripKind,
                out DateTime savedTimeUtc))
        {
            Debug.LogWarning($"Could not parse saved timestamp: {savedAtUtc}");
            return false;
        }

        DateTime nowUtc = DateTime.UtcNow;
        double secondsAway = (nowUtc - savedTimeUtc).TotalSeconds;

        if (secondsAway < minimumOfflineSeconds)
        {
            Debug.Log($"Offline time was only {secondsAway:0.0}s. No offline rewards applied.");
            return false;
        }

        OfflineProgressResult result = CalculateRewards(secondsAway);
        ApplyRewards(result);

        return result.HasRewards;
    }

    public void SimulateDemoOfflineProgress()
    {
        OfflineProgressResult result = CalculateRewards(demoSimulatedSeconds);
        ApplyRewards(result);
    }

    private OfflineProgressResult CalculateRewards(double realSecondsAway)
    {
        OfflineProgressResult result = new OfflineProgressResult
        {
            realSecondsAway = realSecondsAway
        };

        if (offlineEnemyDefinition == null)
        {
            Debug.LogWarning("OfflineProgressManager has no offline enemy definition assigned.");
            return result;
        }

        if (playerStats == null || autoCombatController == null)
        {
            Debug.LogWarning("OfflineProgressManager is missing PlayerStats or AutoCombatController.");
            return result;
        }

        double maxSeconds = maxOfflineHours * 60.0 * 60.0;
        double secondsUsed = Math.Min(realSecondsAway, maxSeconds);
        result.simulatedSecondsUsed = secondsUsed;

        int damage = Mathf.Max(1, playerStats.Damage);
        float attacksPerSecond = Mathf.Max(0.1f, autoCombatController.AttacksPerSecond);

        int enemyHealth = Mathf.Max(1, offlineEnemyDefinition.maxHealth);
        int hitsToKill = Mathf.CeilToInt(enemyHealth / (float)damage);

        double secondsPerKill = (hitsToKill / attacksPerSecond) + respawnDelaySeconds;

        if (secondsPerKill <= 0)
        {
            return result;
        }

        int kills = Mathf.FloorToInt((float)((secondsUsed / secondsPerKill) * offlineEfficiency));

        if (kills <= 0)
        {
            return result;
        }

        result.enemiesDefeated = kills;
        result.xpGained = kills * offlineEnemyDefinition.xpReward;
        result.coinsGained = kills * offlineEnemyDefinition.coinReward;

        if (offlineEnemyDefinition.materialDrop != null)
        {
            float averageDropAmount =
                (offlineEnemyDefinition.minMaterialAmount + offlineEnemyDefinition.maxMaterialAmount) * 0.5f;

            int materialAmount = Mathf.FloorToInt(
                kills *
                offlineEnemyDefinition.materialDropChance *
                averageDropAmount
            );

            result.materialItem = offlineEnemyDefinition.materialDrop;
            result.materialAmount = Mathf.Max(0, materialAmount);
        }

        return result;
    }

    private void ApplyRewards(OfflineProgressResult result)
    {
        if (result == null || !result.HasRewards)
        {
            return;
        }

        if (playerStats != null && result.xpGained > 0)
        {
            playerStats.AddExperience(result.xpGained);
        }

        if (currencyWallet != null && result.coinsGained > 0)
        {
            currencyWallet.AddCoins(result.coinsGained);
        }

        if (inventoryManager != null && result.materialItem != null && result.materialAmount > 0)
        {
            inventoryManager.AddItem(result.materialItem, result.materialAmount);
        }

        if (questManager != null && offlineEnemyDefinition != null && result.enemiesDefeated > 0)
        {
            questManager.AddEnemyKills(offlineEnemyDefinition.enemyId, result.enemiesDefeated);
        }

        GameEvents.RaiseOfflineProgressApplied(result);

        Debug.Log(
            $"Offline progress applied: {result.enemiesDefeated} kills, " +
            $"{result.xpGained} XP, {result.coinsGained} coins, " +
            $"{result.materialAmount} materials."
        );
    }
}