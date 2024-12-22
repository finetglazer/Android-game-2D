using Photon.Pun;
using UnityEngine;

namespace Photon.Character
{
    public class AttackHandlerMultiplayer : MonoBehaviourPun
    {
        private static readonly int Hurt = Animator.StringToHash("hurt");
        public float damageDealt = 1f;
        public float distanceDealDamage = 4f;
        private GameObject _enemy;

        private BoxCollider2D _playerBoxCollider;

        private void Start()
        {
            _playerBoxCollider = GetComponent<BoxCollider2D>();
        }

        private bool EnemyIsInDamageDealtDistance()
        {
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(LayerMask.GetMask("Player")); // Consider changing to "Enemy" if using separate layers
            contactFilter.useTriggers = false;

            RaycastHit2D[] results = new RaycastHit2D[10]; // Array for potential hits
            Vector2 direction = new Vector2(-Mathf.Sign(transform.localScale.x), 0);
            int hitCount = Physics2D.BoxCast(
                _playerBoxCollider.bounds.center,
                _playerBoxCollider.bounds.size * 0.5f, // Adjusted size for more accurate detection
                0,
                direction,
                contactFilter,
                results,
                distanceDealDamage
            );

            for (int i = 0; i < hitCount; i++)
            {
<<<<<<< HEAD
                GameObject detectedObject = raycastHit.collider.gameObject;

                // Get the PhotonView of the detected object
                PhotonView detectedPhotonView = detectedObject.GetComponent<PhotonView>();

                // Ensure the detected object has a PhotonView and it's not the local player
                if (detectedPhotonView != null && !detectedPhotonView.Owner.Equals(photonView.Owner))
=======
                RaycastHit2D hit = results[i];
                if (hit.collider != null && hit.collider.gameObject != gameObject)
>>>>>>> 6d89e7d95d7b580e101314db607405a5e6bc44ee
                {
                    Debug.LogWarning("xyyy");
                    PhotonView detectedPhotonView = hit.collider.GetComponent<PhotonView>();
                    if (detectedPhotonView != null && detectedPhotonView.Owner != photonView.Owner)
                    {
                        _enemy = hit.collider.gameObject;
                        Debug.LogWarning("Enemy found: " + _enemy.name + " by " + photonView.Owner.NickName);
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning("Detected object is self or teammate.");
                    }
                }
            }

            Debug.Log("No enemies in range.");
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