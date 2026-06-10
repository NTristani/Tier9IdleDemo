using TMPro;
using UnityEngine;

public class InventoryTrackerUI : MonoBehaviour
{
    [Header("Tracked Item")]
    [SerializeField] private ItemDefinition trackedItem;

    [Header("References")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private TMP_Text itemCountText;

    private void OnEnable()
    {
        GameEvents.InventoryChanged += HandleInventoryChanged;
    }

    private void OnDisable()
    {
        GameEvents.InventoryChanged -= HandleInventoryChanged;
    }

    private void Start()
    {
        Refresh();
    }

    private void HandleInventoryChanged(ItemDefinition item, int newAmount)
    {
        if (item != trackedItem)
        {
            return;
        }

        SetText(newAmount);
    }

    private void Refresh()
    {
        int amount = 0;

        if (inventoryManager != null && trackedItem != null)
        {
            amount = inventoryManager.GetQuantity(trackedItem);
        }

        SetText(amount);
    }

    private void SetText(int amount)
    {
        if (itemCountText == null)
        {
            return;
        }

        string itemName = trackedItem != null ? trackedItem.displayName : "Item";
        itemCountText.text = $"{itemName}: {amount}";
    }
}