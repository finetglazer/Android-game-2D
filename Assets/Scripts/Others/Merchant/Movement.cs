using UnityEngine;

namespace Others.Merchant
{
    public class Movement : MonoBehaviour
    {
        public float walkSpeed = 4;
        public float jumpSpeed = 2;
        public float currentHealth = 1;
        public float idleTime = 1;
        public float moveTime = 1;
        public float gravityAcceleration = 0.4f;
        private static readonly int Walk = Animator.StringToHash("walk");
        private static readonly int Idle = Animator.StringToHash("idle");
        private static readonly int Jump = Animator.StringToHash("jump");
        private BoxCollider2D _characterBoxCollider;
        private Animator _characterAnimator;
        private AttackHandler _characterDetector;
        private float _clock;
        private float _fallVelocity;
        private bool _playerDetected;
        private void Start()
        {
            _characterBoxCollider = GetComponent<BoxCollider2D>();
            _characterAnimator = GetComponent<Animator>();
            _characterDetector = GetComponent<AttackHandler>();
        }
        private void Update()
        {
            if (_characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("die"))
            {
                Destroy(this);
                return;
            }
            
            _playerDetected = _characterDetector.PlayerDetectedOnLeft() || _characterDetector.PlayerDetectedOnRight();

            if (!IsGrounded())
            {
                _fallVelocity += -gravityAcceleration * Time.deltaTime;
                _characterAnimator.SetTrigger(Jump);
                transform.Translate(new Vector2(0, _fallVelocity));
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
    }
}
