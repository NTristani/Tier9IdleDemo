using UnityEngine;

public enum UpgradeType
{
    Damage,
    AttackSpeed
}

[CreateAssetMenu(
    fileName = "UpgradeDefinition",
    menuName = "Idle Adventurer/Economy/Upgrade Definition")]
public class UpgradeDefinition : ScriptableObject
{
    [Header("Identity")]
    public string upgradeId = "strength_training";
    public string displayName = "Strength Training";
    [TextArea] public string description = "Increase your damage.";

    [Header("Upgrade Effect")]
    public UpgradeType upgradeType = UpgradeType.Damage;
    public float valuePerLevel = 1f;
    public int maxLevel = 10;

    [Header("Coin Cost")]
    public int baseCoinCost = 10;
    public int coinCostIncreasePerLevel = 5;

    [Header("Material Cost")]
    public ItemDefinition requiredItem;
    public int baseItemCost = 2;
    public int itemCostIncreasePerLevel = 1;

    public int GetCoinCostForLevel(int currentLevel)
    {
        return baseCoinCost + (coinCostIncreasePerLevel * currentLevel);
    }

    public int GetItemCostForLevel(int currentLevel)
    {
        return baseItemCost + (itemCostIncreasePerLevel * currentLevel);
    }
}