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
    [SerializeField] private WorldZoneManager worldZoneManager;

    [Header("Offline Balance")]
    [SerializeField] private double minimumOfflineSeconds = 1;
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
        double rawSecondsAway = (nowUtc - savedTimeUtc).TotalSeconds;

        // Round milliseconds up to the nearest whole second.
        double roundedSecondsAway = Math.Ceiling(Math.Max(0, rawSecondsAway));

        if (roundedSecondsAway < minimumOfflineSeconds)
        {
            Debug.Log($"Offline time was only {roundedSecondsAway:0}s. No offline progress applied.");
            return false;
        }

        OfflineProgressResult result = CalculateRewards(roundedSecondsAway);
        ApplyRewards(result);

        return result.HasOfflineTime;
    }

    public void SimulateDemoOfflineProgress()
    {
        if (worldZoneManager != null && worldZoneManager.CurrentZone != GameZone.CombatField)
        {
            Debug.Log("Offline combat simulation is only available while in the combat field.");
            return;
        }

        OfflineProgressResult result = CalculateRewards(demoSimulatedSeconds);
        ApplyRewards(result);
    }

    private OfflineProgressResult CalculateRewards(double secondsAway)
    {
        OfflineProgressResult result = new OfflineProgressResult
        {
            realSecondsAway = secondsAway
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

        // Clamp to max offline duration, then round up to nearest second.
        double secondsUsed = Math.Ceiling(Math.Min(secondsAway, maxSeconds));
        secondsUsed = Math.Max(1, secondsUsed);

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

        double estimatedKills = (secondsUsed / secondsPerKill) * offlineEfficiency;

        // We keep actual defeated enemies whole because quest progress uses whole kills.
        int wholeKills = Mathf.FloorToInt((float)estimatedKills);

        if (wholeKills <= 0)
        {
            return result;
        }

        result.enemiesDefeated = wholeKills;
        result.xpGained = wholeKills * offlineEnemyDefinition.xpReward;
        result.coinsGained = wholeKills * offlineEnemyDefinition.coinReward;

        if (offlineEnemyDefinition.materialDrop != null)
        {
            float averageDropAmount =
                (offlineEnemyDefinition.minMaterialAmount + offlineEnemyDefinition.maxMaterialAmount) * 0.5f;

            int materialAmount = Mathf.FloorToInt(
                wholeKills *
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
        if (result == null || !result.HasOfflineTime)
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

        // Raise the event even if the time was too short to earn a full kill.
        // This lets the popup show exactly how much time passed.
        GameEvents.RaiseOfflineProgressApplied(result);

        Debug.Log(
            $"Offline progress applied for {result.simulatedSecondsUsed:0}s: " +
            $"{result.enemiesDefeated} kills, " +
            $"{result.xpGained} XP, {result.coinsGained} coins, " +
            $"{result.materialAmount} materials."
        );
    }
}