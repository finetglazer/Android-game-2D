#nullable enable
using UnityEngine;

namespace GameObjects.Texture.MobileTexture
{
    public class CollisionDetector : MonoBehaviour
    {
        private Movement? _blockMovement;

        private void Start()
        {
            _blockMovement = GetComponentInParent<Movement>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var obj = other.gameObject;
            var objMobileMovement = obj.GetComponent<Movement>() ;
            if (objMobileMovement is null)
            {
                obj.AddComponent<Movement>();
            }
            else
            {
                objMobileMovement.enabled = true;
            }
            objMobileMovement = obj.GetComponent<Movement>();
            objMobileMovement.xVelocity = _blockMovement!.xVelocity;
            objMobileMovement.yVelocity = _blockMovement.yVelocity;
            objMobileMovement.moveTime = _blockMovement.moveTime;
            objMobileMovement.clock = _blockMovement.clock;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var obj = other.gameObject;
            var objMobileMovement = other.GetComponent<Movement>(); 
            if (objMobileMovement is not null)
            {
                objMobileMovement.enabled = false;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            var obj = other.gameObject;
            var objMobileMovement = obj.GetComponent<Movement>();
            if (objMobileMovement is null)
            {
                obj.AddComponent<Movement>();
            }
            else
            {
                objMobileMovement.enabled = true;
            }
            objMobileMovement = obj.GetComponent<Movement>();
            objMobileMovement.xVelocity = _blockMovement!.xVelocity;
            objMobileMovement.yVelocity = _blockMovement.yVelocity;
            objMobileMovement.moveTime = _blockMovement.moveTime;
            objMobileMovement.clock = _blockMovement.clock;
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            var obj = other.gameObject;
            var objMobileMovement = obj.GetComponent<Movement>();
            if (objMobileMovement is not null)
            {
                objMobileMovement.enabled = false;
            }
        }
    }
}