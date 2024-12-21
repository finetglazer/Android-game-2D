using Photon.Pun;
using UnityEngine;

namespace Photon.Character
{
    public class AttackHandlerMultiplayer : MonoBehaviourPun
    {
        public float damageDealt = 1f;
        public float distanceDealDamage = 1f;

        private BoxCollider2D _playerBoxCollider;

        private void Start()
        {
            _playerBoxCollider = GetComponent<BoxCollider2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {

            if (!photonView.IsMine) return; // Only the local player can initiate attacks
            if (collision.gameObject.CompareTag("Player"))
            {
                PhotonView enemyPhotonView = collision.gameObject.GetComponentInParent<PhotonView>();
                if (enemyPhotonView != null && !enemyPhotonView.IsMine)
                {
                    Debug.LogWarning("Attacking player: " + collision.gameObject.name);
                    // Send RPC to the enemy's owner to apply damage
                    enemyPhotonView.RPC("ApplyDamage", enemyPhotonView.Owner, damageDealt);
                }
                else
                {
                    Debug.LogWarning("PhotonView not found on collided player.");
                }
            }
        }
    }
}