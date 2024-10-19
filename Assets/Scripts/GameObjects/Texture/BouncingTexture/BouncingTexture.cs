using UnityEngine;

namespace GameObjects.Texture.BouncingTexture
{
    public class BouncingTexture : MonoBehaviour
    {
        // Force applied when the player touches the bouncing object
        public float bounceForce = 10f;

        // Detect when the player collides with the bouncing object
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Check if the object that collided is tagged as "Player"
            if (collision.gameObject.CompareTag("Player"))
            {
                // Get the Rigidbody2D of the player
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

                // Apply an upward force to make the player bounce
                if (playerRb != null)
                {
                    playerRb.velocity = new Vector2(playerRb.velocity.x, bounceForce);
                }
            }
        }
    }
}