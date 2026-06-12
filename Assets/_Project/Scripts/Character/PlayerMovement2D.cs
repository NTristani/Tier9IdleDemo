using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private bool canMove = true;

    [Header("Bounds")]
    [SerializeField] private bool useXBounds = true;
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool flipSpriteBasedOnDirection = true;

    private Rigidbody2D rb;
    private float horizontalInput;

    public bool CanMove => canMove;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Update()
    {
        if (!canMove)
        {
            horizontalInput = 0f;
            return;
        }

        horizontalInput = GetHorizontalInput();

        if (flipSpriteBasedOnDirection && spriteRenderer != null)
        {
            if (horizontalInput < -0.01f)
            {
                spriteRenderer.flipX = true;
            }
            else if (horizontalInput > 0.01f)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            StopRigidbody();
            return;
        }

        Vector2 currentPosition = rb != null
            ? rb.position
            : transform.position;

        float targetX = currentPosition.x + horizontalInput * moveSpeed * Time.fixedDeltaTime;

        if (useXBounds)
        {
            targetX = Mathf.Clamp(targetX, minX, maxX);
        }

        Vector2 targetPosition = new Vector2(targetX, currentPosition.y);

        if (rb != null)
        {
            rb.MovePosition(targetPosition);
        }
        else
        {
            transform.position = targetPosition;
        }
    }

    public void SetCanMove(bool value)
    {
        canMove = value;

        if (!canMove)
        {
            horizontalInput = 0f;
            StopRigidbody();
        }
    }

    public void SetXBounds(float newMinX, float newMaxX)
    {
        minX = Mathf.Min(newMinX, newMaxX);
        maxX = Mathf.Max(newMinX, newMaxX);

        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, minX, maxX);
        transform.position = position;
    }

    private float GetHorizontalInput()
    {
        if (Keyboard.current == null)
        {
            return 0f;
        }

        float input = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            input -= 1f;
        }

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            input += 1f;
        }

        return Mathf.Clamp(input, -1f, 1f);
    }

    private void StopRigidbody()
    {
        if (rb == null)
        {
            return;
        }

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}