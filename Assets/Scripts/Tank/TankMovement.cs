using UnityEngine;

public class TankMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 180f; // Degrees per second

    private float moveInput;
    private float rotateInput;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Configure the rigidbody for top-down movement
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void Update()
    {
        // Get input
        moveInput = Input.GetAxis("Vertical");    // W/S or Up/Down
        rotateInput = Input.GetAxis("Horizontal"); // A/D or Left/Right

    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        // Rotate the tank
        float rotation = -rotateInput * rotationSpeed * Time.fixedDeltaTime;
        rb.rotation += rotation;

        // Move forward/backward in the direction the tank is facing
        Vector2 movement = transform.right * moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }
}