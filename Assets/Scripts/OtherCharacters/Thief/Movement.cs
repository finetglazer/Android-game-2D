using Respawner;
using UnityEngine;

namespace OtherCharacters.Thief
{
    public class Movement : MonoBehaviour
    {
        public float walkSpeed = 4;
        public float jumpSpeed = 2;
        public float currentHealth = 1;
        public float idleTime = 1;
        public float moveTime = 1;
        public float deathPoint = -100;
        public float gravityAcceleration = 0.4f;
        public float distanceDetectingFire = 1f;
        public float immortalRerenderTime = 1f;
        private static readonly int Walk = Animator.StringToHash("walk");
        private static readonly int Idle = Animator.StringToHash("idle");
        private static readonly int Jump = Animator.StringToHash("jump");
        private BoxCollider2D _characterBoxCollider;
        private Animator _characterAnimator;
        private AttackHandler _characterDetector;
        private float _clock;
        private Vector3 _initialPosition;
        private float _fallVelocity;
        private bool _playerDetected;
        private bool _isWallOnLeft;
        private bool _isWallOnRight;
        private bool _isFlaming;
        
        private void Start()
        {
            _initialPosition = transform.position;
            DeathNote.AddObject(gameObject, _initialPosition);
            _characterBoxCollider = GetComponent<BoxCollider2D>();
            _characterAnimator = GetComponent<Animator>();
            _characterDetector = GetComponent<AttackHandler>();
        }
        
        private void Update()
        {
            if (_characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("die") || transform.position.y < deathPoint)
            {
                GetComponent<Movement>().enabled = false;
                return;
            }

            if (IsFireOnLeft())
            {
                _isFlaming = true;
            }

            if (_isFlaming)
            {
                RunAwayFromFire();
                return;
            }
            
            _playerDetected = _characterDetector.PlayerDetectedOnLeft() || _characterDetector.PlayerDetectedOnRight();
            _isWallOnLeft = IsWallOnLeft();
            _isWallOnRight = IsWallOnRight();
            
            if (!IsGrounded())
            {
                _fallVelocity += -gravityAcceleration * Time.deltaTime;
                _characterAnimator.SetTrigger(Jump);
                transform.Translate(new Vector2(0, _fallVelocity));
                return;
            }

            if (_isWallOnLeft && !_isWallOnRight)
            {
                _clock = idleTime;
            }
            else if (_isWallOnRight && !_isWallOnLeft)
            {
                _clock = 2 * idleTime + moveTime;
            }
            else if (_isWallOnLeft && _isWallOnRight)
            {
                return;
            }
            
            if (_playerDetected) return;

            _fallVelocity = 0;
            _clock += Time.deltaTime;
            
            if (_clock < idleTime)
            {
                CharacterIdle();
            }
            else if (_clock < idleTime + moveTime)
            {
                CharacterWalkRight();
            }
            else if (_clock < 2 * idleTime + moveTime)
            {
                CharacterIdle();                    
            } 
            else if (_clock < 2 * moveTime + 2 * idleTime)
            {
                CharacterWalkLeft();
            }
            else
            {
                _clock = 0;
                CharacterIdle();
            }
        }

        private void CharacterIdle()
        {
            _characterAnimator.SetTrigger(Idle);
        }

        private void CharacterWalkRight()
        {
            transform.Translate(new Vector2(walkSpeed * Time.deltaTime, 0));
            TurnRight();
            _characterAnimator.SetTrigger(Walk);
        }

        private void CharacterWalkLeft()
        {
            transform.Translate(new Vector2(-walkSpeed * Time.deltaTime, 0));
            TurnLeft();
            _characterAnimator.SetTrigger(Walk);              
        }

        internal void TurnLeft()
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        internal void TurnRight()
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        
        private bool IsGrounded()
        {
            var raycastHit = Physics2D.BoxCast(_characterBoxCollider.bounds.center, _characterBoxCollider.bounds.size, 0, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
            return raycastHit.collider is not null;
        }

        internal bool IsWallOnLeft()
        {
            var raycastHit = Physics2D.Raycast(_characterBoxCollider.bounds.center, Vector2.left, 0.1f, LayerMask.GetMask("Ground"));
            return raycastHit.collider is not null;
        }

        internal bool IsWallOnRight()
        {
            var raycastHit = Physics2D.Raycast(_characterBoxCollider.bounds.center, Vector2.right, 0.1f, LayerMask.GetMask("Ground"));
            return raycastHit.collider is not null;
        }

        private bool IsFireOnLeft()
        {
            var raycastHit = Physics2D.Raycast(_characterBoxCollider.bounds.center, Vector2.left, distanceDetectingFire, LayerMask.GetMask("Fire"));
            return raycastHit.collider is not null;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void RunAwayFromFire()
        {
            if (_isWallOnRight) return;
            TurnRight();
            gameObject.GetComponent<AttackHandler>().enabled = false;
            _characterAnimator.SetTrigger(Walk);
            var newWalkSpeed = walkSpeed * 2;   // Increase by 200% of walk speed
            transform.position += new Vector3(newWalkSpeed * Time.deltaTime , 0, 0);
        }
    }
}
