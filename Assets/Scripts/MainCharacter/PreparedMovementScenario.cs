using UnityEngine;
using UnityEngine.UI;

namespace MainCharacter
{
    public class PreparedMovementScenario : MonoBehaviour
    {
        public float playerUncontrolledDuration = 3f;
        public Sprite forbiddenLeftButtonImage;
        public Sprite forbiddenRightButtonImage;
        public Sprite forbiddenAttackButtonImage;
        public Sprite forbiddenJumpButtonImage;
        private GameObject _player;
        private GameObject _leftButton;
        private GameObject _rightButton;
        private GameObject _attackButton;
        private GameObject _jumpButton;
        private Sprite _leftButtonImage;
        private Sprite _rightButtonImage;
        private Sprite _attackButtonImage;
        private Sprite _jumpButtonImage;
        private float _clock;

        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _leftButton = GameObject.Find("LeftButton");
            _rightButton = GameObject.Find("RightButton");
            _attackButton = GameObject.Find("AttackButton");
            _jumpButton = GameObject.Find("JumpButton");

            _leftButtonImage = _leftButton.GetComponent<Image>().sprite;
            _rightButtonImage = _rightButton.GetComponent<Image>().sprite;
            _attackButtonImage = _attackButton.GetComponent<Image>().sprite;
            _jumpButtonImage = _jumpButton.GetComponent<Image>().sprite;
            
                
            _leftButton.GetComponent<Image>().sprite = forbiddenLeftButtonImage;
            _rightButton.GetComponent<Image>().sprite = forbiddenRightButtonImage;
            _attackButton.GetComponent<Image>().sprite = forbiddenAttackButtonImage;
            _jumpButton.GetComponent<Image>().sprite = forbiddenJumpButtonImage;
            _jumpButton.transform.rotation = Quaternion.Euler(0, 0, -90);
            
            _leftButton.GetComponent<Button>().enabled = false;
            _rightButton.GetComponent<Button>().enabled = false;
            _attackButton.GetComponent<Button>().enabled = false;
            _jumpButton.GetComponent<Button>().enabled = false;
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
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void Release()
        {
            _leftButton.GetComponent<Image>().sprite = _leftButtonImage;
            _rightButton.GetComponent<Image>().sprite = _rightButtonImage;
            _attackButton.GetComponent<Image>().sprite = _attackButtonImage;
            _jumpButton.GetComponent<Image>().sprite = _jumpButtonImage;
            
            _leftButton.GetComponent<Button>().enabled = true;
            _rightButton.GetComponent<Button>().enabled = true;
            _attackButton.GetComponent<Button>().enabled = true;
            _jumpButton.GetComponent<Button>().enabled = true;
        }
    }
}