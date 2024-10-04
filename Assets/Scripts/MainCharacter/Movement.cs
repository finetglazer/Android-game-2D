using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace MainCharacter
{
    public class Movement : MonoBehaviour
    {
        public float walkSpeed = 2;
        public float jumpSpeed = 2;
        public float currentHealth = 2;
        public float horizontalDragCoefficient = 0.2f;
        public GameObject player;
        private static readonly int Walk = Animator.StringToHash("walk");
        private static readonly int Attack = Animator.StringToHash("attack");
        private static readonly int Casting = Animator.StringToHash("casting");
        private static readonly int Victory = Animator.StringToHash("victory");
        private static readonly int Idle = Animator.StringToHash("idle");
        private static readonly int Jump = Animator.StringToHash("jump");
        private Rigidbody2D _playerBody;
        private BoxCollider2D _playerBoxCollider;
        private Animator _playerAnimator;
        [HideInInspector] public bool isDoubleJump;
        [HideInInspector] public float horizontalInput;
        [HideInInspector] public bool canDoubleJump;

        private void Start()
        {
            _playerBody = player.GetComponent<Rigidbody2D>();
            _playerBoxCollider = player.GetComponent<BoxCollider2D>();
            _playerAnimator = player.GetComponent<Animator>();
        }
        
        private void Update()
        {
            if (_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("die"))
            {
                Destroy(this);
                return;
            }

            horizontalInput = Input.GetAxis("Horizontal");  // Uncomment this when using keyboard controlling player 

            if (IsGrounded())
            {
                canDoubleJump = true;
            }
            else
            {
                PlayerJump();
            }

            if (IsGrounded() && horizontalInput == 0)
            {
                PlayerIdle();
            }
            
            if (horizontalInput != 0)
            {
                if (horizontalInput < 0) TurnLeft(); else TurnRight();
                if (IsGrounded())
                {
                    PlayerWalk();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!canDoubleJump) return;
                
                if (!IsGrounded()) isDoubleJump = true;
                PlayerJump();
            }

            if (Input.GetMouseButtonDown(0))    // Uncomment this when using keyboard controlling player 
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
            _playerBody.velocity = new Vector2(0, _playerBody.velocity.y);
        }

        private void PlayerWalk()
        {
            _playerAnimator.SetTrigger(Walk);
            _playerBody.velocity = horizontalInput switch
            {
                > 0 => new Vector2(walkSpeed, _playerBody.velocity.y),
                < 0 => new Vector2(-walkSpeed, _playerBody.velocity.y),
                _ => _playerBody.velocity
            };
        }

        public void PlayerAttack()
        {
            _playerAnimator.SetTrigger(Attack);
        }

        public void PlayerJump()
        {
            _playerAnimator.SetTrigger(Jump);
            if (IsGrounded())
            {
                _playerBody.velocity = new Vector2(_playerBody.velocity.x * horizontalInput, jumpSpeed);
            }
            else if (isDoubleJump)
            {
                _playerBody.velocity = new Vector2(_playerBody.velocity.x + walkSpeed * horizontalInput, _playerBody.velocity.y + jumpSpeed);
                canDoubleJump = false;
                isDoubleJump = false;
            }
            else // When floating on air
            {
                _playerBody.velocity = new Vector2(walkSpeed * horizontalInput, _playerBody.velocity.y);
            }
        }
        
        private void TurnLeft()
        {
            player.transform.localScale = new Vector3(Mathf.Abs(player.transform.localScale.x), player.transform.localScale.y, player.transform.localScale.z);
        }
        
        private void TurnRight()
        {
            player.transform.localScale = new Vector3(-Mathf.Abs(player.transform.localScale.x), player.transform.localScale.y, player.transform.localScale.z);
        }

        private bool IsGrounded()
        {
            var raycastHit = Physics2D.Raycast(_playerBoxCollider.bounds.center, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));
            return raycastHit.collider is not null;
        }
        
        public void PlayerWalkLeft()
        {
            TurnLeft();
            if (!IsGrounded()) return;
            
            _playerAnimator.SetTrigger(Walk);
            _playerBody.velocity = new Vector2(-walkSpeed, _playerBody.velocity.y);
        }

        public void PlayerWalkRight()
        {
            TurnRight();
            if (!IsGrounded()) return;
            
            _playerAnimator.SetTrigger(Walk);
            _playerBody.velocity = new Vector2(walkSpeed, _playerBody.velocity.y);
        }
    }
}
