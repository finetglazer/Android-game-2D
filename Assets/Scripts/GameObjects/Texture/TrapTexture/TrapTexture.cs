using MainCharacter;
using UnityEngine;

namespace GameObjects.Texture.TrapTexture
{
    public class Trap : MonoBehaviour
    {
        // When the trap collides with something
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if the object colliding with the trap is the player
            var player = other.GetComponent<PlayerDie45>();

            // If the player is detected, call the Die method on the player script
            if (player)
            {
                player.Die();
            }
        
            // Check if enemy is detected
            if (other.tag.Contains("Enemy"))
            {
                other.gameObject.SetActive(false);               
            }
        }
    }
}