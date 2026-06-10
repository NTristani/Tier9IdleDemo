using UnityEngine;

public class AutoCombatController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;

    [Header("Combat")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float baseAttacksPerSecond = 1f;
    [SerializeField] private float attackSpeedBonus = 0f;
    [SerializeField] private LayerMask enemyLayer;

    private float attackCooldown;

    public float AttacksPerSecond => baseAttacksPerSecond + attackSpeedBonus;
    public float AttackSpeedBonus => attackSpeedBonus;

    private void Awake()
    {
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }
    }

    private void Start()
    {
        BroadcastCombatStats();
    }

    private void Update()
    {
        attackCooldown -= Time.deltaTime;

        if (attackCooldown > 0f)
        {
            return;
        }

        Enemy target = FindNearestEnemy();

        if (target == null)
        {
            return;
        }

        Attack(target);
    }

    public void AddAttackSpeedBonus(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        attackSpeedBonus += amount;
        BroadcastCombatStats();
    }

    public void SetAttackSpeedBonus(float amount)
    {
        attackSpeedBonus = Mathf.Max(0f, amount);
        BroadcastCombatStats();
    }

    public void ForceBroadcastCombatStats()
    {
        BroadcastCombatStats();
    }

    private Enemy FindNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        Enemy nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();

            if (enemy == null || !enemy.IsAlive)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    private void Attack(Enemy target)
    {
        float safeAttacksPerSecond = Mathf.Max(0.1f, AttacksPerSecond);
        attackCooldown = 1f / safeAttacksPerSecond;

        target.TakeDamage(playerStats.Damage);

        BroadcastCombatStats();
    }

    private void BroadcastCombatStats()
    {
        if (playerStats == null)
        {
            return;
        }

        GameEvents.RaiseCombatStatsChanged(playerStats.Damage, AttacksPerSecond);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}