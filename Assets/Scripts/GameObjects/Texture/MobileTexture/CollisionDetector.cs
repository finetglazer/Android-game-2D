using System;
using UnityEngine;

namespace GameObjects.Texture.MobileTexture
{
    public class CollisionDetector : MonoBehaviour
    {
        private Movement _blockMovement;

        private void Start()
        {
            _blockMovement = GetComponentInParent<Movement>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.name == "1") return; // If other is an overlapped box collider which is not triggered 
            
            var obj = other.gameObject;
            if (obj.GetComponent<Movement>() is null)
            {
                obj.AddComponent<Movement>();
            }
            var objController = obj.GetComponent<Movement>();
            objController.xVelocity = _blockMovement.xVelocity;
            objController.yVelocity = _blockMovement.yVelocity;
            objController.moveTime = _blockMovement.moveTime;
            objController.clock = _blockMovement.clock;
        }
    }
}