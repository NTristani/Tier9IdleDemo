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

    [Header("Death Animation")]
    [SerializeField] private float deathAnimationDuration = 0.22f;
    [SerializeField] private float deathScaleMultiplier = 1.25f;
    [SerializeField] private bool fadeOnDeath = true;

    [Header("Runtime")]
    [SerializeField] private int currentHealth;

    private Color originalColor;
    private Vector3 originalScale;
    private Coroutine flashRoutine;
    private bool isDying;

    private Rigidbody2D rb;
    private EnemyMovement enemyMovement;
    private Collider2D[] colliders;

    public bool IsAlive => currentHealth > 0 && !isDying;
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

        originalScale = transform.localScale;

        rb = GetComponent<Rigidbody2D>();
        enemyMovement = GetComponent<EnemyMovement>();
        colliders = GetComponentsInChildren<Collider2D>();
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

        isDying = false;
        currentHealth = enemyDefinition.maxHealth;
        transform.localScale = originalScale;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        HealthChanged?.Invoke(currentHealth, MaxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (!IsAlive)
        {
            return;
        }

        int finalDamage = Mathf.Max(1, damage);
        currentHealth = Mathf.Max(0, currentHealth - finalDamage);

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
        if (spriteRenderer == null || isDying)
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

        if (!isDying && spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private void Die()
    {
        if (isDying)
        {
            return;
        }

        isDying = true;
        currentHealth = 0;
        HealthChanged?.Invoke(currentHealth, MaxHealth);

        TryDropMaterial();

        if (enemyDefinition != null)
        {
            GameEvents.RaiseEnemyKilled(
                enemyDefinition.enemyId,
                enemyDefinition.xpReward,
                enemyDefinition.coinReward
            );
        }

        DisableEnemyInteraction();

        StartCoroutine(DeathAnimationRoutine());
    }

    private void DisableEnemyInteraction()
    {
        if (enemyMovement != null)
        {
            enemyMovement.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (colliders == null)
        {
            return;
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                colliders[i].enabled = false;
            }
        }
    }

    private IEnumerator DeathAnimationRoutine()
    {
        float timer = 0f;

        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale * deathScaleMultiplier;

        Color startColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

        while (timer < deathAnimationDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / deathAnimationDuration);

            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            if (fadeOnDeath && spriteRenderer != null)
            {
                float alpha = Mathf.Lerp(startColor.a, 0f, t);
                spriteRenderer.color = new Color(
                    startColor.r,
                    startColor.g,
                    startColor.b,
                    alpha
                );
            }

            yield return null;
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