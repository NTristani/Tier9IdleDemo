using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonUI : MonoBehaviour
{
    [Header("Upgrade")]
    [SerializeField] private UpgradeDefinition upgrade;

    [Header("References")]
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text costText;

    private void OnEnable()
    {
        GameEvents.CoinsChanged += HandleCoinsChanged;
        GameEvents.InventoryChanged += HandleInventoryChanged;
        GameEvents.UpgradeLevelChanged += HandleUpgradeLevelChanged;

        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(HandlePurchaseClicked);
        }

        Refresh();
    }

    private void OnDisable()
    {
        GameEvents.CoinsChanged -= HandleCoinsChanged;
        GameEvents.InventoryChanged -= HandleInventoryChanged;
        GameEvents.UpgradeLevelChanged -= HandleUpgradeLevelChanged;

        if (purchaseButton != null)
        {
            purchaseButton.onClick.RemoveListener(HandlePurchaseClicked);
        }
    }

    private void HandlePurchaseClicked()
    {
        if (upgradeManager == null || upgrade == null)
        {
            return;
        }

        upgradeManager.PurchaseUpgrade(upgrade);
        Refresh();
    }

    private void HandleCoinsChanged(int coins)
    {
        Refresh();
    }

    private void HandleInventoryChanged(ItemDefinition item, int newAmount)
    {
        Refresh();
    }

    private void HandleUpgradeLevelChanged(UpgradeDefinition changedUpgrade, int newLevel)
    {
        if (changedUpgrade == upgrade)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        if (upgrade == null || upgradeManager == null)
        {
            return;
        }

        int currentLevel = upgradeManager.GetUpgradeLevel(upgrade);
        bool isMaxed = currentLevel >= upgrade.maxLevel;

        if (titleText != null)
        {
            titleText.text = $"{upgrade.displayName} Lv. {currentLevel}/{upgrade.maxLevel}";
        }

        if (descriptionText != null)
        {
            descriptionText.text = upgrade.description;
        }

        if (costText != null)
        {
            if (isMaxed)
            {
                costText.text = "MAX LEVEL";
            }
            else
            {
                int coinCost = upgrade.GetCoinCostForLevel(currentLevel);
                int itemCost = upgrade.GetItemCostForLevel(currentLevel);
                string itemName = upgrade.requiredItem != null ? upgrade.requiredItem.displayName : "Item";

                costText.text = $"{coinCost} Coins, {itemCost} {itemName}";
            }
        }

        if (purchaseButton != null)
        {
            purchaseButton.interactable = !isMaxed && upgradeManager.CanPurchase(upgrade);
        }
    }
}