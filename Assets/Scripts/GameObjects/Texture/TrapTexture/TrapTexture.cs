using MainCharacter;
using UnityEngine;

namespace GameObjects.Texture.TrapTexture
{
    public class Trap : MonoBehaviour
    {
        private static readonly int Die = Animator.StringToHash("die");

        // When the trap collides with something
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if the object colliding with the trap is the player
            var player = other.GetComponent<PlayerDie>();

            // If the player is detected, call the Die method on the player script
            if (player)
            {
                player.Die();
            }
        
            // Check if enemy is detected
            if (!other.tag.Contains("Enemy")) return;
            var merchantMovement = other.GetComponent<OtherCharacters.Merchant.Movement>();
            var peasantMovement = other.GetComponent<OtherCharacters.Peasant.Movement>();
            var soldierMovement = other.GetComponent<OtherCharacters.Soldier.Movement>();
            var thiefMovement = other.GetComponent<OtherCharacters.Thief.Movement>();
            merchantMovement?.healthBar.gameObject.SetActive(false);
            peasantMovement?.healthBar.gameObject.SetActive(false);
            soldierMovement?.healthBar.gameObject.SetActive(false);
            thiefMovement?.healthBar.gameObject.SetActive(false);
            
            other.gameObject.SetActive(false);
        }
    }
}