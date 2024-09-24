using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Buttons
{
    public class MoveButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public float moveSpeed = 5f;
        private bool _isMovingLeft;
        private bool _isMovingRight;
        public GameObject player;
        public void OnPointerDown(PointerEventData eventData)
        {
            switch (gameObject.name)
            {
                // Replace with your actual left button name
                case "left":
                    _isMovingLeft = true;
                    break;
                // Replace with your actual right button name
                case "right":
                    _isMovingRight = true;
                    break;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isMovingLeft = false;
            _isMovingRight = false;
        }

        private void Update()
        {
            if (_isMovingLeft) MoveLeft();
            if (_isMovingRight) MoveRight();
        }

        private void MoveLeft()
        {
            player.transform.position += Vector3.left * (moveSpeed * Time.deltaTime);
        }

        private void MoveRight()
        {
            player.transform.position += Vector3.right * (moveSpeed * Time.deltaTime);
        }
    }
}