using UnityEngine;

namespace UI.Buttons
{
    public class MovementAndAttackButtons : MonoBehaviour
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
        private const string LeftButton = "LeftButton";
        private const string RightButton = "RightButton";
        private const string JumpButton = "JumpButton";
        private const string AttackButton = "AttackButton";
        private float _countdownToDeath;
        private bool _canDoubleJump;
        private bool _isRightButtonBeingClicked;
        private bool _isLeftButtonBeingClicked;
        private bool _isJumpButtonBeingClicked;
        private bool _isAttackButtonBeingClicked;

        private void Start()
        {
            _playerBody = GetComponent<Rigidbody2D>();
            _playerBoxCollider = GetComponent<BoxCollider2D>();
            _playerAnimator = GetComponent<Animator>();
        }
        
        private void Update()
        {
            print(_canDoubleJump);    
            if (_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("die"))
            {
                Destroy(this);
                return;
            }
            
            if (IsGrounded())
            {
                _canDoubleJump = true;
            }
            else
            {
                _playerAnimator.SetTrigger(Jump);
            }
        
            if (_isRightButtonBeingClicked)
            {
                PlayerWalkRight();
            }
            else if (_isLeftButtonBeingClicked)
            {
                PlayerWalkLeft();
            }

            if (_isJumpButtonBeingClicked)
            {
                if (!_canDoubleJump) return;
                PlayerJump();
                _isJumpButtonBeingClicked = false;
            }

            if (_isAttackButtonBeingClicked)
            {
                PlayerAttack();
            }
            
            if (!_isRightButtonBeingClicked && !_isLeftButtonBeingClicked && !_isJumpButtonBeingClicked &&
                !_isAttackButtonBeingClicked)
            {
                PlayerIdle();
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
            _playerBody.velocity = new Vector2(0, _playerBody.velocity.y);
        }

        private void PlayerAttack()
        {
            _playerAnimator.SetTrigger(Attack);
        }

        private void PlayerJump()
        {
            _playerAnimator.SetTrigger(Jump);
            if (IsGrounded())
            {
                print("Addadadadadad");
                _playerBody.velocity = new Vector2(_playerBody.velocity.x, jumpSpeed);
            }
            else
            {
                print("bbbbbbb");
                _playerBody.velocity = new Vector2(_playerBody.velocity.x, _playerBody.velocity.y + jumpSpeed);
                _canDoubleJump = false;
            }
        }

        private void PlayerWalkLeft()
        {
            TurnLeft();
            if (!IsGrounded()) return;
            
            _playerBody.velocity = new Vector2(-walkSpeed, _playerBody.velocity.y);
            _playerAnimator.SetTrigger(Walk);
        }

        private void PlayerWalkRight()
        {
            TurnRight();
            if (!IsGrounded()) return;
            
            _playerBody.velocity = new Vector2(walkSpeed, _playerBody.velocity.y);
            _playerAnimator.SetTrigger(Walk);
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
            var raycastHit = Physics2D.BoxCast(_playerBoxCollider.bounds.center, _playerBoxCollider.bounds.size, 0, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
            return raycastHit.collider is not null;
        }

        public void OnLeftButtonDown()
        {
            _isLeftButtonBeingClicked = true;
        }

        public void OnLeftButtonUp()
        {
            _isLeftButtonBeingClicked = false;
        }
        
        public void OnRightButtonDown()
        {
            _isRightButtonBeingClicked = true;
        }

        public void OnRightButtonUp()
        {
            _isRightButtonBeingClicked = false;
        }

        public void OnJumpButtonDown()
        {
            _isJumpButtonBeingClicked = true;
        }

        public void OnJumpButtonUp()
        {
            _isJumpButtonBeingClicked = false;
        }
        
        public void OnAttackButtonDown()
        {
            _isAttackButtonBeingClicked = true;
        }

        public void OnAttackButtonUp()
        {
            _isAttackButtonBeingClicked = false;
        }
    }
}
