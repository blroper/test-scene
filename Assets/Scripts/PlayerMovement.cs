using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody playerRb;
    public InputAction jumpAction;
    public InputAction duckAction;
    public float jumpForce = 10;
    public float duckForce = 10;
    public float gravityModifier;
    public bool isOnGround = true;
    public bool gameOver = false;
    private Animator playerAnim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        jumpAction.Enable();
        Physics.gravity *= gravityModifier;
        playerAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (jumpAction.triggered && isOnGround){
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;
            playerAnim.SetTrigger("Jump_trig");
        }
        if (duckAction.triggered && isOnGround){
            playerRb.AddForce(Vector3.down * duckForce, ForceMode.Impulse);
            isOnGround = false;
            playerAnim.SetTrigger("Duck_trig");
        }
    }
}
