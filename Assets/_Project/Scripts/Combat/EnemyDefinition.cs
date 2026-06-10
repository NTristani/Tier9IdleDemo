using UnityEngine;

[CreateAssetMenu(
    fileName = "EnemyDefinition",
    menuName = "Idle Adventurer/Combat/Enemy Definition")]
public class EnemyDefinition : ScriptableObject
{
    [Header("Identity")]
    public string enemyId = "enemy_green";
    public string displayName = "Green Enemy";

    [Header("Stats")]
    public int maxHealth = 10;

    [Header("Rewards")]
    public int xpReward = 5;
    public int coinReward = 2;

    [Header("Material Drop")]
    public ItemDefinition materialDrop;
    [Range(0f, 1f)] public float materialDropChance = 0.75f;
    public int minMaterialAmount = 1;
    public int maxMaterialAmount = 2;
}