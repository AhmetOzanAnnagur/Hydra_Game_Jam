using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 input;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool wasWalkingUp = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        HandleTurnIntoBack();
        HandleFlip();
        HandleAnimation();
    }

    void FixedUpdate()
    {
        rb.MovePosition(
            rb.position + input * moveSpeed * Time.fixedDeltaTime
        );
    }

    void HandleTurnIntoBack()
    {
        bool isWalkingUp = input.y > 0.1f;

        // Fire Turn ONLY when we START walking up
        if (isWalkingUp && !wasWalkingUp)
        {
            animator.SetTrigger("Turn");
        }

        wasWalkingUp = isWalkingUp;
        animator.SetBool("IsWalkingUp", isWalkingUp);
    }

    void HandleFlip()
    {
        if (input.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (input.x < -0.01f)
            spriteRenderer.flipX = true;
    }

    void HandleAnimation()
    {
        bool isRunning = input.sqrMagnitude > 0.01f;
        animator.SetBool("IsRunning", isRunning);
    }
}
