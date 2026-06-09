using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Leveling")]
    [SerializeField] private int level = 1;
    [SerializeField] private int currentXp = 0;
    [SerializeField] private int baseXpRequired = 20;

    [Header("Combat Stats")]
    [SerializeField] private int baseDamage = 2;

    public int Level => level;
    public int CurrentXp => currentXp;
    public int RequiredXp => GetRequiredXpForCurrentLevel();
    public int Damage => baseDamage + level;

    private void OnEnable()
    {
        GameEvents.EnemyKilled += HandleEnemyKilled;
    }

    private void OnDisable()
    {
        GameEvents.EnemyKilled -= HandleEnemyKilled;
    }

    private void Start()
    {
        BroadcastStats();
    }

    private void HandleEnemyKilled(string enemyId, int xpReward, int coinReward)
    {
        AddExperience(xpReward);
    }

    public void AddExperience(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentXp += amount;

        while (currentXp >= GetRequiredXpForCurrentLevel())
        {
            currentXp -= GetRequiredXpForCurrentLevel();
            level++;
        }

        BroadcastStats();
    }

    private int GetRequiredXpForCurrentLevel()
    {
        return baseXpRequired + ((level - 1) * 10);
    }

    private void BroadcastStats()
    {
        GameEvents.RaisePlayerStatsChanged(level, currentXp, GetRequiredXpForCurrentLevel());
    }
}