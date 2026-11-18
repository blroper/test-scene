using UnityEngine;

public class JumpController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 28f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.15f;

    private BoxCollider2D boxCollider;
    private Vector2 velocity;
    private bool isGrounded;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            velocity.y = jumpForce;
            isGrounded = false;
        }

        velocity.y -= gravity * Time.deltaTime;
        transform.position += (Vector3)(velocity * Time.deltaTime);

        CheckGround();
    }

    private void CheckGround()
    {
        if (boxCollider == null)
            return;

        float rayLength = boxCollider.bounds.extents.y + groundCheckDistance;
        RaycastHit2D hit = Physics2D.Raycast(boxCollider.bounds.center, Vector2.down, rayLength, groundLayer);

        if (hit.collider != null)
        {
            if (velocity.y <= 0f)
            {
                float groundY = hit.point.y + boxCollider.bounds.extents.y;
                transform.position = new Vector3(transform.position.x, groundY, transform.position.z);
                velocity.y = 0f;
                isGrounded = true;
            }
        }
        else
        {
            isGrounded = false;
        }
    }
}
