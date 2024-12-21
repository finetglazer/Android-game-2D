using UnityEngine;

namespace GameObjects.Fire
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