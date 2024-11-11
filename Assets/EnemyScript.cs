using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f; // Speed of the enemy
    public Transform pointA; // First point to patrol to
    public Transform pointB; // Second point to patrol to

    private Vector3 targetPosition;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPosition = pointA.position;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        // Move towards the target position in both x and y directions
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
        // Draw lines to show patrol points in the editor
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
