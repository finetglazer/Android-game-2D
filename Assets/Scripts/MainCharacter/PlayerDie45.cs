using GameObjects.Checkpoint;
using Recorder;
using UnityEngine;

namespace MainCharacter
{
    public class PlayerDie45 : MonoBehaviour
    {
        public float deathPoint;
        private Vector3 _respawnPoint;

        // Reference to the current checkpoint the player is at

        private void Start()
        {
            // Set the default respawn point to the player's starting position
            _respawnPoint = transform.position;
        }

        private void Update()
        {
            if (!(transform.position.y < deathPoint)) return;
            
            print("Below Death Point: " + transform.position.y);
            Respawn();
        }

        public void SetCheckpoint(Vector3 checkpointPosition, CheckPointWithTmpTexture checkPointWithTmpTexture)
        {
            // Set the respawn point to the checkpoint's position
            _respawnPoint = checkpointPosition;
        }
        
        public void Die()
        {
            Respawn();
        }

        private void Respawn()
        {
            // Move the player to the last checkpoint
            transform.position = _respawnPoint;
            print("Player respawned at: " + _respawnPoint);

            DeathNote.ReRender();
        }
    }
}