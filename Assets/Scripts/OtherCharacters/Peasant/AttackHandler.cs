using UnityEngine;

namespace OtherCharacters.Peasant
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
        private BoxCollider2D _characterBoxCollider;
        private Animator _characterAnimator;
        private Movement _characterMovement;
        private float _attackCoolDownTime;
        internal float ArrowDirection;

        private void Start()
        {
            _attackCoolDownTime = attackCoolDownTime;
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
                
            if (!PlayerDetectedOnLeft() && !PlayerDetectedOnRight()) return;
            
            if (_attackCoolDownTime < attackCoolDownTime)
            {
                _characterAnimator.SetTrigger(Idle);
                return;
            }
            
            _attackCoolDownTime = 0;
            
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
            if (_characterMovement.IsWallOnLeft() && PlayerDetectedOnLeft()) return;
            if (_characterMovement.IsWallOnRight() && PlayerDetectedOnRight()) return;
            
            foreach (var arrow in arrowArray)
            {
                if (arrow.activeInHierarchy) continue;
                arrow.SetActive(true);
                arrow.transform.position = arrowStartingPoint.transform.position;
                arrow.transform.localScale = new Vector3(arrow.transform.localScale.x,
                    Mathf.Sign(gameObject.transform.localScale.x) * Mathf.Abs(arrow.transform.localScale.y),
                    arrow.transform.localScale.z);
                ArrowDirection = Mathf.Sign(gameObject.transform.localScale.x);
                Debug.Log(ArrowDirection);
                Debug.Log(gameObject.transform.localScale.x);
                arrow.GetComponent<ArrowMovement>().characterController = this;
                break;
            }
        }
    }
}
