using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Enemy enemy;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Canvas healthBarCanvas;

    [Header("Visibility")]
    [SerializeField] private bool hideWhenFull = true;

    private Camera mainCamera;

    private void Awake()
    {
        if (enemy == null)
        {
            enemy = GetComponentInParent<Enemy>();
        }

        if (healthSlider == null)
        {
            healthSlider = GetComponentInChildren<Slider>();
        }

        if (healthBarCanvas == null)
        {
            healthBarCanvas = GetComponentInChildren<Canvas>();
        }

        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        if (enemy != null)
        {
            enemy.HealthChanged += HandleHealthChanged;
        }
    }

    private void OnDisable()
    {
        if (enemy != null)
        {
            enemy.HealthChanged -= HandleHealthChanged;
        }
    }

    private void Start()
    {
        if (enemy != null)
        {
            HandleHealthChanged(enemy.CurrentHealth, enemy.MaxHealth);
        }
    }

    private void LateUpdate()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null && healthBarCanvas != null)
        {
            healthBarCanvas.transform.rotation = mainCamera.transform.rotation;
        }
    }

    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        if (healthSlider == null)
        {
            return;
        }

        healthSlider.maxValue = maxHealth;
        healthSlider.value = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (hideWhenFull)
        {
            bool shouldShow = currentHealth < maxHealth && currentHealth > 0;
            healthSlider.gameObject.SetActive(shouldShow);
        }
    }
}