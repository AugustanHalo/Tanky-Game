using UnityEngine;

public class TurretRotation : MonoBehaviour
{
    [Header("Turret Settings")]
    [Tooltip("How fast the turret rotates towards the target")]
    [SerializeField] private float rotationSpeed = 5f;

    [Tooltip("If true, will smooth the rotation")]
    [SerializeField] private bool smoothRotation = true;

    [Header("Pivot Settings")]
    [Tooltip("The actual pivot point of the turret sprite")]
    [SerializeField] private Vector2 pivotOffset = Vector2.zero;

    [Tooltip("Visualize the pivot point in Scene view")]
    [SerializeField] private bool showPivotGizmo = true;

    private Camera mainCamera;
    private Vector3 mousePosition;
    private Vector2 direction;
    private float targetAngle;
    private float currentAngle;

    private void Start()
    {
        mainCamera = Camera.main;

        // If you're using a sprite renderer, you can optionally calculate the pivot offset automatically
        if (TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
        {
            // Calculate the offset based on the sprite's bounds
            Bounds bounds = spriteRenderer.sprite.bounds;
            // Example: If your pivot is on the left side of the sprite
            pivotOffset = new Vector2(bounds.extents.x, 0);
        }
    }

    private void Update()
    {
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // Calculate the world position of the actual pivot point
        Vector2 worldPivotPoint = (Vector2)transform.position + RotateVector2(pivotOffset, transform.rotation.eulerAngles.z);

        // Calculate direction from the actual pivot point to mouse position
        direction = (Vector2)mousePosition - worldPivotPoint;

        targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (smoothRotation)
        {
            currentAngle = Mathf.LerpAngle(
                transform.rotation.eulerAngles.z,
                targetAngle,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            currentAngle = targetAngle;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }

    // Helper function to rotate a vector by an angle
    private Vector2 RotateVector2(Vector2 vector, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos
        );
    }

    // Visualize the pivot point and rotation direction in the Scene view
    private void OnDrawGizmosSelected()
    {
        if (!showPivotGizmo) return;

        Vector2 worldPivotPoint = (Vector2)transform.position + RotateVector2(pivotOffset, transform.rotation.eulerAngles.z);

        // Draw pivot point
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(worldPivotPoint, 0.1f);

        // Draw direction line from pivot
        Gizmos.color = Color.red;
        Gizmos.DrawLine(worldPivotPoint, worldPivotPoint + direction.normalized * 2f);
    }
}