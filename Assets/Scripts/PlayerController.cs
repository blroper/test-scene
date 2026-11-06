using UnityEngine;
using UnityEngine.InputSystem;

/// Basic 2D platformer character controller
/// 2026 Owen Mundy
///
/// SETUP
/// 1. Add this script to your player. A Rigidbody2D is added automatically.
/// 2. Give the player a Collider2D, and freeze Rotation Z on the Rigidbody2D.
/// 3. Create an empty child object at the player's feet and drag it into "Ground Check".
///    Select the player to see the ground check as a yellow circle in the Scene view.
/// 4. Create a layer named "Ground" and put your floors and platforms on it.
///    Keep the player on a different layer. Set "Ground Layer" on this script to Ground.
///
/// Controls: arrow keys to move, up to jump, down to duck. You can change the keys
/// in the "Input" section of this script in the Inspector.
///
/// ANIMATOR (optional)
/// If your character has an Animator, its controller needs these parameters,
/// spelled exactly like this:
///   Speed (Float), IsJumping (Bool), IsDucking (Bool)
///
/// IF SOMETHING ISN'T WORKING
/// - Check the Console first. This script tells you about common setup mistakes.
/// - Can't jump? Make sure the yellow circle touches the ground when the player stands on it.
/// - Walks but can't jump, even though the circle touches the ground? The player may be
///   stuck inside a solid collider, like a large camera bounds box. Tick "Is Trigger" on
///   any collider that the player should pass through.
/// - Character faces backwards? This script assumes your art faces right.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb2d; // Moves the player. Found automatically if left empty.
    public Animator animator; // Optional. Found automatically if left empty.
    public Transform groundCheck; // An empty child object at the player's feet
    public LayerMask groundLayer; // The layer(s) the player can stand on

    [Header("Input")]
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction duckAction;

    [Header("Movement")]
    public float runSpeed = 6f; // Top speed, in units per second

    [Range(0, .3f)]
    public float movementSmoothing = .05f; // Seconds to reach top speed. 0 is instant.
    public bool airControl = true; // Can the player steer while in the air?

    [Header("Jump")]
    public float jumpForce = 6.2f; // Upward speed at takeoff. Higher jumps higher.
    public float fallMultiplier = 2.5f; // Gravity is multiplied by this while falling
    public float lowJumpMultiplier = 3f; // Gravity is multiplied by this if jump is released early. 1 makes every jump full height.
    public float rememberGroundedFor = 0.1f; // Seconds the player can still jump after walking off a ledge

    [Header("Status (for checking, not for editing)")]
    public bool isGrounded; // Is the player standing on the ground right now?

    const float groundCheckRadius = .2f; // Size of the circle used to look for ground

    float moveX; // Left/right input, from -1 (left) to 1 (right)
    float xSmoothVelocity; // Used by SmoothDamp to remember how fast the speed is changing
    float lastTimeGrounded; // The last time the player was on the ground
    float baseGravity; // The Gravity Scale set on the Rigidbody2D, before any jump changes
    bool jumpQueued; // Was jump pressed, and is it waiting for FixedUpdate?
    bool isJumping; // Has the player jumped and not landed yet? Prevents double jumps.

    // Unity runs this when the script is first added, or when you choose Reset in the
    // Inspector. It gives the player working keys straight away.
    void Reset()
    {
        moveAction = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
        moveAction
            .AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        jumpAction = new InputAction("Jump", InputActionType.Button, "<Keyboard>/upArrow");
        duckAction = new InputAction("Duck", InputActionType.Button, "<Keyboard>/downArrow");
    }

    // Runs once when the game starts, before anything else in this script
    void Awake()
    {
        // Find any references that were left empty
        if (rb2d == null)
            rb2d = GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        baseGravity = rb2d.gravityScale;
        CheckSetup();
    }

    // Input actions must be turned on before they can be read
    void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        duckAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        duckAction.Disable();
    }

    // Update runs once per rendered frame. Use it for reading input and for animation.
    void Update()
    {
        moveX = moveAction.ReadValue<Vector2>().x;

        // A key press only lasts one frame, and FixedUpdate doesn't run on every frame.
        // Remembering the press here means FixedUpdate can't miss it.
        if (jumpAction.WasPressedThisFrame())
            jumpQueued = true;

        UpdateAnimator();
    }

    // FixedUpdate runs at a steady rate, in step with the physics. Use it for moving a Rigidbody.
    void FixedUpdate()
    {
        // Look for ground in a small circle at the player's feet
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
            lastTimeGrounded = Time.time;

        // Copy the velocity, change the copy, then apply it at the end
        Vector2 velocity = rb2d.linearVelocity;

        // The player has landed once they're on the ground and no longer moving up.
        // Right after takeoff the circle still touches the ground, but the player is
        // moving up, so this doesn't count as landing.
        if (isGrounded && velocity.y <= 0)
            isJumping = false;

        // Run: ease toward top speed, unless the player is in the air without air control
        if (isGrounded || airControl)
        {
            velocity.x = Mathf.SmoothDamp(
                velocity.x,
                moveX * runSpeed,
                ref xSmoothVelocity,
                movementSmoothing
            );
            FaceDirection();
        }

        // Jump: allowed if the player hasn't already jumped, and is on the ground
        // or walked off a ledge a moment ago
        bool wasJustGrounded = Time.time - lastTimeGrounded <= rememberGroundedFor;
        if (jumpQueued && !isJumping && wasJustGrounded)
        {
            velocity.y = jumpForce;
            isJumping = true;
        }
        jumpQueued = false; // The press has been handled, whether or not the player jumped

        rb2d.linearVelocity = velocity;
        rb2d.gravityScale = baseGravity * GravityMultiplier(velocity.y);
    }

    // Changing gravity during a jump makes it feel less floaty:
    // the player falls faster, and a quick tap gives a shorter jump than a long press
    float GravityMultiplier(float yVelocity)
    {
        if (yVelocity < 0)
            return fallMultiplier; // Falling
        if (yVelocity > 0 && !jumpAction.IsPressed())
            return lowJumpMultiplier; // Rising, but jump was released early
        return 1f; // Normal gravity
    }

    // Flip the character to face the way the player is pressing.
    // A negative X scale mirrors the character and all of its child objects.
    void FaceDirection()
    {
        if (moveX == 0)
            return; // No input, so keep facing the same way

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(moveX);
        transform.localScale = scale;
    }

    // Send the player's state to the Animator, if there is one
    void UpdateAnimator()
    {
        if (animator == null)
            return;

        animator.SetFloat("Speed", Mathf.Abs(moveX));
        animator.SetBool("IsJumping", !isGrounded);
        animator.SetBool("IsDucking", isGrounded && duckAction.IsPressed());
    }

    // Explain common setup mistakes in the Console, and stop this script until they're fixed
    void CheckSetup()
    {
        if (groundCheck == null)
        {
            Debug.LogError("PlayerController: drag an empty child object at the player's feet into Ground Check.", this);
            enabled = false;
            return;
        }

        if (groundLayer == 0)
        {
            Debug.LogError("PlayerController: choose your ground layer in Ground Layer.", this);
            enabled = false;
            return;
        }

        // Is the player's own layer included in Ground Layer?
        // If so, the player would count as standing on themselves.
        if ((groundLayer & (1 << gameObject.layer)) != 0)
        {
            Debug.LogError("PlayerController: the player can't be on a layer that's included in Ground Layer. Move the player to a different layer.", this);
            enabled = false;
        }
    }

    // Draws the ground check circle in the Scene view when the player is selected
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
