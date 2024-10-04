using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Buttons
{
    public class JumpButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public GameObject player;
        private MainCharacter.Movement _playerMovement;
        private int _clickCount;
        private bool _canJump;
        
        private void Start()
        {
            _playerMovement = player.GetComponent<MainCharacter.Movement>();
        }

        private void Update()
        {
            _canJump = _playerMovement.canDoubleJump;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _clickCount++;
            if (!_canJump)
            {
                _clickCount = 0;
                return;
            }
            
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