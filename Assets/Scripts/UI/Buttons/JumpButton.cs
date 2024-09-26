using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Buttons
{
    public class JumpButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public GameObject player;
        private MainCharacter.Movement _playerMovement;
        private bool _canDoubleJump;

        private void Start()
        {
            _playerMovement = player.GetComponent<MainCharacter.Movement>();
        }

        private void Update()
        {
            _canDoubleJump = _playerMovement.canDoubleJump || _playerMovement.IsGrounded();
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_canDoubleJump) return;
            _playerMovement.PlayerJump();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            
        }
    }
}