using Photon.Pun;
using UnityEngine;

namespace Photon.Solo.Characters.Knight
{
    public class AttackHandlerSoloPlayer : MonoBehaviourPun
    {
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
            contactFilter.SetLayerMask(LayerMask.GetMask("Player", "Enemy")); // Consider changing to "Enemy" if using separate layers
            // contactFilter.useTriggers = false;   // Solo mode
            contactFilter.useTriggers = true;       // Multiplayer mode

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
            
            print("number of objects in range: " + hitCount);

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit2D hit = results[i];
                if (hit.collider != null && hit.collider.gameObject != gameObject)
                {
                    Debug.Log("xyyy");
                    PhotonView detectedPhotonView = hit.collider.GetComponent<PhotonView>();
                    if (detectedPhotonView != null 
                        && (!detectedPhotonView.Owner.Equals(photonView.Owner))     // Solo mode
                            || gameObject.name != hit.collider.gameObject.name)     // Multiplayer mode
                    {
                        _enemy = hit.collider.gameObject;
                        if (!detectedPhotonView.Owner.Equals(photonView.Owner))
                        {
                            Debug.Log("Enemy found: " + _enemy.name + " by " + photonView.Owner.NickName);
                        }
                        else if (gameObject.name != hit.collider.gameObject.name)
                        {
                            Debug.Log("Enemy found: " + _enemy.name + " by " + hit.collider.gameObject.name);
                        }

                        return true;
                    }
                    else if (detectedPhotonView is null)
                    {
                        Debug.LogWarning("Detected object is null");
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
                if (enemyPhotonView != null 
                    && (!enemyPhotonView.IsMine                 // Solo mode
                        || gameObject.name != _enemy.name))     // Multiplayer mode                      
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