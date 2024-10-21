using UnityEngine;

namespace GameObjects.Fire
{
    public class Movement : MonoBehaviour
    {
        public float currentSpeed = 1f;
        public float speedIncrement = 1f;
        private const string SpeedAcceleratorName = "Accelerator";

        private void Update()
        {
            transform.Translate(new Vector2(currentSpeed, 0));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.name != SpeedAcceleratorName) return;
            currentSpeed += speedIncrement;
        }
    }
}