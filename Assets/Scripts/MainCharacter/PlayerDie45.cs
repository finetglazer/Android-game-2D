﻿using Recorder;
using UnityEngine;

namespace MainCharacter
{
    public class PlayerDie45 : MonoBehaviour
    {
        public float deathPoint;
        private Vector3 _respawnPoint;
        

        private void Start()
        {
            // Set the default respawn point to the player's starting position
            _respawnPoint = transform.position;
        }

        private void Update()
        {
            if (!(transform.position.y < deathPoint)) return;
            
            print("Below Death Point: " + transform.position.y);
            Respawn();
        }

        public void SetCheckpoint(Vector3 checkpointPosition)
        {
            // Set the respawn point to the checkpoint's position
            _respawnPoint = checkpointPosition;
            // _currentCheckPointWithTmpTexture = checkPointWithTmpTexture;
        }
        
        public void Die()
        {
            Respawn();
        }

        private void Respawn()
        {
            // Move the player to the last checkpoint
            transform.position = _respawnPoint;
            print("Player respawned at: " + _respawnPoint);

            // Reset all temporary textures related to the current checkpoint
            // if (_currentCheckPointWithTmpTexture is null) return;
            // print("Resetting textures");
            // _currentCheckPointWithTmpTexture.ResetTextures();
            DeathNote.ReRender();
        }
    }
}