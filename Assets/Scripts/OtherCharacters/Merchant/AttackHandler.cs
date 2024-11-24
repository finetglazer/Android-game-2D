using JetBrains.Annotations;
using UnityEngine;

namespace OtherCharacters.Merchant
{
    public class AttackHandler : MonoBehaviour
    {
        public float damageDealt = 1;
        public float distanceTriggeringAttack = 1;
        public float distanceDetectingPlayer = 1;
        public float increaseWalkSpeedWhenChasingBy = 0.2f;
        public float attackCoolDownTime = 1;
        public float delayTimeBeforeAttacking = 0.2f;
        private static readonly int Hurt = Animator.StringToHash("hurt");
        private static readonly int Die = Animator.StringToHash("die");
        private static readonly int Attack = Animator.StringToHash("attack");
        private static readonly int Walk = Animator.StringToHash("walk");
        private static readonly int Idle = Animator.StringToHash("idle");
        private const string PlayerMask = "Player";
        private BoxCollider2D _characterBoxCollider;
        private Animator _characterAnimator;
        [CanBeNull] private GameObject _player;
        private Movement _characterMovement;
        private float _attackCoolDownTime;
        private float _delayClock;
        private float _firstWalkSpeed;
        
        private void Start()
        {
            _firstWalkSpeed = GetComponent<Movement>().walkSpeed;
            _characterBoxCollider = GetComponent<BoxCollider2D>();
            _characterAnimator = GetComponent<Animator>();
            _characterMovement = GetComponent<Movement>();
        }

        private void Update()
        {
            if (_characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("die"))
            {
                enabled = false;
                return;
            }
            
            _attackCoolDownTime += Time.deltaTime;
            if (!PlayerDetectedOnLeft() && !PlayerDetectedOnRight())
            {
                GetComponent<Movement>().walkSpeed = _firstWalkSpeed;       // Return to first speed when stopping chasing
                _delayClock = 0;
                return;
            }
            
            ChasePlayer();

            if (!PlayerIsInDamageDealtDistance()) return;

            if (_player is not null &&
                _player.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name != "walk")
            {
                _characterAnimator.SetTrigger(Idle);
            }

            if (_attackCoolDownTime < attackCoolDownTime) return;
            
            _delayClock += Time.deltaTime;
            if (_delayClock < delayTimeBeforeAttacking) return;
            
            _characterAnimator.SetTrigger(Attack);
            _attackCoolDownTime = 0;
        }

        internal bool PlayerDetectedOnLeft()
        {
            var raycastHit = Physics2D.Raycast(_characterBoxCollider.bounds.center, Vector2.left, distanceDetectingPlayer, LayerMask.GetMask(PlayerMask));
            if (raycastHit.collider is not null) _player = raycastHit.collider.gameObject;
            return raycastHit.collider is not null;
        }

        internal bool PlayerDetectedOnRight()
        {
            var raycastHit = Physics2D.Raycast(_characterBoxCollider.bounds.center, Vector2.right, distanceDetectingPlayer, LayerMask.GetMask(PlayerMask));
            if (raycastHit.collider is not null) _player = raycastHit.collider.gameObject;
            return raycastHit.collider is not null;
        }

        private bool PlayerIsInDamageDealtDistance()
        {
            var raycastHitLeft = Physics2D.BoxCast(_characterBoxCollider.bounds.center, _characterBoxCollider.bounds.size, 0f, Vector2.left, distanceTriggeringAttack, LayerMask.GetMask(PlayerMask));
            var raycastHitRight = Physics2D.BoxCast(_characterBoxCollider.bounds.center, _characterBoxCollider.bounds.size, 0f, Vector2.right, distanceTriggeringAttack, LayerMask.GetMask(PlayerMask));
            return raycastHitLeft.collider is not null || raycastHitRight.collider is not null;            
        }
        private void ChasePlayer()
        {
            if (_player is null || PlayerIsInDamageDealtDistance())
            {
                return;
            }
            
            if (PlayerDetectedOnLeft()) _characterMovement.TurnLeft();
            else _characterMovement.TurnRight();

            if (_characterMovement.IsWallOnLeft() && PlayerDetectedOnLeft()) return;
            if (_characterMovement.IsWallOnRight() && PlayerDetectedOnRight()) return;
            
            _characterAnimator.SetTrigger(Walk);

            var newWalkSpeed = _firstWalkSpeed * (1 + increaseWalkSpeedWhenChasingBy);
            GetComponent<Movement>().walkSpeed = newWalkSpeed;
            transform.position += Mathf.Approximately(Mathf.Sign(transform.localScale.x), 1) switch
            {
                true => Vector3.left * (newWalkSpeed * Time.deltaTime),
                _ => Vector3.right * (newWalkSpeed * Time.deltaTime)
            };
        }

        // ReSharper disabled Unity.PerformanceAnalysis avoiding GetComponent<T>() invoked continuously in Update()
        // ReSharper disable Unity.PerformanceAnalysis
        private void CauseDamage()
        {
            if (PlayerDetectedOnLeft()) _characterMovement.TurnLeft();
            else _characterMovement.TurnRight();

            var playerAnimator = _player!.GetComponent<Animator>();
            
            if (_player.GetComponent<MainCharacter.Movement>() is null) return;
            var currentPlayerHealth = _player.GetComponent<MainCharacter.Movement>().currentHealth;

            if (!PlayerDetectedOnLeft() && !PlayerDetectedOnRight()) return;
            
            playerAnimator.SetTrigger(Hurt);
            _player!.GetComponent<MainCharacter.Movement>().currentHealth = currentPlayerHealth - damageDealt;
            currentPlayerHealth -= damageDealt;
            
            if (currentPlayerHealth > 0) return;
           
            playerAnimator.SetTrigger(Die);
        }

    }
}
