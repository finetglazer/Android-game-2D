using System;
using System.Text;
using UnityEngine;

namespace Others.Peasant
{
    public class Movement : MonoBehaviour
    {
        public float walkSpeed = 4;
        public float jumpSpeed = 2;
        public float currentHealth = 2;
        public float idleTime = 1;
        public float moveTime = 1;
        public float distanceDetectingEnemy = 1;
        public BoxCollider2D stopChasingPoint;             // Where a bot will stop chasing player
        private static readonly int Walk = Animator.StringToHash("walk");
        private static readonly int Attack = Animator.StringToHash("attack");
        private static readonly int Die = Animator.StringToHash("die");
        private static readonly int Hurt = Animator.StringToHash("hurt");
        private static readonly int Victory = Animator.StringToHash("victory");
        private static readonly int Idle = Animator.StringToHash("idle");
        private static readonly int Jump = Animator.StringToHash("jump");
        private BoxCollider2D _characterBoxCollider;
        private Animator _animator;
        private float _clock;
        private bool _enemyDetected;
        private void Start()
        {
            _characterBoxCollider = GetComponent<BoxCollider2D>();
            _animator = GetComponent<Animator>();
        }
        private void Update()
        {
            _enemyDetected = EnemyDetectedOnLeft() || EnemyDetectedOnRight();
            
            if (IsGrounded() && !_enemyDetected)
            {
                _clock += Time.deltaTime;
                if (_clock < idleTime)
                {
                    _animator.SetTrigger(Idle);
                }
                else if (_clock < idleTime + moveTime)
                {
                    transform.Translate(new Vector2(walkSpeed * Time.deltaTime, 0));
                    TurnRight();
                    _animator.SetTrigger(Walk);
                }
                else if (_clock < 2 * idleTime + moveTime)
                {
                    _animator.SetTrigger(Idle);                    
                } 
                else if (_clock < 2 * moveTime + 2 * moveTime)
                {
                    transform.Translate(new Vector2(-walkSpeed * Time.deltaTime, 0));
                    TurnLeft();
                    _animator.SetTrigger(Walk);                    
                }
                else
                {
                    _clock = 0;
                    _animator.SetTrigger(Idle);
                }
            }

            if (_enemyDetected)
            {
                if (EnemyDetectedOnLeft()) TurnLeft(); else TurnRight();
                _animator.SetTrigger(Attack);
                // ChasePlayer();
            }
        }

        private void TurnLeft()
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        private void TurnRight()
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        private bool IsGrounded()
        {
            var raycastHit = Physics2D.BoxCast(_characterBoxCollider.bounds.center, _characterBoxCollider.bounds.size, 0, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
            return raycastHit.collider is not null;
        }
        private bool EnemyDetectedOnLeft()
        {
            var raycastHitLeft = Physics2D.BoxCast(_characterBoxCollider.bounds.center, _characterBoxCollider.bounds.size, 0, Vector2.left, distanceDetectingEnemy, LayerMask.GetMask("Player"));
            return raycastHitLeft.collider is not null;            
        }
        private bool EnemyDetectedOnRight()
        {
            var raycastHitRight = Physics2D.BoxCast(_characterBoxCollider.bounds.center, _characterBoxCollider.bounds.size, 0, Vector2.right, distanceDetectingEnemy, LayerMask.GetMask("Player"));
            return raycastHitRight.collider is not null;
        }
        private void ChasePlayer()
        {
            var enemyOnLeft = EnemyDetectedOnLeft();
            if (enemyOnLeft) TurnLeft(); else TurnRight();
        }
    }
}
