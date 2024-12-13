using MainCharacter;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Photon.Character
{
    public class MovementMultiplayer : MonoBehaviourPun, IPunObservable
    {
        public InputActionAsset inputActions;
        public GameObject player;
        public GameObject healthBar;
        public float walkSpeed = 2f;
        public float jumpSpeed = 2f;
        public float currentHealth = 15f; // Adjusted for multiplayer
        public static float SceneTime;
        private static readonly int Walk = Animator.StringToHash("walk");
        private static readonly int Attack = Animator.StringToHash("attack");
        private static readonly int Casting = Animator.StringToHash("casting");
        private static readonly int Victory = Animator.StringToHash("victory");
        private static readonly int Idle = Animator.StringToHash("idle");
        private static readonly int Jump = Animator.StringToHash("jump");
        private InputAction _moveAction;
        private Rigidbody2D _playerBody;
        private BoxCollider2D _playerBoxCollider;
        private Animator _playerAnimator;

        [HideInInspector] public bool isDoubleJump;
        [HideInInspector] public float horizontalInput;
        [HideInInspector] public bool canDoubleJump;
        [HideInInspector] public int deathCount;

        // Network synchronization variables
        private Vector2 networkPosition;
        private Vector2 networkVelocity;
        private float lerpRate = 10f;

        private void Start()
        {
            _playerBody = player.GetComponent<Rigidbody2D>();
            _playerBoxCollider = player.GetComponent<BoxCollider2D>();
            _playerAnimator = player.GetComponent<Animator>();

            // Initialize input actions only for the local player
            if (photonView.IsMine)
            {
                _moveAction = inputActions.FindAction("Movement/Move");
                _moveAction.Enable();
            }
            else
            {
                // Disable scripts for remote players if necessary
                healthBar.SetActive(false); // Hide health bar for remote players
            }
        }

        private void Update()
        {
            SceneTime += Time.deltaTime;

            if (photonView.IsMine)
            {
                HandleInput();
            }
            else
            {
                // Smoothly interpolate remote players' positions
                _playerBody.position = Vector2.Lerp(_playerBody.position, networkPosition, Time.deltaTime * lerpRate);
                _playerBody.velocity = networkVelocity;
            }

            // Common updates
            if (currentHealth <= 0 && photonView.IsMine)
            {
                healthBar.SetActive(false);
            }

            if (_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("die"))
            {
                if (photonView.IsMine)
                {
                    gameObject.GetComponent<PlayerDie>().Die();
                    ++deathCount;
                }
                PlayerIdle();
                return;
            }
        }

        private void HandleInput()
        {
            horizontalInput = _moveAction.ReadValue<Vector2>().x;

            if (IsGrounded())
            {
                canDoubleJump = true;
            }
            else
            {
                PlayerJump();
            }

            // Idle state when no horizontal input
            if (IsGrounded() && horizontalInput == 0)
            {
                PlayerIdle();
            }

            // Walking movement
            if (horizontalInput != 0)
            {
                if (horizontalInput < 0) TurnLeft();
                else TurnRight();
                if (IsGrounded())
                {
                    PlayerWalk();
                }
            }

            // Double Jump logic
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if (!canDoubleJump) return;

                if (!IsGrounded()) isDoubleJump = true;
                PlayerJump();
            }

            // Attack logic
            if (Input.GetMouseButtonDown(0)) // Mouse click
            {
                PlayerAttack();
            }

            if (Keyboard.current.uKey.wasPressedThisFrame) // Keyboard attack
            {
                PlayerAttack();
            }
        }

        // Player actions
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
            _playerBody.velocity = new Vector2(horizontalInput * walkSpeed, _playerBody.velocity.y);
        }

        public void PlayerAttack()
        {
            _playerAnimator.SetTrigger(Attack);
            // Implement attack logic (e.g., instantiate attack effects, detect hits)
        }

        public void PlayerJump()
        {
            _playerAnimator.SetTrigger(Jump);
            if (IsGrounded())
            {
                _playerBody.velocity = new Vector2(_playerBody.velocity.x, jumpSpeed);
            }
            else if (isDoubleJump)
            {
                _playerBody.velocity = new Vector2(_playerBody.velocity.x, jumpSpeed);
                canDoubleJump = false;
                isDoubleJump = false;
            }
        }

        private void TurnLeft()
        {
            player.transform.localScale = new Vector3(-Mathf.Abs(player.transform.localScale.x), player.transform.localScale.y, player.transform.localScale.z);
        }

        private void TurnRight()
        {
            player.transform.localScale = new Vector3(Mathf.Abs(player.transform.localScale.x), player.transform.localScale.y, player.transform.localScale.z);
        }

        private bool IsGrounded()
        {
            var raycastHit = Physics2D.Raycast(_playerBoxCollider.bounds.center, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));
            return raycastHit.collider != null;
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

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Send data to others
                stream.SendNext(_playerBody.position);
                stream.SendNext(_playerBody.velocity);
                stream.SendNext(currentHealth);
                // Add more variables if needed (e.g., animation states)
            }
            else
            {
                // Receive data from others
                networkPosition = (Vector2)stream.ReceiveNext();
                networkVelocity = (Vector2)stream.ReceiveNext();
                currentHealth = (float)stream.ReceiveNext();
                // Update more variables if needed
            }
        }
    }
}
