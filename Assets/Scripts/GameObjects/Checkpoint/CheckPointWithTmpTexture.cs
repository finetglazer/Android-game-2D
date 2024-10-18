using System.Collections.Generic;
using GameObjects.Texture.TemporaryTexture;
using MainCharacter;
using UnityEngine;

namespace GameObjects.Checkpoint
{
    public class CheckPointWithTmpTexture : MonoBehaviour
    {
        public float offset;
        public List<TemporaryTexture> tempTextures = new();
        // set up a reference to the player
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if the player touched the checkpoint
            if (!other.CompareTag("Player")) return;
            // Get the Player script and set the respawn position to this checkpoint
            var player = other.GetComponent<PlayerDie45>();
            if (player == null) return;
            // set transform position -10 x compared to the checkpoint
            // player.SetCheckpoint(transform.position);
            player.SetCheckpoint(new Vector3(transform.position.x + offset , transform.position.y, transform.position.z));
            print("Checkpoint activated at: " + transform.position);
            
        }
    
        // ReSharper disable Unity.PerformanceAnalysis
        public void ResetTextures()
        {
            foreach (var tempTexture in tempTextures)
            {
                if (tempTexture)
                {
                    tempTexture.ResetTexture();
                }
            }
        }
    }
}