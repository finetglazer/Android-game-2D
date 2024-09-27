using UnityEngine;

namespace MainCharacter
{
    
    
    public class Checkpoint : MonoBehaviour
    {
        public float offset = 0;
    
        // set up a reference to the player
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if the player touched the checkpoint
            if (other.CompareTag("Player"))
            {
                // Get the Player script and set the respawn position to this checkpoint
                PlayerDie player = other.GetComponent<PlayerDie>();
                if (player != null)
                {
                    // set transform position -10 x compared to the checkpoint
                    // player.SetCheckpoint(transform.position);
                    player.SetCheckpoint(new Vector3(transform.position.x + offset , transform.position.y, transform.position.z));
                    Debug.Log("Checkpoint activated at: " + transform.position);
                }
            }
        }
    }
}