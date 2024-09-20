using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDie : MonoBehaviour
{
    private Vector3 respawnPoint;

    private void Start()
    {
        // Set the default respawn point to the player's starting position
        respawnPoint = transform.position;
    }

    private void Update()
    {
        if (transform.position.y < -10)
        {
            Respawn();
        }
    }

    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        // Set the respawn point to the checkpoint's position
        respawnPoint = checkpointPosition;
    }

    public void Die()
    {
        // Call this method when the player dies
        Respawn();
    }

    private void Respawn()
    {
        // Move the player to the last checkpoint
        transform.position = respawnPoint;
        Debug.Log("Player respawned at: " + respawnPoint);
    }
}
