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

    [Serializable]
    public class InventorySaveEntry
    {
        public string itemId;
        public int amount;
    }

    [Header("Item Database")]
    [SerializeField] private List<ItemDefinition> knownItems = new List<ItemDefinition>();

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

    public void SetItemQuantity(ItemDefinition item, int amount)
    {
        if (item == null)
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

        entry.amount = Mathf.Max(0, amount);
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

    public List<InventorySaveEntry> CaptureSaveData()
    {
        List<InventorySaveEntry> saveEntries = new List<InventorySaveEntry>();

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == null)
            {
                continue;
            }

            saveEntries.Add(new InventorySaveEntry
            {
                itemId = items[i].item.itemId,
                amount = items[i].amount
            });
        }

        return saveEntries;
    }

    public void LoadFromSaveData(List<InventorySaveEntry> saveEntries)
    {
        items.Clear();

        if (saveEntries == null)
        {
            BroadcastKnownItems();
            return;
        }

        for (int i = 0; i < saveEntries.Count; i++)
        {
            ItemDefinition item = FindItemById(saveEntries[i].itemId);

            if (item == null)
            {
                Debug.LogWarning($"Could not find item with ID {saveEntries[i].itemId} while loading inventory.");
                continue;
            }

            SetItemQuantity(item, saveEntries[i].amount);
        }

        BroadcastKnownItems();
    }

    private void BroadcastKnownItems()
    {
        for (int i = 0; i < knownItems.Count; i++)
        {
            if (knownItems[i] == null)
            {
                continue;
            }

            GameEvents.RaiseInventoryChanged(knownItems[i], GetQuantity(knownItems[i]));
        }
    }

    private ItemDefinition FindItemById(string itemId)
    {
        for (int i = 0; i < knownItems.Count; i++)
        {
            if (knownItems[i] != null && knownItems[i].itemId == itemId)
            {
                return knownItems[i];
            }
        }

        return null;
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