using System;
using Cinemachine;
using UnityEngine;

namespace GameObjects.InvisibleCamArea
{
    public class InvisibleCam : MonoBehaviour
    {
        // public CinemachineVirtualCamera targetCamera; // Reference to the target virtual camera

        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     // Check if the player touched the checkpoint
        //     if (!other.CompareTag("Player")) return;
        //     
        //     SwitchToTargetCamera();
        //
        //     print("Invisible checkpoint activated at: " + transform.position);
        // }
        //
        // private void SwitchToTargetCamera()
        // {
        //     // Disable all other virtual cameras and enable the target camera
        //     var allCameras = FindObjectsOfType<CinemachineVirtualCamera>();
        //
        //     foreach (var cam in allCameras)
        //     {
        //         cam.Priority = cam == targetCamera ? 10 : 0; // Set target camera priority higher to activate it
        //     }
        // }

        public CinemachineVirtualCamera targetCam;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            SetPriorityCam();
            
        }

        private void SetPriorityCam()
        {
            // set priority for the target cam 10 and other 0
            var allCameras = FindObjectsOfType<CinemachineVirtualCamera>();
            foreach (var VARIABLE in allCameras) 
            {
                if (VARIABLE == targetCam)
                {
                    VARIABLE.Priority = 10;
                } else 
                    VARIABLE.Priority = 0;
            }
        }
    }
}