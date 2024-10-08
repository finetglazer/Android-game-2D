using Recorder;
using UnityEngine;

namespace MainCharacter
{
    public class PlayerDie12 : MonoBehaviour
    {
        public float deathPoint;
        private Vector3 _respawnPoint;

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

        public void SetCheckpoint(Vector3 checkpointPosition)
        {   
            // Set the respawn point to the checkpoint's position
            _respawnPoint = checkpointPosition;
        }

        private void Respawn()
        {
            // Re-render enemies
            DeathNote.ReRender();
            
            // Move the player to the last checkpoint
            transform.position = _respawnPoint;
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            print("Player respawned at: " + _respawnPoint);
        }
    }
}
