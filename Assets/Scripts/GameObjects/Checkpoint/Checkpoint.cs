using UnityEngine;
using MainCharacter; // Make sure to include the Cinemachine namespace

namespace GameObjects.Checkpoint
{
    public class Checkpoint : MonoBehaviour
    {
        public float offset;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            
            // Get the Player script and set the respawn position to this checkpoint
            var player = other.GetComponent<PlayerDie>();
            if (player is null) return;

            // Set the player's respawn position relative to the checkpoint
            player.SetCheckpoint(new Vector3(transform.position.x + offset, transform.position.y, transform.position.z));

            // Switch to the target camera
            // SwitchToTargetCamera();

            print("Checkpoint activated at: " + transform.position);
        }
        
        // private void SwitchToTargetCamera()
        // {
        //     // Disable all other virtual cameras and enable the target camera
        //     CinemachineVirtualCamera[] allCameras = FindObjectsOfType<CinemachineVirtualCamera>();
        //
        //     foreach (var cam in allCameras)
        //     {
        //         cam.Priority = cam == targetCamera ? 10 : 0; // Set target camera priority higher to activate it
        //     }
        // }
    }
}
