using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.25f;
    [SerializeField] private float stopDistance = 1.1f;
    [SerializeField] private bool moveOnlyOnX = true;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool flipSpriteBasedOnDirection = true;

    private Rigidbody2D rb;
    private Enemy enemy;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Start()
    {
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                target = playerObject.transform;
            }
            else
            {
                Debug.LogWarning($"{name} could not find a GameObject tagged Player.");
            }
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (enemy != null && !enemy.IsAlive)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = target.position;

        if (moveOnlyOnX)
        {
            targetPosition.y = currentPosition.y;
        }

        float distance = Vector2.Distance(currentPosition, targetPosition);

        if (distance <= stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = (targetPosition - currentPosition).normalized;
        rb.linearVelocity = direction * moveSpeed;

        UpdateFacing(direction);
    }

    private void UpdateFacing(Vector2 direction)
    {
        if (!flipSpriteBasedOnDirection || spriteRenderer == null)
        {
            return;
        }

        if (Mathf.Abs(direction.x) < 0.01f)
        {
            return;
        }

        spriteRenderer.flipX = direction.x < 0f;
    }
}