using UnityEngine;
using MainCharacter; // Make sure to include the Cinemachine namespace

namespace GameObjects.Checkpoint
{
    public class Checkpoint : MonoBehaviour
    {
        public float offset;
        // public CinemachineVirtualCamera targetCamera; // Reference to the target virtual camera

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if the player touched the checkpoint
            if (!other.CompareTag("Player")) return;

            var player = other.GetComponent<PlayerDie>();

            // Set the player's respawn position relative to the checkpoint
            player.SetCheckpoint(new Vector3(transform.position.x + offset, transform.position.y, transform.position.z));

            print("Checkpoint activated at: " + transform.position);
        }
        
    }
}
