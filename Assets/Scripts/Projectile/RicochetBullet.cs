using UnityEngine;

public class RicochetBullet : MonoBehaviour
{
    [SerializeField] private int maxBounces = 3;
    [SerializeField] private float bulletSpeed = 20f;
    

    private int bounceCount = 0;
    private Rigidbody2D rb;
    private Vector2 lastVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 0f;
        
    }

    private void Update()
    {
        // Store the velocity before collision
        lastVelocity = rb.velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (bounceCount >= maxBounces)
        {
            Destroy(gameObject);
            return;
        }

        // Use the velocity from just before the collision
        Vector2 incomingVelocity = lastVelocity.normalized;
        Vector2 normal = collision.contacts[0].normal;

        // Calculate reflection using the actual incoming velocity
        Vector2 reflectedDirection = Vector2.Reflect(incomingVelocity, normal).normalized;

        // Set the new velocity
        rb.velocity = reflectedDirection * bulletSpeed;

        // Update rotation to match new direction
        float angle = Mathf.Atan2(reflectedDirection.y, reflectedDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
       
        

        bounceCount++;
    }

    // Optional: Draw debug lines to visualize the reflection
    private void OnDrawGizmos()
    {
        if (rb != null && rb.velocity != Vector2.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)rb.velocity.normalized);
        }
    }
}