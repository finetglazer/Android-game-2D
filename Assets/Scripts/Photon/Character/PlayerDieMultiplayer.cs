using GameObjects.Fire;
using GameObjects.Water;
using Photon.Pun;
using Respawner;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Photon.Character
{
    public class PlayerDieNetworked : MonoBehaviourPun, IPunObservable
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

            if (photonView.IsMine)
            {
                print("Below Death Point: " + transform.position.y);
                Respawn();
            }
        }

        public void SetCheckpoint(Vector3 checkpointPosition)
        {
            if (photonView.IsMine)
            {
                _respawnPoint = checkpointPosition;
            }
        }

        public void Die()
        {
            if (photonView.IsMine)
            {
                Respawn();
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void Respawn()
        {
            DeathNote.ReRender();

            // Move the player to the last checkpoint
            transform.position = _respawnPoint;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<MovementMultiplayer>().currentHealth = 15;
            GetComponent<MovementMultiplayer>().healthBar.SetActive(true);
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            print("Player respawned at: " + _respawnPoint);

            // Handle scene-specific respawn logic
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

            // Optionally synchronize respawn across network
            // PhotonView.RPC("SyncRespawn", RpcTarget.Others, _respawnPoint);
        }

        [PunRPC]
        public void SyncRespawn(Vector3 respawnPosition)
        {
            if (!photonView.IsMine)
            {
                transform.position = respawnPosition;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<MovementMultiplayer>().currentHealth = 15;
                GetComponent<MovementMultiplayer>().healthBar.SetActive(true);
                transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                print("Player respawned at: " + _respawnPoint);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Send respawn position if needed
                stream.SendNext(_respawnPoint);
            }
            else
            {
                // Receive respawn position if needed
                _respawnPoint = (Vector3)stream.ReceiveNext();
            }
        }
    }
}
