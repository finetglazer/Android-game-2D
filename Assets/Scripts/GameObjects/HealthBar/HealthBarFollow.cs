using UnityEngine;


namespace GameObjects.HealthBar
{
    public class HealthBarFollow : MonoBehaviour
    {
        public Transform priestTransform;  // Reference to the Priest object (GameObject)
        public RectTransform healthBarTransform;  // The RectTransform of the health bar
        public UnityEngine.Camera mainCamera;  // Reference to the camera (likely Camera.main)

        public Vector3 offset = new (0, 1.5f, 0);  // Offset to position the health bar above the Priest

        private void Update()
        {
            // Convert the Priest's world position to screen position
            var worldPosition = priestTransform.position + offset;
            // var screenPosition = mainCamera.WorldToScreenPoint(worldPosition);   // For Canvas - World space mode

            var screenPosition = worldPosition;     // For Canvas - Screen space Camera mode
            // Set the health bar's position in the UI Canvas
            healthBarTransform.position = screenPosition;
        }
    }
}