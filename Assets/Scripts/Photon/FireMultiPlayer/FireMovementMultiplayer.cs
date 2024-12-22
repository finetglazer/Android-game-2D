using UnityEngine;

namespace Photon.FireMultiPlayer
{
    public class FireMovementMultiplayer : MonoBehaviour
    {
        public float currentSpeed = 1f;
        
        private void Update()
        {
            transform.Translate(new Vector2(currentSpeed, 0));
        }
    }
}