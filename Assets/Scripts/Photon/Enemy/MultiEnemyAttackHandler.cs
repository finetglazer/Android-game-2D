using Photon.Pun;
using Photon.Solo.Commons.HealthBar;
using UnityEngine;

namespace Photon.Enemy
{
    public class MultiEnemyAttackHandler : MonoBehaviourPun, IPunObservable
    {
        // Public Variables
        public float damageDealt = 1f;
        public float distanceTriggeringAttack = 1f;
        public float distanceDetectingPlayer = 1f;
        public float increaseWalkSpeedWhenChasingBy = 0.2f;
        public float attackCoolDownTime = 1f;
        public float delayTimeBeforeAttacking = 0.2f;
        // public GameObject healthBar;

        // Animator Parameters
        private static readonly int Hurt = Animator.StringToHash("hurt");
        private static readonly int Die = Animator.StringToHash("die");
        private static readonly int Attack = Animator.StringToHash("attack");
        private static readonly int Walk = Animator.StringToHash("walk");
        private static readonly int Idle = Animator.StringToHash("idle");

        // Layer Masks
        private const string PlayerMask = "Player";

        // Private Variables
        private BoxCollider2D _characterBoxCollider;
        private Animator _characterAnimator;
        private GameObject _player;
        private MultiEnemyMovement _characterMovement;
        private float _attackCoolDownTimer;
        private float _delayClock;
        private float _firstWalkSpeed;
        private bool _isAttacking;

        // Synchronization Variables
        private float synchronizedHealth;
        private bool synchronizedIsAttacking;

        private Solo.Characters.Knight.MovementSoloPlayer _knightMovementSoloPlayer;
        private Solo.Characters.Merchant.MovementSoloPlayer _merchantMovementSoloPlayer;
        private Solo.Characters.Peasant.MovementSoloPlayer _peasantMovementSoloPlayer;
        private Solo.Characters.Soldier.MovementSoloPlayer _soldierMovementSoloPlayer;
        private Solo.Characters.Thief.MovementSoloPlayer _thiefMovementSoloPlayer;
        
        private void Start()
        {
            // Initialize Components
            _characterBoxCollider = GetComponent<BoxCollider2D>();
            _characterAnimator = GetComponent<Animator>();
            _characterMovement = GetComponent<MultiEnemyMovement>();
        
            // Cache initial walk speed
            if (_characterMovement != null)
            {
                _firstWalkSpeed = _characterMovement.walkSpeed;
            }
            else
            {
                Debug.LogError("Movement script is missing on the enemy prefab.");
            }
            
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                // Owner client handles attack logic
                HandleAttackLogic();
            }
            else
            {
                // Non-owner clients update health bar based on synchronized data
                // UpdateHealthBar();
            }


        }

        /// <summary>
        /// Handles the attack logic on the owner client.
        /// </summary>
        private void HandleAttackLogic()
        {
            _attackCoolDownTimer += Time.deltaTime;

            // Detect player presence
            var playerDetected = PlayerDetectedOnLeft() || PlayerDetectedOnRight();

            if (!playerDetected)
            {
                _characterMovement.walkSpeed = _firstWalkSpeed; // Reset to initial speed
                _delayClock = 0f;
                return;
            }

            // Chase the player
            ChasePlayer();

            // Check if player is within attack distance
            if (!PlayerIsInDamageDealtDistance()) return;

            // Handle attack animations based on player's state
            if (_player != null)
            {
                var playerAnimator = _player.GetComponent<Animator>();
                if (playerAnimator != null &&
                    playerAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "walk")
                {
                    _characterAnimator.SetTrigger(Idle);
                }
            }

            // Check attack cooldown
            if (_attackCoolDownTimer < attackCoolDownTime) return;

            // Handle attack delay
            _delayClock += Time.deltaTime;
            if (_delayClock < delayTimeBeforeAttacking) return;

            // Trigger attack animation and reset timers
            _characterAnimator.SetTrigger(Attack);
            _attackCoolDownTimer = 0f;
            _delayClock = 0f;

            // Invoke damage via RPC
            photonView.RPC("CauseDamage", RpcTarget.AllBuffered);
        }

        /// <summary>
        /// Detects if a player is on the left side.
        /// </summary>
        /// <returns>True if a player is detected on the left, otherwise false.</returns>
        internal bool PlayerDetectedOnLeft()
        {
            var raycastHit = Physics2D.Raycast(
                _characterBoxCollider.bounds.center,
                Vector2.left,
                distanceDetectingPlayer,
                LayerMask.GetMask(PlayerMask)
            );

            if (raycastHit.collider != null)
            {
                _player = raycastHit.collider.gameObject;
                return true;
            }

            _player = null;
            return false;
        }

        /// <summary>
        /// Detects if a player is on the right side.
        /// </summary>
        /// <returns>True if a player is detected on the right, otherwise false.</returns>
        internal bool PlayerDetectedOnRight()
        {
            var raycastHit = Physics2D.Raycast(
                _characterBoxCollider.bounds.center,
                Vector2.right,
                distanceDetectingPlayer,
                LayerMask.GetMask(PlayerMask)
            );

            if (raycastHit.collider != null)
            {
                _player = raycastHit.collider.gameObject;
                return true;
            }

            _player = null;
            return false;
        }

