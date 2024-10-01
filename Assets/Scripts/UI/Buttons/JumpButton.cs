using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Buttons
{
    public class JumpButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public GameObject player;
        private MainCharacter.Movement _playerMovement;
        private bool _canDoubleJump;
        private int _clickCount;
        
        private void Start()
        {
            _playerMovement = player.GetComponent<MainCharacter.Movement>();
            _clickCount = 0;
        }

        private void Update()
        {
            _canDoubleJump = _playerMovement.canDoubleJump;
            _clickCount = _playerMovement.IsGrounded() ? 0 : _clickCount;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            ++_clickCount;
            if (!_canDoubleJump) return;

            if (_clickCount == 2)
            {
                _playerMovement.isDoubleJump = true;
                _clickCount = 0;
            }
            _playerMovement.PlayerJump();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            
        }
    }
}