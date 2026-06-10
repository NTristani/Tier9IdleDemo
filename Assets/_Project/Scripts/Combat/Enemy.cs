using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EnemyDefinition enemyDefinition;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hitFlashColor = Color.white;
    [SerializeField] private float hitFlashDuration = 0.08f;

    [Header("Runtime")]
    [SerializeField] private int currentHealth;

    private Color originalColor;
    private Coroutine flashRoutine;

    public bool IsAlive => currentHealth > 0;
    public EnemyDefinition Definition => enemyDefinition;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => enemyDefinition != null ? enemyDefinition.maxHealth : 1;

    public event Action<int, int> HealthChanged;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

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
        HealthChanged?.Invoke(currentHealth, MaxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (!IsAlive)
        {
            return;
        }

        int finalDamage = Mathf.Max(1, damage);
        currentHealth -= finalDamage;

        HealthChanged?.Invoke(currentHealth, MaxHealth);

        FloatingDamageSpawner.Instance?.SpawnDamageNumber(
            finalDamage,
            transform.position + Vector3.up * 0.8f
        );

        PlayHitFlash();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void PlayHitFlash()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        spriteRenderer.color = hitFlashColor;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        currentHealth = 0;

        TryDropMaterial();

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

    private void TryDropMaterial()
    {
        if (enemyDefinition == null)
        {
            return;
        }

        if (enemyDefinition.materialDrop == null)
        {
            return;
        }

        float roll = UnityEngine.Random.value;

        if (roll > enemyDefinition.materialDropChance)
        {
            return;
        }

        int minAmount = Mathf.Max(1, enemyDefinition.minMaterialAmount);
        int maxAmount = Mathf.Max(minAmount, enemyDefinition.maxMaterialAmount);

        int amount = UnityEngine.Random.Range(minAmount, maxAmount + 1);

        GameEvents.RaiseItemCollected(enemyDefinition.materialDrop, amount);
    }
}