using MainCharacter;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Photon.Character
{
    public class MovementMultiplayer : MonoBehaviourPun, IPunObservable
    {
        public InputActionAsset inputActions;
        // public GameObject healthBar;
        public float walkSpeed = 2f;
        public float jumpSpeed = 5f;
        public float currentHealth = 15f; 
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

        // For network syncing
        private Vector2 networkPosition;
        private Vector2 networkVelocity;
        private float lerpRate = 10f;

        private void Start()
        {
            // Get components from the same GameObject this script is attached to
            _playerBody = GetComponent<Rigidbody2D>();
            _playerBoxCollider = GetComponent<BoxCollider2D>();
            _playerAnimator = GetComponent<Animator>();

            // Only enable input for local player
            if (photonView.IsMine)
            {
                _moveAction = inputActions.FindAction("Movement/Move");
                if (_moveAction != null)
                {
                    _moveAction.Enable();
                }
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
                // Interpolate remote player positions
                _playerBody.position = Vector2.Lerp(_playerBody.position, networkPosition, Time.deltaTime * lerpRate);
                _playerBody.velocity = networkVelocity;
            }
        }

        private void HandleInput()
        {
            if (_moveAction == null)
            {
                return; // Just in case _moveAction wasn't found
            }

            horizontalInput = _moveAction.ReadValue<Vector2>().x;

            if (IsGrounded())
            {
                canDoubleJump = true;
            }
            else
            {
                PlayerJump();
            }

            // Idle
            if (IsGrounded() && horizontalInput == 0)
            {
                PlayerIdle();
            }

            // Walking
            if (horizontalInput != 0)
            {
                if (horizontalInput < 0) TurnLeft();
                else TurnRight();

                if (IsGrounded())
                {
                    PlayerWalk();
                }
            }

            // Jump logic
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if (!canDoubleJump) return;

                if (!IsGrounded()) isDoubleJump = true;
                PlayerJump();
            }

            // Attack logic
            if (Input.GetMouseButtonDown(0) || Keyboard.current.uKey.wasPressedThisFrame)
            {
                PlayerAttack();
            }
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
            // Additional attack logic here
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
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        private void TurnRight()
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        private bool IsGrounded()
        {
            var raycastHit = Physics2D.Raycast(_playerBoxCollider.bounds.center, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));
            return raycastHit.collider != null;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_playerBody.position);
                stream.SendNext(_playerBody.velocity);
                stream.SendNext(currentHealth);
            }
            else
            {
                networkPosition = (Vector2)stream.ReceiveNext();
                networkVelocity = (Vector2)stream.ReceiveNext();
                currentHealth = (float)stream.ReceiveNext();
            }
        }
    }
}
