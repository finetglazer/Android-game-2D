using JetBrains.Annotations;
using UnityEngine;

namespace Others.Merchant
{
    public class AttackHandler : MonoBehaviour
    {
        public float damageDealt = 1;
        public float distanceTriggeringAttack = 1;
        public float distanceDetectingPlayer = 1;
        public float delayBeforeCausingDamage = 0.5f;
        private static readonly int Hurt = Animator.StringToHash("hurt");
        private static readonly int Die = Animator.StringToHash("die");
        private static readonly int Attack = Animator.StringToHash("attack");
        private const string PlayerMask = "Player";
        private BoxCollider2D _characterBoxCollider;
        private Animator _characterAnimator;
        [CanBeNull] private GameObject _player;
        private Movement _characterMovement;
        
        private void Start()
        {
            _characterBoxCollider = GetComponent<BoxCollider2D>();
            _characterAnimator = GetComponent<Animator>();
            _characterMovement = GetComponent<Movement>();
        }

        private void Update()
        {
            if (!PlayerDetectedOnLeft() && !PlayerDetectedOnRight()) return;
            // ChasePlayer();
            CauseDamage();
        }

        internal bool PlayerDetectedOnLeft()
        {
            var raycastHit = Physics2D.BoxCast(_characterBoxCollider.bounds.center, _characterBoxCollider.bounds.size, 0f, Vector2.left, distanceDetectingPlayer, LayerMask.GetMask(PlayerMask));
            if (raycastHit.collider is not null) _player = raycastHit.collider.gameObject;
            return raycastHit.collider is not null;
        }

        internal bool PlayerDetectedOnRight()
        {
            var raycastHit = Physics2D.BoxCast(_characterBoxCollider.bounds.center, _characterBoxCollider.bounds.size, 0f, Vector2.right, distanceDetectingPlayer, LayerMask.GetMask(PlayerMask));
            if (raycastHit.collider is not null) _player = raycastHit.collider.gameObject;
            return raycastHit.collider is not null;
        }

        private void ChasePlayer()
        {
            
        }

        // ReSharper disabled Unity.PerformanceAnalysis avoiding GetComponent<T>() invoked continuously in Update()
        // ReSharper disable Unity.PerformanceAnalysis
        private void CauseDamage()
        {
            
            if (PlayerDetectedOnLeft()) _characterMovement.TurnLeft();
            else _characterMovement.TurnRight();

            var playerAnimator = _player!.GetComponent<Animator>();
            var currentPlayerHealth = _player!.GetComponent<MainCharacter.Movement>().currentHealth;
            
            _characterAnimator.SetTrigger(Attack);
            playerAnimator.SetTrigger(Hurt);
            var currentCharacterAnimation = _characterAnimator.GetCurrentAnimatorStateInfo(0);
            if (currentCharacterAnimation.IsName("attack") && currentCharacterAnimation.length >= delayBeforeCausingDamage) currentPlayerHealth -= damageDealt;
            _player!.GetComponent<MainCharacter.Movement>().currentHealth = currentPlayerHealth;
            if (currentPlayerHealth <= 0)
            {
                playerAnimator.SetTrigger(Die);
            }
        }
    }
}
