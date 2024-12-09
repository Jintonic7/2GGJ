using System;
using System.Diagnostics;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

public class Enemy : MonoBehaviour
{
    public float speed = 15f; // Speed of the enemy
    public Vector2 patrolAreaCenter; // Center of the patrol area
    public Vector2 patrolAreaSize; // Width and height of the patrol area
    public float detectionRange = 5f; // Detection range for the player
    public float attackRange = 2f; // Range for melee attack
    public float attackCooldown = 2f; // Cooldown between attacks
    private float timeSinceLastSeen = 0f; // Tracks time since player was last in range
    public float lostPlayerDelay = 1f;   // Delay before returning to patrolling
    private float lastAttackTime = 0f;
    private Vector3 targetPosition;
    private Rigidbody2D rb;
    private GameObject player;
    private EnemyState currentState = EnemyState.Patrolling;

    private enum EnemyState
    {
        Patrolling,
        Chasing,
        Attacking
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player"); // Assuming the player has a "Player" tag
        SetRandomPatrolPoint(); // Set the initial random patrol point
    }

    void Update()
    {
        DetectPlayer();
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;

            case EnemyState.Chasing:
                if (IsPlayerInAttackRange())
                {
                    rb.linearVelocity = Vector2.zero; // Stop the Rigidbody's velocity
                    currentState = EnemyState.Attacking;
                    UnityEngine.Debug.Log("Player in attack range. Switching to Attacking state.");
                }
                else
                {
                    TargetPlayer();
                }
                break;

            case EnemyState.Attacking:
                if (!IsPlayerInAttackRange())
                {
                    currentState = EnemyState.Chasing;
                    UnityEngine.Debug.Log("Player out of attack range. Switching to Chasing state.");
                }
                else
                {
                    AttackPlayer();
                }
                break;
        }
    }

    void DetectPlayer()
    {
        if (IsPlayerInRange())
        {
            timeSinceLastSeen = 0f; // Reset the timer if the player is in range

            if (currentState == EnemyState.Patrolling)
            {
                currentState = EnemyState.Chasing;
            }
        }
        else
        {
            timeSinceLastSeen += Time.deltaTime;

            if (timeSinceLastSeen >= lostPlayerDelay && currentState == EnemyState.Chasing)
            {
                currentState = EnemyState.Patrolling;
            }
        }
    }

    bool IsPlayerInRange()
    {
        if (player == null)
            return false;

        // Calculate the distance between the enemy and the player
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        // Check if the player is within detection range
        return distanceToPlayer <= detectionRange;
    }


    bool IsPlayerInAttackRange()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        UnityEngine.Debug.Log($"Distance to Player: {distanceToPlayer}, Attack Range: {attackRange}");
        return distanceToPlayer <= attackRange;
    }


    void TargetPlayer()
    {
        Vector2 targetPosition = new Vector2(player.transform.position.x, rb.position.y); // Keep y fixed
        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.deltaTime);
        rb.MovePosition(newPosition);
    }

    void AttackPlayer()
    {
        rb.linearVelocity = Vector2.zero; // Stop the Rigidbody's velocity
        MeleeAttack(); // Trigger the attack logic
    }

    void Patrol()
    {
        // Move toward the current target position
        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.deltaTime);
        rb.MovePosition(newPosition);

        // If the enemy has reached the target position, pick a new random point
        if (Vector2.Distance(rb.position, targetPosition) < 0.1f)
        {
            SetRandomPatrolPoint();
        }
    }

    void SetRandomPatrolPoint()
    {
        // Generate a random point within the patrol area
        float randomX = UnityEngine.Random.Range(patrolAreaCenter.x - patrolAreaSize.x / 2, patrolAreaCenter.x + patrolAreaSize.x / 2);
        targetPosition = new Vector3(randomX, rb.position.y, 0f);
    }

    void OnDrawGizmos()
    {
        // Draw the patrol area in the Scene view
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(patrolAreaCenter, patrolAreaSize);

        // Draw detection range as a circle
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

    }

    void MeleeAttack()
    {
        UnityEngine.Debug.Log("Enemy performs a melee attack!");
        // Add attack logic here, e.g., reduce player health or trigger an attack animation
        // Example:
        // player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
    }
}
