using UnityEngine;

public class FloatingDamageSpawner : MonoBehaviour
{
    public static FloatingDamageSpawner Instance { get; private set; }

    [Header("Prefab")]
    [SerializeField] private FloatingDamageText floatingDamageTextPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SpawnDamageNumber(int damage, Vector3 worldPosition)
    {
        if (floatingDamageTextPrefab == null)
        {
            Debug.LogWarning("FloatingDamageSpawner is missing a floating damage text prefab.");
            return;
        }

        FloatingDamageText textInstance = Instantiate(
            floatingDamageTextPrefab,
            worldPosition,
            Quaternion.identity
        );

        textInstance.Initialize(damage);
    }
}