using JetBrains.Annotations;
using MainCharacter;
using Photon.Pun;
using Respawner;
using UnityEngine;

namespace Photon.Character
{
    public class AttackHandlerMultiplayer : MonoBehaviourPun, IPunObservable
    {
        private static readonly int Hurt = Animator.StringToHash("hurt");
        private static readonly int Die = Animator.StringToHash("die");
        public float damageDealt = 1f;
        public float distanceDealDamage = 1f;
        private const string EnemyTag = "Enemy";
        [CanBeNull] private GameObject _enemy;
        private BoxCollider2D _playerBoxCollider;
        private Animator _playerAnimator;
        private bool _isEnemyDead;
        
        private void Start()
        {
            _playerBoxCollider = GetComponent<BoxCollider2D>();
            _playerAnimator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                HandleAttackInput();
            }

            // Common updates
            // if (_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("die"))
            // {
            //     if (photonView.IsMine)
            //     {
            //         gameObject.GetComponent<PlayerDie>().Die();
            //     }
            //     return;
            // }

            if (EnemyIsInDamageDealtDistance())
            {
                // Optional: Implement continuous attack or effects
            }
        }

        private void HandleAttackInput()
        {
            // Attack logic is triggered by the Movement script or directly here
            // Depending on your implementation
        }

        private bool EnemyIsInDamageDealtDistance()
        {
            var direction = new Vector2(-Mathf.Sign(transform.localScale.x), 0);
            var raycastHit = Physics2D.BoxCast(
                _playerBoxCollider.bounds.center,
                _playerBoxCollider.bounds.size,
                0,
                direction,
                distanceDealDamage,
                LayerMask.GetMask("Player")
            );

            if (raycastHit.collider != null)
            {
                _enemy = raycastHit.collider.gameObject;
                return true;
            }

            _enemy = null;
            return false;
        }
        
        
        // Temporary no-op to stop warnings
        public void CauseDamage() 
        {
            // Temporary no-op to stop warnings
        }


        // ReSharper disable Unity.PerformanceAnalysis
        [PunRPC]
        private void CauseDamage(int enemyViewID)
        {
            GameObject enemy = PhotonView.Find(enemyViewID)?.gameObject;
            if (enemy == null) return;
        
            Animator enemyAnimator = enemy.GetComponent<Animator>();
            if (enemyAnimator == null) return;
        
            // Determine enemy type and get health component
            float currentEnemyHealth = 0f;
            MovementMultiplayer enemyMovement = enemy.GetComponent<MovementMultiplayer>();
           

            if (enemyMovement == null) return;
            
            currentEnemyHealth = enemyMovement.currentHealth;
            
            if (EnemyIsInDamageDealtDistance())
            {
                currentEnemyHealth -= damageDealt;
                enemyAnimator.SetTrigger(Hurt);
            
                // Update enemy health over the network
                enemyMovement.photonView.RPC("UpdateHealth", RpcTarget.All, currentEnemyHealth);
            
                if (currentEnemyHealth <= 0)
                {
                    _isEnemyDead = !enemy.name.ToLower().Contains("priest") || !BossAndEnemiesRespawner.CanReproducible;
                    if (_isEnemyDead)
                    {
                        enemyAnimator.SetTrigger(Die);
                    }
                }
            }
        }
        
        [PunRPC]
        public void UpdateHealth(float newHealth, MovementMultiplayer enemyMovement)
        {
            enemyMovement.currentHealth = newHealth;
            if (newHealth <= 0)
            {
                gameObject.GetComponent<PlayerDieNetworked>().Die();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Example: Trigger damage when attacking
            if (photonView.IsMine && collision.CompareTag("Player"))
            {
                PhotonView enemyPhotonView = collision.GetComponent<PhotonView>();
                if (enemyPhotonView != null)
                {
                    photonView.RPC("CauseDamage", RpcTarget.All, enemyPhotonView.ViewID);
                }
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            var currentHealth = gameObject.GetComponent<MovementMultiplayer>().currentHealth;
            if (stream.IsWriting)
            {
                // Send the current health to other players
                stream.SendNext(currentHealth);
            }
            else
            {
                // Receive the health from the network
                currentHealth = (float)stream.ReceiveNext();
            }
        }
    }
}
