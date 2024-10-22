using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Buttons
{
    public class AttackButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public GameObject player;
        [HideInInspector] public bool isBeingClicked;
        private MainCharacter.Movement _playerMovement;

        private void Start()
        {
            _playerMovement = player.GetComponent<MainCharacter.Movement>();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            isBeingClicked = true;
            _playerMovement.PlayerAttack();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isBeingClicked = false;
        }
    }
}