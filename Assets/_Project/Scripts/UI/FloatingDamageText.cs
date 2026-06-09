using TMPro;
using UnityEngine;

public class FloatingDamageText : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text damageText;

    [Header("Motion")]
    [SerializeField] private float lifetime = 0.7f;
    [SerializeField] private float riseSpeed = 1.5f;
    [SerializeField] private float horizontalDrift = 0.25f;

    private float timer;
    private Color originalColor;
    private Vector3 driftDirection;

    private void Awake()
    {
        if (damageText == null)
        {
            damageText = GetComponent<TMP_Text>();
        }

        if (damageText != null)
        {
            originalColor = damageText.color;
        }

        driftDirection = new Vector3(Random.Range(-horizontalDrift, horizontalDrift), 1f, 0f);
    }

    public void Initialize(int damage)
    {
        if (damageText != null)
        {
            damageText.text = damage.ToString();
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        transform.position += driftDirection * riseSpeed * Time.deltaTime;

        if (damageText != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);
            damageText.color = new Color(
                originalColor.r,
                originalColor.g,
                originalColor.b,
                alpha
            );
        }

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}