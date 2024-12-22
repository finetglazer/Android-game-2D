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
            Debug.LogWarning("BoxCast initiated");

            // Define the direction based on player's facing
            Vector2 direction = new Vector2(-Mathf.Sign(transform.localScale.x), 0);

            // Perform the BoxCast
            RaycastHit2D raycastHit = Physics2D.BoxCast(
                _playerBoxCollider.bounds.center,
                _playerBoxCollider.bounds.size,
                0,
                direction,
                distanceDealDamage,
                LayerMask.GetMask("Player") // Ensure this layer includes all players
            );

            // Debug information
            if (raycastHit.collider != null)
            {
                GameObject detectedObject = raycastHit.collider.gameObject;

                // Get the PhotonView of the detected object
                PhotonView detectedPhotonView = detectedObject.GetComponent<PhotonView>();

                // Ensure the detected object has a PhotonView and it's not the local player
                if (detectedPhotonView != null && !detectedPhotonView.Owner.Equals(photonView.Owner))
                {
                    _enemy = detectedObject;
                    Debug.LogWarning("Enemy found: " + _enemy.GetComponent<PhotonView>().Owner.NickName);
                    return true;
                }
                else
                {
                    Debug.LogWarning("Detected object is self or has no PhotonView.");
                }
            }
            else
            {
                Debug.LogWarning("No enemy detected.");
            }

            return false;
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
            Debug.LogWarning("dfdf");
            EnemyIsInDamageDealtDistance();
            if (_enemy == null)
            {
                Debug.LogWarning("Enemy not found");
                return;
            }


            if (_enemy.GetComponent<Animator>() is null)
            {
                Debug.LogWarning("Enemy is dead");
                return; // Enemy is dead
            }
        
            if (!EnemyIsInDamageDealtDistance()) {
                print("xxx3");
                 return;}
            
            if (EnemyIsInDamageDealtDistance())
            {
                if (!photonView.IsMine) {
                    print("xxx4");
                    return; // Only the local player can initiate attacks
                }
                PhotonView enemyPhotonView = _enemy.GetComponent<PhotonView>();
                if (enemyPhotonView != null && !enemyPhotonView.IsMine)
                {
                    Debug.LogWarning("Attacking player: " + _enemy.name);
                    // Send RPC to the enemy's owner to apply damage
                    enemyPhotonView.RPC("ApplyDamage", enemyPhotonView.Owner, damageDealt);
                }
                else
                {
                    Debug.LogWarning("player: " + enemyPhotonView.Owner.NickName);
                }
            }
            

        }
    }
}