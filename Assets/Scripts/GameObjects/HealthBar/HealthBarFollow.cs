using UnityEngine;
using UnityEngine.Serialization;

namespace GameObjects.HealthBar
{
    public class HealthBarFollow : MonoBehaviour
    {
        public Transform priestTransform;  // Reference to the Priest object (GameObject)
        public RectTransform healthBarSystemTransform;  // The RectTransform of the health bar
        public UnityEngine.Camera mainCamera;  // Reference to the camera (likely Camera.main)

        public Vector3 offset = new Vector3(0, 1.5f, 0);  // Offset to position the health bar above the Priest

        private void Update()
        {
            // Convert the Priest's world position to screen position
            var worldPosition = priestTransform.position + offset;  // Add the offset to place the health bar above
            var screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

            // Set the health bar's position in the UI Canvas
            healthBarSystemTransform.position = screenPosition;
        }
    }
}