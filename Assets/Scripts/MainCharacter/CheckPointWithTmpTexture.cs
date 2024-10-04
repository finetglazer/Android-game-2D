using UnityEngine;
using System.Collections.Generic;
using MainCharacter;

public class CheckPointWithTmpTexture : MonoBehaviour
{
    public float offset = 0;
    public List<TemporaryTexture> tempTextures = new List<TemporaryTexture>();
    // set up a reference to the player
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player touched the checkpoint
        if (other.CompareTag("Player"))
        {
            // Get the Player script and set the respawn position to this checkpoint
            PlayerDie45 player = other.GetComponent<PlayerDie45>();
            if (player != null)
            {
                // set transform position -10 x compared to the checkpoint
                // player.SetCheckpoint(transform.position);
                player.SetCheckpoint(new Vector3(transform.position.x + offset , transform.position.y, transform.position.z), this);
                Debug.Log("Checkpoint activated at: " + transform.position);
            }
        }
    }
    
    public void ResetTextures()
    {
        foreach (var tempTexture in tempTextures)
        {
            if (tempTexture != null)
            {
                tempTexture.ResetTexture();
            }
        }
    }
}