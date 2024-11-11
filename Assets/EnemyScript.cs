using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f; // Speed of the enemy
    public Transform pointA; // First patrol point
    public Transform pointB; // Second patrol point
    public float detectionRange = 5f; // Detection range for the player

    private Vector3 targetPosition;
    private Rigidbody2D rb;
    private GameObject player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPosition = pointA.position; // Start by moving towards point A
        player = GameObject.FindGameObjectWithTag("Player"); // Assuming the player has a "Player" tag
    }

    void Update()
    {
        if (IsPlayerInRange())
        {
            TargetPlayer();
        }
        else
        {
            Patrol();
        }
    }

    bool IsPlayerInRange()
    {
        if (player == null)
            return false;

        // Check if the player is within detection range
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        return distanceToPlayer <= detectionRange;
    }

    void TargetPlayer()
    {
        // Move towards the player's position
        Vector2 playerPosition = player.transform.position;
        Vector2 newPosition = Vector2.MoveTowards(rb.position, playerPosition, speed * Time.deltaTime);
        rb.MovePosition(newPosition);
    }

    void Patrol()
    {
        // Patrol between pointA and pointB
        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.deltaTime);
        rb.MovePosition(newPosition);

        // Check if the enemy has reached the target position
        if (Vector2.Distance(rb.position, targetPosition) < 0.1f)
        {
            // Switch target position between pointA and pointB
            targetPosition = targetPosition == pointA.position ? pointB.position : pointA.position;
        }
    }

    void OnDrawGizmos()
    {
        // Draw lines to show patrol points and detection range in the editor
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }

        // Draw detection range as a circle
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
