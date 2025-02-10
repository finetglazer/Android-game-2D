using System.Collections;
using GameObjects.Fire;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using UnityEngine.Networking;
using UnityEngine.SceneManagement; // Import Photon.Pun for networking

namespace MainCharacter
{
    public class Movement : MonoBehaviourPunCallbacks
    {
        public InputActionAsset inputActions;
        public GameObject player;
        public GameObject healthBar;
        public float walkSpeed = 2;
        public float jumpSpeed = 2;
        public float currentHealth = 2;
        public static float SceneTime;
        private static readonly int Walk = Animator.StringToHash("walk");
        private static readonly int Attack = Animator.StringToHash("attack");
        private static readonly int Casting = Animator.StringToHash("casting");
        private static readonly int Victory = Animator.StringToHash("victory");
        private static readonly int Idle = Animator.StringToHash("idle");
        private static readonly int Jump = Animator.StringToHash("jump");
        private InputAction _inputAction;
        private Rigidbody2D _playerBody;
        private BoxCollider2D _playerBoxCollider;
        private Animator _playerAnimator;
        [HideInInspector] public bool isDoubleJump;
        [HideInInspector] public float horizontalInput;
        [HideInInspector] public bool canDoubleJump;
        [HideInInspector] public int deathCount;

        private void Start()
        {
            _playerBody = player.GetComponent<Rigidbody2D>();
            _playerBoxCollider = player.GetComponent<BoxCollider2D>();
            _playerAnimator = player.GetComponent<Animator>();
            _inputAction = inputActions.FindAction("Movement/Move");
            _inputAction.Enable();

            // Disable movement input and animation for other players
            // if (!photonView.IsMine)
            // {
            //     enabled = false;  // Disable this script for non-local players
            //     return;
            // }
        }

        private void Update()
        {
            // Check if this is the local player's object
            // if (!photonView.IsMine) return;

            SceneTime += Time.deltaTime;

            if (currentHealth <= 0)
            {
                healthBar.SetActive(false);
            }

            if (_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("die"))
            {
                Debug.Log("hihi");
                gameObject.GetComponent<PlayerDie>().Die();
                ++deathCount;
                PlayerIdle();
                if (SceneManager.GetActiveScene().name == "6thscene")
                {
                    var fire = GameObject.Find("FireWalls");
                    fire.transform.position = new Vector3(-0.930000007f, -0.589999974f, -0.0533737652f);
                    // get component movement of fire and set speed to 0.01f
                    var fireMovement = fire.GetComponent<GameObjects.Fire.Movement>();
                    fireMovement.currentSpeed= 0.013f;
                }
                StartCoroutine(CreateUpdateDeathCountRequest());
                return;
            }

            // Handle input for movement and jumping
            horizontalInput = _inputAction.ReadValue<Vector2>().x; // Keyboard mode - Uncomment this

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

        private IEnumerator CreateUpdateDeathCountRequest()
        {
            const string url = "http://localhost:8080/api/gameplay/update-death-count";
            var request = new UnityWebRequest(url, "POST");
            var userId = PlayerPrefs.GetString("userId");
            var jsonBody = "{\"userId\":\"" + userId + "\"}";  
            var jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            print(request.result == UnityWebRequest.Result.Success
                ? "+1 death time on server!"
                : request.downloadHandler.text);
        }
    }
}
