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
                Die();
            }
        }

        public void Die()
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.LoadLevel("EndingScene");
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
