using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EnemyDefinition enemyDefinition;

    [Header("Runtime")]
    [SerializeField] private int currentHealth;

    public bool IsAlive => currentHealth > 0;
    public EnemyDefinition Definition => enemyDefinition;

    private void Start()
    {
        Initialize(enemyDefinition);
    }

    public void Initialize(EnemyDefinition definition)
    {
        enemyDefinition = definition;

        if (enemyDefinition == null)
        {
            Debug.LogError($"{name} has no EnemyDefinition assigned.");
            return;
        }

        currentHealth = enemyDefinition.maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (!IsAlive)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        currentHealth = 0;

        if (enemyDefinition != null)
        {
            GameEvents.RaiseEnemyKilled(
                enemyDefinition.enemyId,
                enemyDefinition.xpReward,
                enemyDefinition.coinReward
            );
        }

        Destroy(gameObject);
    }
}