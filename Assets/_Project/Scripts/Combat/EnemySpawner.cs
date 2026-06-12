using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private EnemyDefinition enemyDefinition;

    [Header("Spawning")]
    [SerializeField] private bool spawnOnStart = false;
    [SerializeField] private int maxEnemies = 5;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(6f, 2f);

    [Header("Debug")]
    [SerializeField] private bool logStateChanges = false;

    private readonly List<Enemy> activeEnemies = new List<Enemy>();
    private float spawnTimer;
    private bool isSpawning;
    private bool hasBeenControlledExternally;

    public bool IsSpawning => isSpawning;

    private void Start()
    {
        /*
         * Important:
         * If WorldZoneManager or SaveManager already called BeginSpawning()
         * before this Start() method runs, do not turn spawning off again.
         */
        if (hasBeenControlledExternally)
        {
            return;
        }

        if (spawnOnStart)
        {
            BeginSpawning();
        }
        else
        {
            StopSpawningAndDespawn();
        }
    }

    private void Update()
    {
        if (!isSpawning)
        {
            return;
        }

        CleanupMissingEnemies();

        if (activeEnemies.Count >= maxEnemies)
        {
            return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemy();
        }
    }

    public void BeginSpawning()
    {
        hasBeenControlledExternally = true;
        isSpawning = true;

        // Spawn quickly after entering the field instead of waiting a full interval.
        spawnTimer = spawnInterval;

        if (logStateChanges)
        {
            Debug.Log($"{name} began spawning.");
        }
    }

    public void StopSpawningAndDespawn()
    {
        hasBeenControlledExternally = true;
        isSpawning = false;
        spawnTimer = 0f;

        DespawnAll();

        if (logStateChanges)
        {
            Debug.Log($"{name} stopped spawning and despawned enemies.");
        }
    }

    public void DespawnAll()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] != null)
            {
                Destroy(activeEnemies[i].gameObject);
            }
        }

        activeEnemies.Clear();
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null || enemyDefinition == null)
        {
            Debug.LogWarning("EnemySpawner is missing an enemy prefab or enemy definition.");
            return;
        }

        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnAreaSize.x * 0.5f, spawnAreaSize.x * 0.5f),
            Random.Range(-spawnAreaSize.y * 0.5f, spawnAreaSize.y * 0.5f),
            0f
        );

        Enemy enemy = Instantiate(enemyPrefab, transform.position + randomOffset, Quaternion.identity);
        enemy.Initialize(enemyDefinition);
        activeEnemies.Add(enemy);
    }

    private void CleanupMissingEnemies()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] == null)
            {
                activeEnemies.RemoveAt(i);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}