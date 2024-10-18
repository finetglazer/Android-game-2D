using UnityEngine;
using UnityEngine.UIElements;

namespace GameObjects.Meteor
{
    public class Movement : MonoBehaviour
    {
        private Controller.Controller _controller;
        private float _fallAngle; // In degrees
        private float _fallSpeed;
        private float _disappearDepth;

        private void OnEnable()
        {
            _controller = GetComponentInParent<Controller.Controller>();
            _fallAngle = _controller.fallAngle;
            _fallSpeed = _controller.fallSpeed;
            _disappearDepth = _controller.disappearDepth;
            transform.position = _controller.initialPosition;
        }
        
        private void Update()
        {
            if (transform.position.y < _disappearDepth)
            {
                gameObject.SetActive(false);
                return;
            }
            var angleInRadians = Mathf.PI / 180 * _fallAngle;
            transform.Translate(new Vector2(_fallSpeed * Mathf.Sin(angleInRadians), -_fallSpeed * Mathf.Cos(angleInRadians)));
        }
    }
}