using UnityEngine;

namespace MainCharacter
{
    public class PlayerDie45 : MonoBehaviour
    {
        public float deathPoint;
        private Vector3 respawnPoint;

        // Reference to the current checkpoint the player is at
        private CheckPointWithTmpTexture currentCheckPointWithTmpTexture;

        private void Start()
        {
            // Set the default respawn point to the player's starting position
            respawnPoint = transform.position;
        }

        private void Update()
        {
            if (transform.position.y < deathPoint)
            {
                Debug.Log("Player fell below death point");
                Respawn();
            }
        }

        public void SetCheckpoint(Vector3 checkpointPosition, CheckPointWithTmpTexture checkPointWithTmpTexture)
        {
            // Set the respawn point to the checkpoint's position
            respawnPoint = checkpointPosition;
            currentCheckPointWithTmpTexture = checkPointWithTmpTexture;
        }
       

        public void Die()
        {
            Respawn();
        }

        private void Respawn()
        {
            // Move the player to the last checkpoint
            transform.position = respawnPoint;
            Debug.Log("Player respawned at: " + respawnPoint);
            print(currentCheckPointWithTmpTexture == null);

            // Reset all temporary textures related to the current checkpoint
            if (currentCheckPointWithTmpTexture != null)
            {
                Debug.Log("Resetting textures");
                currentCheckPointWithTmpTexture.ResetTextures();
            }
        }
    }
}