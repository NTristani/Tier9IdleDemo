using TMPro;
using UnityEngine;

public class CurrentObjectiveUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private QuestManager questManager;
    [SerializeField] private WorldZoneManager worldZoneManager;
    [SerializeField] private UpgradeManager upgradeManager;

    [Header("Optional Upgrade Goal")]
    [SerializeField] private UpgradeDefinition firstUpgradeGoal;

    private QuestDefinition currentQuest;
    private int currentKills;
    private bool isQuestComplete;
    private bool rewardClaimed;

    private void OnEnable()
    {
        GameEvents.QuestProgressChanged += HandleQuestProgressChanged;
        GameEvents.ZoneChanged += HandleZoneChanged;
        GameEvents.UpgradeLevelChanged += HandleUpgradeLevelChanged;
        GameEvents.CoinsChanged += HandleCoinsChanged;
        GameEvents.InventoryChanged += HandleInventoryChanged;
    }

    private void OnDisable()
    {
        GameEvents.QuestProgressChanged -= HandleQuestProgressChanged;
        GameEvents.ZoneChanged -= HandleZoneChanged;
        GameEvents.UpgradeLevelChanged -= HandleUpgradeLevelChanged;
        GameEvents.CoinsChanged -= HandleCoinsChanged;
        GameEvents.InventoryChanged -= HandleInventoryChanged;
    }

    private void Start()
    {
        if (questManager != null)
        {
            currentQuest = questManager.ActiveQuest;
            currentKills = questManager.CurrentKills;
            isQuestComplete = questManager.IsQuestComplete;
            rewardClaimed = questManager.RewardClaimed;
        }

        RefreshObjective();
    }

    private void HandleQuestProgressChanged(
        QuestDefinition quest,
        int kills,
        bool complete,
        bool claimed)
    {
        currentQuest = quest;
        currentKills = kills;
        isQuestComplete = complete;
        rewardClaimed = claimed;

        RefreshObjective();
    }

    private void HandleZoneChanged(GameZone newZone)
    {
        RefreshObjective();
    }

    private void HandleUpgradeLevelChanged(UpgradeDefinition upgrade, int newLevel)
    {
        RefreshObjective();
    }

    private void HandleCoinsChanged(int coins)
    {
        RefreshObjective();
    }

    private void HandleInventoryChanged(ItemDefinition item, int newAmount)
    {
        RefreshObjective();
    }

    private void RefreshObjective()
    {
        if (objectiveText == null)
        {
            return;
        }

        GameZone currentZone = GameZone.Town;

        if (worldZoneManager != null)
        {
            currentZone = worldZoneManager.CurrentZone;
        }

        objectiveText.text = GetObjectiveText(currentZone);
    }

    private string GetObjectiveText(GameZone currentZone)
    {
        if (currentQuest == null)
        {
            return "Current Goal: Explore the town.";
        }

        if (!isQuestComplete)
        {
            if (currentZone == GameZone.Town)
            {
                if (currentKills <= 0)
                {
                    return "Current Goal: Talk to the Field Guide, then travel to the monster field.";
                }

                return $"Current Goal: Return to the field and defeat {currentQuest.requiredKills - currentKills} more {currentQuest.targetDisplayName}.";
            }

            return $"Current Goal: Defeat {currentQuest.targetDisplayName}: {currentKills} / {currentQuest.requiredKills}.";
        }

        if (isQuestComplete && !rewardClaimed)
        {
            if (currentZone == GameZone.CombatField)
            {
                return "Current Goal: Return to town and claim your quest reward.";
            }

            return "Current Goal: Talk to the Field Guide and claim your reward.";
        }

        if (rewardClaimed)
        {
            if (firstUpgradeGoal != null && upgradeManager != null)
            {
                int upgradeLevel = upgradeManager.GetUpgradeLevel(firstUpgradeGoal);

                if (upgradeLevel <= 0)
                {
                    return $"Current Goal: Buy your first upgrade: {firstUpgradeGoal.displayName}.";
                }
            }

            if (currentZone == GameZone.Town)
            {
                return "Current Goal: Travel to the field and test combat AFK/offline gains.";
            }

            return "Current Goal: Keep farming enemies, buy upgrades, and test offline progress.";
        }

        return "Current Goal: Keep progressing through the demo loop.";
    }
}