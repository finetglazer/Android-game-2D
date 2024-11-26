using GameObjects.Fire;
using GameObjects.Water;
using Respawner;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            GetComponent<Movement>().currentHealth = 15;
            GetComponent<Movement>().healthBar.SetActive(true);
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            print("Player respawned at: " + _respawnPoint);
            switch (SceneManager.GetActiveScene().name)
            {
                case "6thScene":
                    if (FireAccelerationAndCauseDamage.FireIsOngoing == false)
                    {
                        break;
                    }
                    var fireWall = GameObject.Find("FireWalls");
                    var playerPosition = gameObject.transform.position;
                    fireWall.SetActive(true);
                    fireWall.GetComponent<GameObjects.Fire.Movement>().currentSpeed = 0.005f;
                    fireWall.transform.position = new Vector2(playerPosition.x - 5f, playerPosition.y);
                    break;
                case "9thscene":
                    if (WaterAccelerationAndCauseDamage.WaterIsOngoing == false)
                    {
                        break;
                    }

                    var water = GameObject.Find("Water");
                    playerPosition = gameObject.transform.position;
                    water.SetActive(true);
                    water.GetComponent<GameObjects.Water.Movement>().currentSpeed = 0.01f;
                    water.transform.position = new Vector2(playerPosition.x, playerPosition.y - 100f);
                    print(water.transform.position.y);
                    break;
            }
        }
    }
}