using UnityEngine;
using MainCharacter;  // Make sure this namespace matches where your PlayerDie45 script is located

public class Trap : MonoBehaviour
{
    // When the trap collides with something
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object colliding with the trap is the player
        PlayerDie45 player = other.GetComponent<PlayerDie45>();

        // If the player is detected, call the Die method on the player script
        if (player != null)
        {
            player.Die();
        }
    }
}