using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;
    private float moveInput = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        // CODE-BASED FIX: Automatically creates a frictionless material 
        // and applies it directly to the player's box collider.
        BoxCollider myCollider = GetComponent<BoxCollider>();
        if (myCollider != null)
        {
            PhysicsMaterial customMat = new PhysicsMaterial("FrictionlessCode");
            customMat.dynamicFriction = 0f;
            customMat.staticFriction = 0f;
            customMat.frictionCombine = PhysicsMaterialCombine.Minimum;
            
            myCollider.material = customMat;
        }
    }


    private void Update()
    {
        // 1. Gather player input
        moveInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            moveInput = -1f;  // Move left
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            moveInput = 1f;   // Move right
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            // 2. Apply velocity safely to move the player on the X axis
            rb.linearVelocity = new Vector3(moveInput * speed, rb.linearVelocity.y, rb.linearVelocity.z);
        }
    }
}
