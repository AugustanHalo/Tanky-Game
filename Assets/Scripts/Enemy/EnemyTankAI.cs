using UnityEngine;
using System.Collections;

public class EnemyTankAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TankMovement movement;
    [SerializeField] private TankShoot shooter;
    [SerializeField] private Transform turretChild; // Reference to the child turret transform

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float shootingRange = 8f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float turretRotationSpeed = 5f; // Speed at which the turret rotates

    [Header("Movement Settings")]
    [SerializeField] private float patrolSpeed = 3f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float minPatrolDistance = 5f;
    [SerializeField] private float maxPatrolDistance = 15f;

    private Transform player;
    private Vector2 patrolTarget;
    private bool isWaiting;
    private bool canSeePlayer;
    private Rigidbody2D rb;
    private float currentTurretAngle;

    private enum TankState
    {
        Patrol,
        Chase,
        Attack
    }

    private TankState currentState;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        SetNewPatrolTarget();
        movement.enabled = false; // Disable the default tank movement

        // Initialize turret rotation
        if (turretChild != null)
        {
            currentTurretAngle = turretChild.localRotation.eulerAngles.z;
        }
    }

    private void Update()
    {
        if (player == null) return;

        CheckPlayerVisibility();
        UpdateState();
        UpdateTurretRotation();
    }

    private void FixedUpdate()
    {
        ExecuteCurrentState();
    }

    private void CheckPlayerVisibility()
    {
        if (player == null) return;

        Vector2 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= detectionRange)
        {
            // Check if there are obstacles between tank and player
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                directionToPlayer.normalized,
                distanceToPlayer,
                obstacleLayer
            );

            canSeePlayer = hit.collider == null;
        }
        else
        {
            canSeePlayer = false;
        }
    }

    private void UpdateState()
    {
        if (!canSeePlayer)
        {
            currentState = TankState.Patrol;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= shootingRange)
        {
            currentState = TankState.Attack;
        }
        else
        {
            currentState = TankState.Chase;
        }
    }

    private void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case TankState.Patrol:
                Patrol();
                break;
            case TankState.Chase:
                ChasePlayer();
                break;
            case TankState.Attack:
                Attack();
                break;
        }
    }

    private void Patrol()
    {
        if (isWaiting) return;

        float distanceToTarget = Vector2.Distance(transform.position, patrolTarget);

        if (distanceToTarget < 0.5f)
        {
            StartCoroutine(WaitAtPatrolPoint());
            return;
        }

        MoveToTarget(patrolTarget, patrolSpeed);
    }

    private void ChasePlayer()
    {
        if (player == null) return;
        MoveToTarget(player.position, chaseSpeed);
    }

    private void Attack()
    {
        if (player == null) return;

        // Stop moving when attacking
        rb.velocity = Vector2.zero;

        // Check if player is in front of the tank
        Vector2 directionToPlayer = player.position - transform.position;
        float angle = Vector2.Angle(transform.right, directionToPlayer);

        if (angle < 30f && canSeePlayer)
        {
            shooter.Shoot();
        }
    }

    private void MoveToTarget(Vector2 target, float speed)
    {
        Vector2 directionToTarget = target - rb.position;
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

        // Rotate towards target
        float currentRotation = rb.rotation;
        float newRotation = Mathf.MoveTowardsAngle(currentRotation, -angle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(newRotation);

        // Move forward if facing roughly the right direction
        if (Mathf.Abs(Mathf.DeltaAngle(-angle, currentRotation)) < 30f)
        {
            Vector2 movement = transform.right * speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
        }
    }

    private void UpdateTurretRotation()
    {
        if (player == null || turretChild == null) return;

        // Calculate the direction to the player in world space
        Vector2 directionToPlayer = player.position - transform.position;

        // Calculate the desired angle in world space
        float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        // Convert to local rotation by subtracting the tank's rotation
        float localTargetAngle = targetAngle - transform.eulerAngles.z;

        // Normalize the angle to be between -180 and 180 degrees
        localTargetAngle = Mathf.DeltaAngle(0, localTargetAngle);

        // Smoothly rotate the turret
        currentTurretAngle = Mathf.MoveTowardsAngle(
            currentTurretAngle,
            localTargetAngle,
            turretRotationSpeed * Time.deltaTime
        );

        // Apply the rotation locally
        turretChild.localRotation = Quaternion.Euler(0f, 0f, currentTurretAngle);
    }

    private void SetNewPatrolTarget()
    {
        float randomDistance = Random.Range(minPatrolDistance, maxPatrolDistance);
        float randomAngle = Random.Range(0f, 360f);
        Vector2 randomDirection = Quaternion.Euler(0f, 0f, randomAngle) * Vector2.right;

        patrolTarget = (Vector2)transform.position + randomDirection * randomDistance;
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(patrolWaitTime);
        SetNewPatrolTarget();
        isWaiting = false;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw shooting range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootingRange);

        // Draw patrol target
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(patrolTarget, 0.5f);
        }
    }
}