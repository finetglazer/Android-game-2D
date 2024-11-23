using MainCharacter;
using UnityEngine;

namespace GameObjects.scenario
{
    public class WaypointTrigger : MonoBehaviour
    {
        public WaypointMover waypointMover; // Reference to the WaypointMover in the scene

        private bool hasTriggered = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!hasTriggered && collision.CompareTag("Player"))
            {
                hasTriggered = true;

                // Start the movement
                if (waypointMover != null)
                {
                    waypointMover.StartMoving();
                }
                else
                {
                    Debug.LogError("WaypointMover is not assigned in WaypointTrigger.");
                }

                // Optionally, disable the trigger to prevent re-triggering
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}