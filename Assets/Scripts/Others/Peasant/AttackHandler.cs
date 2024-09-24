
using UnityEngine;

namespace Others.Peasant
{
    public class AttackHandler : MonoBehaviour
    {
        private static readonly int Attack = Animator.StringToHash("attack");
        private static readonly int Idle = Animator.StringToHash("idle");
        public float damageDealt = 1;
        public float distanceDetectingPlayer = 1;
        public float attackCoolDownTime = 1;
        public float arrowSpeed = 5;
        public GameObject arrowStartingPoint;
        public GameObject[] arrowArray;
        private const string PlayerMask = "Player";
        private const string PlayerTag = "Player";
        private Animator _playerAnimator;
        private BoxCollider2D _characterBoxCollider;
        private Animator _characterAnimator;
        private Movement _characterMovement;
        private float _attackCoolDownTime;
        private bool _isPlayerDead;
        internal float ArrowDirection;

        public AttackHandler(GameObject[] arrowArray)
        {
            this.arrowArray = arrowArray;
        }

        private void Start()
        {
            _attackCoolDownTime = attackCoolDownTime;
            _playerAnimator = GameObject.FindGameObjectWithTag(PlayerTag).GetComponent<Animator>();
            _characterBoxCollider = GetComponent<BoxCollider2D>();
            _characterAnimator = GetComponent<Animator>();
            _characterMovement = GetComponent<Movement>();
            _isPlayerDead = false;
        }

        private void Update()
        {
            if (_characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("die"))
            {
                Destroy(this);
                return;
            }
            
            _attackCoolDownTime += Time.deltaTime;
                
            if (!PlayerDetectedOnLeft() && !PlayerDetectedOnRight()) return;

            if (_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("die"))
            {
                _isPlayerDead = true;
            }
            if (_attackCoolDownTime < attackCoolDownTime)
            {
                _characterAnimator.SetTrigger(Idle);
                return;
            }
            
            _attackCoolDownTime = 0;
            if (_isPlayerDead) return;
            
            if (PlayerDetectedOnLeft())
            {
                _characterMovement.TurnLeft();
                ArrowDirection = 1;
            }
            else if (PlayerDetectedOnRight())
            {
                _characterMovement.TurnRight();
                ArrowDirection = -1;
            }
            else return;
            
            _characterAnimator.SetTrigger(Attack);  // Firing() will be called in Animation Event
        }

        internal bool PlayerDetectedOnLeft()
        {
            var raycastHit = Physics2D.BoxCast(_characterBoxCollider.bounds.center, _characterBoxCollider.bounds.size, 0f, Vector2.left, distanceDetectingPlayer, LayerMask.GetMask(PlayerMask));
            return raycastHit.collider is not null;
        }

        internal bool PlayerDetectedOnRight()
        {
            var raycastHit = Physics2D.BoxCast(_characterBoxCollider.bounds.center, _characterBoxCollider.bounds.size, 0f, Vector2.right, distanceDetectingPlayer, LayerMask.GetMask(PlayerMask));
            return raycastHit.collider is not null;
        }

        // ReSharper disabled Unity.PerformanceAnalysis avoiding GetComponent<T>() invoked continuously in Update()
        // ReSharper disable Unity.PerformanceAnalysis
        private void Firing()
        { 
            foreach (var arrow in arrowArray)
            {
                if (arrow.activeInHierarchy) continue;
                arrow.SetActive(true);
                arrow.transform.position = arrowStartingPoint.transform.position;
                ArrowDirection = Mathf.Sign(arrowStartingPoint.transform.localScale.x);
                break;
            }
        }
    }
}
