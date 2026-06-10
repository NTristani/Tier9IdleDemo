using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Serializable]
    private class UpgradeRuntimeData
    {
        public UpgradeDefinition upgrade;
        public int level;
    }

    [Serializable]
    public class UpgradeSaveEntry
    {
        public string upgradeId;
        public int level;
    }

    [Header("Available Upgrades")]
    [SerializeField] private List<UpgradeRuntimeData> upgrades = new List<UpgradeRuntimeData>();

    [Header("References")]
    [SerializeField] private CurrencyWallet currencyWallet;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private AutoCombatController autoCombatController;

    public int GetUpgradeLevel(UpgradeDefinition upgrade)
    {
        UpgradeRuntimeData data = GetRuntimeData(upgrade);
        return data != null ? data.level : 0;
    }

    public bool CanPurchase(UpgradeDefinition upgrade)
    {
        if (upgrade == null)
        {
            return false;
        }

        UpgradeRuntimeData data = GetRuntimeData(upgrade);

        if (data == null)
        {
            return false;
        }

        if (data.level >= upgrade.maxLevel)
        {
            return false;
        }

        int coinCost = upgrade.GetCoinCostForLevel(data.level);
        int itemCost = upgrade.GetItemCostForLevel(data.level);

        if (currencyWallet == null || currencyWallet.Coins < coinCost)
        {
            return false;
        }

        if (upgrade.requiredItem != null)
        {
            if (inventoryManager == null || !inventoryManager.HasItem(upgrade.requiredItem, itemCost))
            {
                return false;
            }
        }

        return true;
    }

    public void PurchaseUpgrade(UpgradeDefinition upgrade)
    {
        if (upgrade == null)
        {
            Debug.LogWarning("Tried to purchase a null upgrade.");
            return;
        }

        UpgradeRuntimeData data = GetRuntimeData(upgrade);

        if (data == null)
        {
            Debug.LogWarning($"Upgrade {upgrade.displayName} is not registered in UpgradeManager.");
            return;
        }

        if (data.level >= upgrade.maxLevel)
        {
            Debug.Log($"{upgrade.displayName} is already max level.");
            return;
        }

        int coinCost = upgrade.GetCoinCostForLevel(data.level);
        int itemCost = upgrade.GetItemCostForLevel(data.level);

        if (currencyWallet == null || !currencyWallet.TrySpendCoins(coinCost))
        {
            Debug.Log("Not enough coins.");
            return;
        }

        if (upgrade.requiredItem != null && itemCost > 0)
        {
            if (inventoryManager == null || !inventoryManager.TrySpendItem(upgrade.requiredItem, itemCost))
            {
                currencyWallet.AddCoins(coinCost);
                Debug.Log("Not enough materials.");
                return;
            }
        }

        data.level++;

        ApplyAllUpgradeEffects();

        GameEvents.RaiseUpgradeLevelChanged(upgrade, data.level);
    }

    public List<UpgradeSaveEntry> CaptureSaveData()
    {
        List<UpgradeSaveEntry> saveEntries = new List<UpgradeSaveEntry>();

        for (int i = 0; i < upgrades.Count; i++)
        {
            if (upgrades[i].upgrade == null)
            {
                continue;
            }

            saveEntries.Add(new UpgradeSaveEntry
            {
                upgradeId = upgrades[i].upgrade.upgradeId,
                level = upgrades[i].level
            });
        }

        return saveEntries;
    }

    public void LoadFromSaveData(List<UpgradeSaveEntry> saveEntries)
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            upgrades[i].level = 0;
        }

        if (saveEntries != null)
        {
            for (int i = 0; i < saveEntries.Count; i++)
            {
                UpgradeRuntimeData data = GetRuntimeDataById(saveEntries[i].upgradeId);

                if (data == null || data.upgrade == null)
                {
                    Debug.LogWarning($"Could not find upgrade with ID {saveEntries[i].upgradeId} while loading.");
                    continue;
                }

                data.level = Mathf.Clamp(saveEntries[i].level, 0, data.upgrade.maxLevel);
            }
        }

        ApplyAllUpgradeEffects();
        BroadcastAllUpgradeLevels();
    }

    public void ResetAllUpgrades()
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            upgrades[i].level = 0;
        }

        ApplyAllUpgradeEffects();
        BroadcastAllUpgradeLevels();
    }

    private void ApplyAllUpgradeEffects()
    {
        int totalDamageBonus = 0;
        float totalAttackSpeedBonus = 0f;

        for (int i = 0; i < upgrades.Count; i++)
        {
            UpgradeDefinition upgrade = upgrades[i].upgrade;

            if (upgrade == null)
            {
                continue;
            }

            switch (upgrade.upgradeType)
            {
                case UpgradeType.Damage:
                    totalDamageBonus += Mathf.RoundToInt(upgrade.valuePerLevel * upgrades[i].level);
                    break;

                case UpgradeType.AttackSpeed:
                    totalAttackSpeedBonus += upgrade.valuePerLevel * upgrades[i].level;
                    break;
            }
        }

        if (playerStats != null)
        {
            playerStats.SetUpgradeDamageBonus(totalDamageBonus);
        }

        if (autoCombatController != null)
        {
            autoCombatController.SetAttackSpeedBonus(totalAttackSpeedBonus);
            autoCombatController.ForceBroadcastCombatStats();
        }
    }

    private void BroadcastAllUpgradeLevels()
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            if (upgrades[i].upgrade == null)
            {
                continue;
            }

            GameEvents.RaiseUpgradeLevelChanged(upgrades[i].upgrade, upgrades[i].level);
        }
    }

    private UpgradeRuntimeData GetRuntimeData(UpgradeDefinition upgrade)
    {
        if (upgrade == null)
        {
            return null;
        }

        for (int i = 0; i < upgrades.Count; i++)
        {
            if (upgrades[i].upgrade == upgrade)
            {
                return upgrades[i];
            }
        }

        return null;
    }

    private UpgradeRuntimeData GetRuntimeDataById(string upgradeId)
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            if (upgrades[i].upgrade != null && upgrades[i].upgrade.upgradeId == upgradeId)
            {
                return upgrades[i];
            }
        }

        return null;
    }
}