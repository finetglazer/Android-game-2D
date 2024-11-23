using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using UnityEngine;

namespace GameObjects.scenario
{
    public class WaypointMover : MonoBehaviour
    {
        public List<Transform> waypoints = new List<Transform>();
        public float moveSpeed;
        public float stopDuration = 0f; // Time to wait at each waypoint

        private GameObject target; // The player GameObject
        private int currentWaypointIndex = 0;
        private bool isMoving = false;

        private MonoBehaviour movementScript; // Reference to the Movement script
        private Rigidbody2D rb; // Player's Rigidbody2D
        private Animator animator;

        private float originalGravityScale; // Store original gravity scale
        private bool originalIsKinematic;   // Store original kinematic state

        public void StartMoving()
        {
            if (!isMoving && waypoints.Count > 0)
            {
                // Find the player at runtime
                target = GameObject.FindGameObjectWithTag("Player");
                if (target == null)
                {
                    Debug.LogError("Player not found in the scene. Make sure the player has the 'Player' tag.");
                    return;
                }

                // Get components
                movementScript = target.GetComponent<Movement>();
                rb = target.GetComponent<Rigidbody2D>();
                animator = target.GetComponent<Animator>();

                if (movementScript != null)
                {
                    // Disable the Movement script to prevent player input
                    movementScript.enabled = false;
                }
                else
                {
                    Debug.LogWarning("Movement script not found on the player.");
                }

                if (rb != null)
                {
                    // Stop any existing movement
                    rb.velocity = Vector2.zero;

                    // Store original physics properties
                    originalGravityScale = rb.gravityScale;
                    originalIsKinematic = rb.isKinematic;

                    // Disable gravity and set kinematic during movement
                    rb.gravityScale = 0;
                    rb.isKinematic = true;
                }
                else
                {
                    Debug.LogWarning("Rigidbody2D component not found on the player.");
                }

                StartCoroutine(MoveAlongWaypoints());
            }
        }

        private IEnumerator MoveAlongWaypoints()
        {
            isMoving = true;

            // Start walking animation
            if (animator != null)
            {
                animator.ResetTrigger("idle");
                animator.SetTrigger("walk");
            }

            while (currentWaypointIndex < waypoints.Count)
            {
                Transform targetWaypoint = waypoints[currentWaypointIndex];

                // Calculate direction to determine facing
                Vector2 direction = (targetWaypoint.position - target.transform.position).normalized;

                // Face the direction of movement
                if (direction.x > 0)
                {
                    TurnRight();
                }
                else if (direction.x < 0)
                {
                    TurnLeft();
                }

                // Move towards the waypoint
                while (Vector2.Distance(target.transform.position, targetWaypoint.position) > 0.1f)
                {
                    // Calculate new position using Rigidbody2D.MovePosition
                    Vector2 newPosition = Vector2.MoveTowards(rb.position, targetWaypoint.position, moveSpeed * Time.fixedDeltaTime);
                    rb.MovePosition(newPosition);

                    yield return new WaitForFixedUpdate(); // Wait for the next physics update
                }

                // Arrived at waypoint
                rb.position = targetWaypoint.position;

                // Wait at the waypoint
                if (stopDuration > 0f)
                {
                    // Set idle animation while waiting
                    if (animator != null)
                    {
                        animator.ResetTrigger("walk");
                        animator.SetTrigger("idle");
                    }

                    yield return new WaitForSeconds(stopDuration);

                    // Resume walking animation if there are more waypoints
                    if (animator != null && currentWaypointIndex + 1 < waypoints.Count)
                    {
                        animator.ResetTrigger("idle");
                        animator.SetTrigger("walk");
                    }
                }

                currentWaypointIndex++;
            }

            isMoving = false;

            // Restore physics properties
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.gravityScale = originalGravityScale;
                rb.isKinematic = originalIsKinematic;
            }

            // Set idle animation
            if (animator != null)
            {
                animator.ResetTrigger("walk");
                animator.SetTrigger("idle");
            }

            // Re-enable the Movement script
            if (movementScript != null)
            {
                movementScript.enabled = true;
            }

            // Reset waypoint index for next time
            currentWaypointIndex = 0;
        }

        public void StopMoving()
        {
            StopCoroutine(MoveAlongWaypoints());
            isMoving = false;

            // Restore physics properties
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.gravityScale = originalGravityScale;
                rb.isKinematic = originalIsKinematic;
            }

            // Re-enable the Movement script
            if (movementScript != null)
            {
                movementScript.enabled = true;
            }

            // Set idle animation
            if (animator != null)
            {
                animator.ResetTrigger("walk");
                animator.SetTrigger("idle");
            }
        }

        private void TurnRight()
        {
            // Adjust to match your character's facing direction
            Vector3 scale = target.transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            target.transform.localScale = scale;
        }

        private void TurnLeft()
        {
            Vector3 scale = target.transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            target.transform.localScale = scale;
        }
    }
}
