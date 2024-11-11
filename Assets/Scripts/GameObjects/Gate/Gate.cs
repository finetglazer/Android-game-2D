using System;
using UnityEngine;

namespace GameObjects.Gate
{
    public class Gate : MonoBehaviour
    {
        // public Transform destinationGate; // The other gate's position
        //
        // private bool canTeleport = true; // To track if the player can teleport
        //
        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     if (!other.CompareTag("Player") || !canTeleport) return;
        //
        //     // Move the player to the destination gate's position
        //     // Offset the player's position by a small amount to prevent the player from immediately teleporting again
        //     other.transform.position = destinationGate.position + new Vector3(0, -2.5f, 0);
        //     
        // }

        public Transform destinationGate;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            other.transform.position = destinationGate.position + new Vector3(0, -2.5f, 0);
        }
    }
}