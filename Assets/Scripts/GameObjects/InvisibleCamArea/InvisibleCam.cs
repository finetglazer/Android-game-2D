using Cinemachine;
using UnityEngine;

namespace GameObjects.InvisibleCamArea
{
    public class InvisibleCam : MonoBehaviour
    {
        public CinemachineVirtualCamera targetCamera; // Reference to the target virtual camera

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if the player touched the checkpoint
            if (!other.CompareTag("Player")) return;
            
            SwitchToTargetCamera();

            print("Invisible checkpoint activated at: " + transform.position);
        }

        private void SwitchToTargetCamera()
        {
            // Disable all other virtual cameras and enable the target camera
            var allCameras = FindObjectsOfType<CinemachineVirtualCamera>();

            foreach (var cam in allCameras)
            {
                cam.Priority = cam == targetCamera ? 10 : 0; // Set target camera priority higher to activate it
            }
        }
    }
}