using OtherCharacters.Merchant;
using Photon.Pun;
using Respawner;
using UnityEngine;

namespace Photon.Enemy
{
    public class MultiEnemyMovement : MonoBehaviourPunCallbacks, IPunObservable
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
        private GameObject _healthBar;
        private static readonly int Walk = Animator.StringToHash("walk");
        private static readonly int Idle = Animator.StringToHash("idle");
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
            _characterBoxCollider = GetComponent<BoxCollider2D>();
            _characterAnimator = GetComponent<Animator>();
            _characterDetector = GetComponent<AttackHandler>();
            // _healthBar = GetComponent<>()
        }

        private void Update()
        {
        
            if (_characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("die") || transform.position.y < deathPoint || currentHealth <= 0)
            {
                enabled = false;
                return;
            }
            
            
            _playerDetected = _characterDetector.PlayerDetectedOnLeft() || _characterDetector.PlayerDetectedOnRight();
            _isWallOnLeft = IsWallOnLeft();
            _isWallOnRight = IsWallOnRight();

            if (!IsGrounded())
            {
                _fallVelocity += -gravityAcceleration * Time.deltaTime;
                transform.Translate(new Vector2(0, _fallVelocity));
                return;
            }

            if (IsGrounded())
            {
                _fallVelocity = 0;
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
            var raycastHit = Physics2D.Raycast(_characterBoxCollider.bounds.center, Vector2.right, 0.5f, LayerMask.GetMask("Ground"));
            return raycastHit.collider is not null;
        }
        
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Send position and rotation data
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
        
                // Send health data
                stream.SendNext(currentHealth);
        
                // Send animation state
                bool isWalking = _characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("walk");
                stream.SendNext(isWalking);
            }
            else
            {
                // Receive position and rotation data
                transform.position = (Vector3)stream.ReceiveNext();
                transform.rotation = (Quaternion)stream.ReceiveNext();
        
                // Receive health data
                currentHealth = (float)stream.ReceiveNext();
        
                // Receive animation state
                bool isWalking = (bool)stream.ReceiveNext();
                if (isWalking)
                {
                    _characterAnimator.SetTrigger(Walk);
                }
                else
                {
                    _characterAnimator.SetTrigger(Idle);
                }
            }
        }

    }
}