        /// <summary>
        /// Checks if the player is within damage-dealing distance.
        /// </summary>
        /// <returns>True if the player is within range, otherwise false.</returns>
        private bool PlayerIsInDamageDealtDistance()
        {
            var raycastHitLeft = Physics2D.BoxCast(
                _characterBoxCollider.bounds.center,
                _characterBoxCollider.bounds.size,
                0f,
                Vector2.left,
                distanceTriggeringAttack,
                LayerMask.GetMask(PlayerMask)
            );

            var raycastHitRight = Physics2D.BoxCast(
                _characterBoxCollider.bounds.center,
                _characterBoxCollider.bounds.size,
                0f,
                Vector2.right,
                distanceTriggeringAttack,
                LayerMask.GetMask(PlayerMask)
            );

            return raycastHitLeft.collider != null || raycastHitRight.collider != null;
        }

        /// <summary>
        /// Handles chasing the detected player.
        /// </summary>
        private void ChasePlayer()
        {
            if (_player == null || PlayerIsInDamageDealtDistance())
            {
                return;
            }

            // Determine direction to turn
            if (PlayerDetectedOnLeft())
            {
                _characterMovement.TurnLeft();
            }
            else if (PlayerDetectedOnRight())
            {
                _characterMovement.TurnRight();
            }

            // Check for walls
            if (_characterMovement.IsWallOnLeft() && PlayerDetectedOnLeft()) return;
            if (_characterMovement.IsWallOnRight() && PlayerDetectedOnRight()) return;

            // Trigger walking animation
            _characterAnimator.SetTrigger(Walk);

            // Increase walk speed when chasing
            var newWalkSpeed = _firstWalkSpeed * (1 + increaseWalkSpeedWhenChasingBy);
            _characterMovement.walkSpeed = newWalkSpeed;

            // Move towards the player
            var movementDirection = Mathf.Approximately(Mathf.Sign(transform.localScale.x), 1) 
                ? Vector3.left 
                : Vector3.right;

            transform.position += movementDirection * newWalkSpeed * Time.deltaTime;
        }

        /// <summary>
        /// Causes damage to the detected player.
        /// This method is called via RPC to ensure all clients apply the damage.
        /// </summary>
        [PunRPC]
        private void CauseDamage()
        {
            if (_player == null) return;

            var playerAnimator = _player.GetComponent<Animator>();

            _knightMovementSoloPlayer = _player.GetComponent<Solo.Characters.Knight.MovementSoloPlayer>();
            _merchantMovementSoloPlayer = _player.GetComponent<Solo.Characters.Merchant.MovementSoloPlayer>();
            _peasantMovementSoloPlayer = _player.GetComponent<Solo.Characters.Peasant.MovementSoloPlayer>();
            _soldierMovementSoloPlayer = _player.GetComponent<Solo.Characters.Soldier.MovementSoloPlayer>();
            _thiefMovementSoloPlayer = _player.GetComponent<Solo.Characters.Thief.MovementSoloPlayer>();
            
            if (AllPlayerMovementAreNull())
            {
                Debug.LogError("Player's Movement script is missing.");
                return;
            }

            // Apply damage
            if (_knightMovementSoloPlayer) _knightMovementSoloPlayer.currentHealth -= damageDealt;
            else if (_merchantMovementSoloPlayer) _merchantMovementSoloPlayer.currentHealth -= damageDealt;
            else if (_peasantMovementSoloPlayer) _peasantMovementSoloPlayer.currentHealth -= damageDealt;
            else if (_soldierMovementSoloPlayer) _soldierMovementSoloPlayer.currentHealth -= damageDealt;
            else if (_thiefMovementSoloPlayer) _thiefMovementSoloPlayer.currentHealth -= damageDealt;

            // Trigger hurt animation
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger(Hurt);
            }

            // Check if player is dead
            if ((_knightMovementSoloPlayer && _knightMovementSoloPlayer.currentHealth <= 0)
                || (_merchantMovementSoloPlayer && _merchantMovementSoloPlayer.currentHealth <= 0)
                || (_peasantMovementSoloPlayer && _peasantMovementSoloPlayer.currentHealth <= 0)
                || (_soldierMovementSoloPlayer && _soldierMovementSoloPlayer.currentHealth <= 0)
                || (_thiefMovementSoloPlayer && _thiefMovementSoloPlayer.currentHealth <= 0))
            {
                if (playerAnimator != null)
                {
                    playerAnimator.SetTrigger(Die);
                }
            }
            
        }

        /// <summary>
        /// Updates the enemy's health bar on non-owner clients.
        /// </summary>
        // private void UpdateHealthBar()
        // {
        //     if (_healthBarManager == null) return;
        //
        //     HealthBarUI healthBarUI = _healthBarManager.GetEnemyHealthBar(photonView.ViewID);
        //     if (healthBarUI != null)
        //     {
        //         healthBarUI.UpdateHealth(synchronizedHealth, 1f); // Adjust maxHealth as needed
        //     }
        // }

        /// <summary>
        /// Serializes and deserializes data for synchronization.
        /// </summary>
        /// <param name="stream">PhotonStream for sending/receiving data.</param>
        /// <param name="info">PhotonMessageInfo containing info about the message.</param>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Owner sends data
                // stream.SendNext(currentHealth);
                stream.SendNext(_isAttacking);
            }
            else
            {
                // Non-owners receive data
                // synchronizedHealth = (float)stream.ReceiveNext();
                // synchronizedIsAttacking = (bool)stream.ReceiveNext();
                //
                // // Update local variables
                // // currentHealth = synchronizedHealth;
                //
                // // Update attack animation if necessary
                // if (synchronizedIsAttacking)
                // {
                //     _characterAnimator.SetTrigger(Attack);
                // }
            }
        }

        private bool AllPlayerMovementAreNull()
        {
            return (_knightMovementSoloPlayer == null 
                    && _merchantMovementSoloPlayer == null 
                    && _peasantMovementSoloPlayer == null 
                    && _soldierMovementSoloPlayer == null 
                    && _thiefMovementSoloPlayer == null);
        }
    }
}
