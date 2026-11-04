using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Drag your Player object into this slot in the Inspector
    public Transform target; 
    
    // Adjust how far away the camera should stay (e.g., X=0, Y=2, Z=-10)
    public Vector3 offset = new Vector3(0f, 2f, -10f); 
    
    // How smoothly the camera catches up (lower numbers = smoother/slower delay)
    public float smoothSpeed = 0.125f; 

    private float fixedCameraY;

    private void Start()
    {
        fixedCameraY = -93f;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate where the camera wants to go
        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            fixedCameraY + offset.y,
            target.position.z + offset.z
        );
        
        // Linearly interpolate (blend) between current position and target position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        // Move the camera to the smoothed position
        transform.position = smoothedPosition;
    }
}
