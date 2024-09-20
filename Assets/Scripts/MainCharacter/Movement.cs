using UnityEditor.SceneManagement;
using UnityEngine;

namespace MainCharacter
{
    public class Movement : MonoBehaviour
    {
        public float walkSpeed = 2;
        public float jumpSpeed = 2;
        public float currentHealth = 2;
        private static readonly int Walk = Animator.StringToHash("walk");
        private static readonly int Attack = Animator.StringToHash("attack");
        private static readonly int Casting = Animator.StringToHash("casting");
        private static readonly int Victory = Animator.StringToHash("victory");
        private static readonly int Idle = Animator.StringToHash("idle");
        private static readonly int Jump = Animator.StringToHash("jump");
        private Rigidbody2D _playerBody;
        private BoxCollider2D _playerBoxCollider;
        private Animator _playerAnimator;
        private GameObject _enemy;
        private float _horizontalInput;

        private void Start()
        {
            _playerBody = GetComponent<Rigidbody2D>();
            _playerBoxCollider = GetComponent<BoxCollider2D>();
            _playerAnimator = GetComponent<Animator>();
        }
        // TODO: Must handle movement on Simulator
        private void Update()
        {
            if (_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("die"))
            {
                Destroy(this);
                // _playerBody.velocity = Vector2.zero;
                enabled = false;   
            }
            
            _horizontalInput = Input.GetAxis("Horizontal"); 
            if (_horizontalInput == 0 && IsGrounded())
            {
                PlayerIdle();
            }
            
            if (_horizontalInput != 0)
            {
                if (_horizontalInput < 0) TurnLeft(); else TurnRight();
                if (IsGrounded())
                {
                    PlayerWalk();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space) || !IsGrounded())
            {
                PlayerJump();
            }

            if (Input.GetMouseButtonDown(0))
            {
                PlayerAttack();
            }
        }
        
        private void PlayerCasting()
        {
            _playerAnimator.SetTrigger(Casting);    
        }

        private void PlayerVictory()
        {
            _playerAnimator.SetTrigger(Victory);
        }
        
        private void PlayerIdle()
        {
            _playerAnimator.SetTrigger(Idle);
            _playerBody.velocity = Vector2.zero;
        }

        private void PlayerWalk()
        {
            _playerAnimator.SetTrigger(Walk);
            _playerBody.velocity = _horizontalInput switch
            {
                > 0 => new Vector2(walkSpeed, _playerBody.velocity.y),
                < 0 => new Vector2(-walkSpeed, _playerBody.velocity.y),
                _ => _playerBody.velocity
            };
        }

        private void PlayerAttack()
        {
            _playerAnimator.SetTrigger(Attack);
        }
        private void TurnLeft()
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        private void PlayerJump()
        {
            _playerAnimator.SetTrigger(Jump);
            _playerBody.velocity = new Vector2(_playerBody.velocity.x, IsGrounded() ? jumpSpeed : _playerBody.velocity.y);

        }
        private void TurnRight()
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        private bool IsGrounded()
        {
            var raycastHit = Physics2D.BoxCast(_playerBoxCollider.bounds.center, _playerBoxCollider.bounds.size, 0, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
            return raycastHit.collider is not null;
        }
    }
}
