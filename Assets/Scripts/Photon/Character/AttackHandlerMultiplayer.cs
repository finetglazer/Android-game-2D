using Photon.Pun;
using UnityEngine;

namespace Photon.Character
{
    public class AttackHandlerMultiplayer : MonoBehaviourPun
    {
        private static readonly int Hurt = Animator.StringToHash("hurt");
        public float damageDealt = 1f;
        public float distanceDealDamage = 1f;
        private GameObject _enemy;

        private BoxCollider2D _playerBoxCollider;

        private void Start()
        {
            _playerBoxCollider = GetComponent<BoxCollider2D>();
        }

        private bool EnemyIsInDamageDealtDistance()
        {
            var raycastHit = Physics2D.BoxCast(_playerBoxCollider.bounds.center, _playerBoxCollider.bounds.size, 0, new Vector2(-Mathf.Sign(transform.localScale.x), 0), distanceDealDamage, LayerMask.GetMask("Player"));
            if (raycastHit.collider is not null) _enemy = raycastHit.collider.gameObject;
            return raycastHit.collider is not null;
        }

        // private void OnCollisionEnter2D(Collision2D collision)
        // {
        //
        //     if (!photonView.IsMine) return; // Only the local player can initiate attacks
        //     if (collision.gameObject.CompareTag("Player"))
        //     {
        // PhotonView enemyPhotonView = collision.gameObject.GetComponentInParent<PhotonView>();
        // if (enemyPhotonView != null && !enemyPhotonView.IsMine)
        // {
        //     Debug.LogWarning("Attacking player: " + collision.gameObject.name);
        //     // Send RPC to the enemy's owner to apply damage
        //     enemyPhotonView.RPC("ApplyDamage", enemyPhotonView.Owner, damageDealt);
        // }
        // else
        // {
        //     Debug.LogWarning("PhotonView not found on collided player.");
        // }
        //     }
        
        private void CauseDamage()
        {
            if (_enemy == null) return;
            if (_enemy.GetComponent<Animator>() is null) return;   // Enemy is dead
            if (!EnemyIsInDamageDealtDistance()) return;
            
            if (EnemyIsInDamageDealtDistance())
            {
                if (!photonView.IsMine) return; // Only the local player can initiate attacks
                PhotonView enemyPhotonView = _enemy.GetComponentInParent<PhotonView>();
                if (enemyPhotonView != null && !enemyPhotonView.IsMine)
                {
                    Debug.LogWarning("Attacking player: " + _enemy.name);
                    // Send RPC to the enemy's owner to apply damage
                    enemyPhotonView.RPC("ApplyDamage", enemyPhotonView.Owner, damageDealt);
                }
            }
            

        }
    }
}