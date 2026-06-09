using UnityEngine;

public class AutoCombatController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;

    [Header("Combat")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attacksPerSecond = 1f;
    [SerializeField] private LayerMask enemyLayer;

    private float attackCooldown;

    private void Awake()
    {
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }
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
        attackCooldown = 1f / attacksPerSecond;
        target.TakeDamage(playerStats.Damage);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}