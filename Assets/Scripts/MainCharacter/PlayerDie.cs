using Respawner;
using UnityEngine;

namespace MainCharacter
{
    public class PlayerDie : MonoBehaviour
    {
        public float deathPoint;
        private Vector3 _respawnPoint;

        private void Start()
        {
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
            _respawnPoint = checkpointPosition;
        }

        public void Die()
        {
            Respawn();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void Respawn()
        {
            DeathNote.ReRender();
            
            // Move the player to the last checkpoint
            transform.position = _respawnPoint;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Movement>().currentHealth = 1;
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            print("Player respawned at: " + _respawnPoint);
        }
    }
}