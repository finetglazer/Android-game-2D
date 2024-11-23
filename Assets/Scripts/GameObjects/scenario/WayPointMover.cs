using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainCharacter
{
    public class WaypointMover : MonoBehaviour
    {
        public GameObject target; // The GameObject to move (the player)
        public List<Transform> waypoints = new List<Transform>();
        public float moveSpeed = 2f;
        public float stopDuration = 0f; // Time to wait at each waypoint

        private int currentWaypointIndex = 0;
        private bool isMoving = false;

        private MonoBehaviour movementScript; // Reference to the existing Movement script on the player

        public void StartMoving()
        {
            if (!isMoving && waypoints.Count > 0)
            {
                if (target == null)
                {
                    Debug.LogError("Target is not assigned in WaypointMover.");
                    return;
                }

                // Find and store the Movement script component on the target
                movementScript = target.GetComponent<Movement>();
                if (movementScript != null)
                {
                    // Disable the Movement script to prevent player input
                    movementScript.enabled = false;
                }
                else
                {
                    Debug.LogWarning("Movement script not found on the target.");
                }

                StartCoroutine(MoveAlongWaypoints());
            }
        }

        private IEnumerator MoveAlongWaypoints()
        {
            isMoving = true;

            while (currentWaypointIndex < waypoints.Count)
            {
                Transform targetWaypoint = waypoints[currentWaypointIndex];

                // Move the target towards the waypoint
                while (Vector2.Distance(target.transform.position, targetWaypoint.position) > 0.1f)
                {
                    Vector2 newPosition = Vector2.MoveTowards(target.transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);
                    target.transform.position = newPosition;

                    // Face the direction of movement
                    if (target.transform.position.x < targetWaypoint.position.x)
                    {
                        TurnRight();
                    }
                    else if (target.transform.position.x > targetWaypoint.position.x)
                    {
                        TurnLeft();
                    }

                    yield return null;
                }

                // Arrived at waypoint
                target.transform.position = targetWaypoint.position;

                // Wait at the waypoint
                if (stopDuration > 0f)
                {
                    yield return new WaitForSeconds(stopDuration);
                }

                currentWaypointIndex++;
            }

            isMoving = false;

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

            // Re-enable the Movement script
            if (movementScript != null)
            {
                movementScript.enabled = true;
            }
        }

        private void TurnLeft()
        {
            // Assuming your player flips direction by changing localScale.x
            Vector3 scale = target.transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            target.transform.localScale = scale;
        }

        private void TurnRight()
        {
            Vector3 scale = target.transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            target.transform.localScale = scale;
        }
    }
}
