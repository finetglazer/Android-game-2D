using UnityEngine;

namespace GameObjects.BubbleChat
{
    public class BubbleChatTrigger : MonoBehaviour
    {
        [SerializeField] private BubbleChatController bubbleChatController; // Reference to the BubbleChatController
        [SerializeField] private string message = "Hello, traveler!";       // The message to display
        [SerializeField] private float displayDuration = 3.0f;              // Duration to show the message

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if the object entering the trigger is the player
            if (other.CompareTag("Player"))
            {
                // Display the message in the bubble chat
                bubbleChatController.ShowMessage(message, displayDuration);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Optional: Hide the bubble chat when the player leaves the trigger area
            if (other.CompareTag("Player"))
            {
                bubbleChatController.HideBubbleChat();
            }
        }
    }
}