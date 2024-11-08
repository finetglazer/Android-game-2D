using UnityEngine;

namespace MainCharacter
{
    public class PreparedMovementScenario : MonoBehaviour
    {
        public float playerUncontrolledDuration = 3f;
        public float walkSpeedInUncontrolledState = 2f;
        private GameObject _player;
        private GameObject _leftButton;
        private GameObject _rightButton;
        private GameObject _attackButton;
        private GameObject _jumpButton;
        private float _clock;
        private float _initialWalkSpeed;

        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _initialWalkSpeed = _player.GetComponent<Movement>().walkSpeed;
            _leftButton = GameObject.Find("LeftButton");
            _rightButton = GameObject.Find("RightButton");
            _attackButton = GameObject.Find("AttackButton");
            _jumpButton = GameObject.Find("JumpButton");
            
            // _leftButton.GetComponent<Button>().enabled = false;
            // _rightButton.GetComponent<Button>().enabled = false;
            // _attackButton.GetComponent<Button>().enabled = false;
            // _jumpButton.GetComponent<Button>().enabled = false;
            
            _leftButton.gameObject.SetActive(false);
            _rightButton.gameObject.SetActive(false);
            _attackButton.gameObject.SetActive(false);
            _jumpButton.gameObject.SetActive(false);
            
        }

        private void Update()
        {
            // Logic here
            // Example:
            _clock += Time.deltaTime;
            if (_clock > playerUncontrolledDuration)
            {
                Release();
                return;
            }

            var playerMovement = _player.GetComponent<Movement>();
            playerMovement.horizontalInput = 1f;        // Comment " horizontalInput = Input.GetAxis("Horizontal"); " to use
            playerMovement.walkSpeed = walkSpeedInUncontrolledState;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void Release()
        {
            var playerMovement = _player.GetComponent<Movement>();
            playerMovement.horizontalInput = 0;
            playerMovement.walkSpeed = _initialWalkSpeed;
            _leftButton.gameObject.SetActive(true);
            _rightButton.gameObject.SetActive(true);
            _attackButton.gameObject.SetActive(true);
            _jumpButton.gameObject.SetActive(true);
            
            // _leftButton.GetComponent<Button>().enabled = true;
            // _rightButton.GetComponent<Button>().enabled = true;
            // _attackButton.GetComponent<Button>().enabled = true;
            // _jumpButton.GetComponent<Button>().enabled = true;
        }
    }
}