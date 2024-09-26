using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Buttons
{
    public class LeftButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public GameObject player;
        private MainCharacter.Movement _playerMovement;
        
        private void Start()
        {
            _playerMovement = player.GetComponent<MainCharacter.Movement>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _playerMovement.horizontalInput = -1;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _playerMovement.horizontalInput = 0;
        }
    }
}