using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // The object the camera will follow.
    [SerializeField] private Rigidbody2D tankRigidbody;

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 0, -10); // Offset (keep Z at -10 for 2D cameras).
    public float smoothSpeed = 0.125f; // Smoothing factor for the movement.

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow2D: Target not assigned!");
            return;
        }


        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

    }
}
