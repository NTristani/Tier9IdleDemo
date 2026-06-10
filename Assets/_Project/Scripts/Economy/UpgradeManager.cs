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

        ApplyUpgradeEffect(upgrade);

        GameEvents.RaiseUpgradeLevelChanged(upgrade, data.level);
    }

    private void ApplyUpgradeEffect(UpgradeDefinition upgrade)
    {
        switch (upgrade.upgradeType)
        {
            case UpgradeType.Damage:
                if (playerStats != null)
                {
                    playerStats.AddDamageBonus(Mathf.RoundToInt(upgrade.valuePerLevel));
                }
                break;

            case UpgradeType.AttackSpeed:
                if (autoCombatController != null)
                {
                    autoCombatController.AddAttackSpeedBonus(upgrade.valuePerLevel);
                }
                break;
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
}