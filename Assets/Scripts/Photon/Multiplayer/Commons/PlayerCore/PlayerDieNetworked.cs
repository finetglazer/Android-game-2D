using Photon.Pun;
using Photon.Solo.Characters.Knight;
using UnityEngine;

namespace Photon.Multiplayer.Commons.PlayerCore
{
    public class PlayerDieNetworked : MonoBehaviourPun, IPunObservable
    {
        public float deathPoint = -15f;
        private Vector3 _respawnPoint;

        private MovementSoloPlayer _movementSoloPlayer;

        private void Start()
        {
            _respawnPoint = transform.position;
            _movementSoloPlayer = GetComponent<MovementSoloPlayer>();
            if (_movementSoloPlayer == null)
            {
                Debug.LogError("MovementMultiplayer component not found on the player.");
            }
        }

        private void Update()
        {
            if (!_movementSoloPlayer) return;

            if (_movementSoloPlayer.currentHealth <= 0f)
            {
                if (photonView.IsMine)
                {
                    Die();
                }
            }

            // Optional: Handle death based on position
            if (transform.position.y < deathPoint)
            {
                if (photonView.IsMine)
                {
                    Die();
                }
            }
        }

        public void Die()
        {
            if (photonView.IsMine)
            {
                // Notify all clients to load the "EndingScene"
                photonView.RPC("LoadScene", RpcTarget.All);
            }
        }

        [PunRPC]
        private void LoadScene()
        {
            // Optionally, add delay or effects before loading the scene
            PhotonNetwork.LoadLevel("EndingScene");
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
