using UnityEngine;

[CreateAssetMenu(
    fileName = "QuestDefinition",
    menuName = "Idle Adventurer/Quests/Quest Definition")]
public class QuestDefinition : ScriptableObject
{
    [Header("Identity")]
    public string questId = "clear_the_field";
    public string displayName = "Clear the Field";
    [TextArea] public string description = "Defeat enemies in the field.";

    [Header("Objective")]
    public string targetEnemyId = "enemy_green";
    public string targetDisplayName = "Enemy Green";
    public int requiredKills = 5;

    [Header("Rewards")]
    public int coinReward = 25;
    public int xpReward = 20;
}