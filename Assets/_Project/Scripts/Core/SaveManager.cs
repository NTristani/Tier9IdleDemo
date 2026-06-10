using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string SaveKey = "IdleAdventurer_SaveData";

    [Serializable]
    private class GameSaveData
    {
        public int playerLevel;
        public int playerXp;
        public int coins;

        public int questCurrentKills;
        public bool questIsComplete;
        public bool questRewardClaimed;

        public List<InventoryManager.InventorySaveEntry> inventory = new List<InventoryManager.InventorySaveEntry>();
        public List<UpgradeManager.UpgradeSaveEntry> upgrades = new List<UpgradeManager.UpgradeSaveEntry>();

        public string savedAtUtc;
    }

    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private CurrencyWallet currencyWallet;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private QuestManager questManager;
    [SerializeField] private UpgradeManager upgradeManager;

    [Header("Settings")]
    [SerializeField] private bool loadOnStart = true;
    [SerializeField] private bool saveOnApplicationQuit = true;

    private bool suppressNextQuitSave;

    private void Start()
    {
        if (loadOnStart)
        {
            LoadGame();
        }
    }

    private void OnApplicationQuit()
    {
        if (suppressNextQuitSave)
        {
            Debug.Log("Quit save suppressed because save data was cleared.");
            return;
        }

        if (saveOnApplicationQuit)
        {
            SaveGame();
        }
    }

    public void SaveGame()
    {
        suppressNextQuitSave = false;

        GameSaveData saveData = new GameSaveData();

        if (playerStats != null)
        {
            saveData.playerLevel = playerStats.Level;
            saveData.playerXp = playerStats.CurrentXp;
        }

        if (currencyWallet != null)
        {
            saveData.coins = currencyWallet.Coins;
        }

        if (questManager != null)
        {
            saveData.questCurrentKills = questManager.CurrentKills;
            saveData.questIsComplete = questManager.IsQuestComplete;
            saveData.questRewardClaimed = questManager.RewardClaimed;
        }

        if (inventoryManager != null)
        {
            saveData.inventory = inventoryManager.CaptureSaveData();
        }

        if (upgradeManager != null)
        {
            saveData.upgrades = upgradeManager.CaptureSaveData();
        }

        saveData.savedAtUtc = DateTime.UtcNow.ToString("o");

        string json = JsonUtility.ToJson(saveData, true);

        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();

        Debug.Log($"Game saved:\n{json}");
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            Debug.Log("No save data found.");
            return;
        }

        string json = PlayerPrefs.GetString(SaveKey);

        if (string.IsNullOrWhiteSpace(json))
        {
            Debug.LogWarning("Save data was empty.");
            return;
        }

        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

        if (saveData == null)
        {
            Debug.LogWarning("Failed to parse save data.");
            return;
        }

        suppressNextQuitSave = false;

        if (playerStats != null)
        {
            playerStats.LoadProgress(saveData.playerLevel, saveData.playerXp);
        }

        if (currencyWallet != null)
        {
            currencyWallet.SetCoins(saveData.coins);
        }

        if (inventoryManager != null)
        {
            inventoryManager.LoadFromSaveData(saveData.inventory);
        }

        if (questManager != null)
        {
            questManager.LoadProgress(
                saveData.questCurrentKills,
                saveData.questIsComplete,
                saveData.questRewardClaimed
            );
        }

        if (upgradeManager != null)
        {
            upgradeManager.LoadFromSaveData(saveData.upgrades);
        }

        Debug.Log($"Game loaded from save created at UTC: {saveData.savedAtUtc}");
    }

    public void ClearSave()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
        }

        ResetRuntimeProgress();

        suppressNextQuitSave = true;

        Debug.Log("Save data cleared and runtime progress reset.");
    }

    public bool HasSave()
    {
        return PlayerPrefs.HasKey(SaveKey);
    }

    private void ResetRuntimeProgress()
    {
        if (playerStats != null)
        {
            playerStats.LoadProgress(1, 0);
            playerStats.SetUpgradeDamageBonus(0);
        }

        if (currencyWallet != null)
        {
            currencyWallet.SetCoins(0);
        }

        if (inventoryManager != null)
        {
            inventoryManager.LoadFromSaveData(new List<InventoryManager.InventorySaveEntry>());
        }

        if (questManager != null)
        {
            questManager.ResetQuestProgress();
        }

        if (upgradeManager != null)
        {
            upgradeManager.ResetAllUpgrades();
        }
    }
}