using UnityEngine;
using UnityEngine.InputSystem;

public class MoveController : MonoBehaviour
{   public float speed = 5f;

    void Update()
    {
        float moveInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveInput = 1f;   // Move background right
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveInput = -1f;  // Move background left
        }

        transform.Translate(Vector3.right * moveInput * speed * Time.deltaTime);
    }
}
