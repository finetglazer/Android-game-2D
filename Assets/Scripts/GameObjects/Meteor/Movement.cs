using UnityEngine;
using UnityEngine.UIElements;

namespace GameObjects.Meteor
{
    public class Movement : MonoBehaviour
    {
        private MeteorController _meteorController;
        private float _fallAngle; // In degrees
        private float _fallSpeed;
        private float _disappearDepth;

        private void OnEnable()
        {
            _meteorController = GetComponentInParent<MeteorController>();
            _fallAngle = _meteorController.fallAngle;
            _fallSpeed = _meteorController.fallSpeed;
            _disappearDepth = _meteorController.disappearDepth;
            transform.position = _meteorController.initialPosition;
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