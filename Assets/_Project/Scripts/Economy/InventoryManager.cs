using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Serializable]
    private class InventoryEntry
    {
        public ItemDefinition item;
        public int amount;
    }

    [Header("Runtime Inventory")]
    [SerializeField] private List<InventoryEntry> items = new List<InventoryEntry>();

    private void OnEnable()
    {
        GameEvents.ItemCollected += HandleItemCollected;
    }

    private void OnDisable()
    {
        GameEvents.ItemCollected -= HandleItemCollected;
    }

    private void HandleItemCollected(ItemDefinition item, int amount)
    {
        AddItem(item, amount);
    }

    public void AddItem(ItemDefinition item, int amount)
    {
        if (item == null)
        {
            Debug.LogWarning("Tried to add a null item to inventory.");
            return;
        }

        if (amount <= 0)
        {
            return;
        }

        InventoryEntry entry = GetEntry(item);

        if (entry == null)
        {
            entry = new InventoryEntry
            {
                item = item,
                amount = 0
            };

            items.Add(entry);
        }

        entry.amount += amount;

        GameEvents.RaiseInventoryChanged(item, entry.amount);
    }

    public bool HasItem(ItemDefinition item, int requiredAmount)
    {
        return GetQuantity(item) >= requiredAmount;
    }

    public bool TrySpendItem(ItemDefinition item, int amount)
    {
        if (item == null || amount <= 0)
        {
            return false;
        }

        InventoryEntry entry = GetEntry(item);

        if (entry == null || entry.amount < amount)
        {
            return false;
        }

        entry.amount -= amount;
        GameEvents.RaiseInventoryChanged(item, entry.amount);

        return true;
    }

    public int GetQuantity(ItemDefinition item)
    {
        InventoryEntry entry = GetEntry(item);
        return entry != null ? entry.amount : 0;
    }

    private InventoryEntry GetEntry(ItemDefinition item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == item)
            {
                return items[i];
            }
        }

        return null;
    }
}