using TMPro;
using UnityEngine;

namespace GameObjects.BubbleChat
{
    public class BubbleChatController : MonoBehaviour
    {
        public TextMeshProUGUI chatText;       // Reference to the Text component
        public GameObject bubbleChatObject;    // Reference to the bubble chat Image

        private Transform playerTransform;     // Reference to the player's transform
        private Vector3 originalScale;         // Original scale of the bubble chat

        void Start()
        {
            // Hide the bubble chat initially
            bubbleChatObject.SetActive(false);

            // Store the original scale of the bubble chat
            originalScale = transform.localScale;

            // Get the player's transform reference
            playerTransform = transform.parent; // Assuming the bubble chat is a child of the player
        }

        void Update()
        {
            // Keep the bubble chat facing the correct direction
            if (playerTransform.localScale.x < 0)
            {
                // If the player is facing left, set the bubble chat's X scale to stay positive
                transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
            }
            else
            {
                // If the player is facing right, restore the original scale
                transform.localScale = originalScale;
            }
        }

        public void ShowMessage(string message, float duration)
        {
            Debug.Log("Message to display: " + message);
            chatText.text = message;               // Set the chat message
            bubbleChatObject.SetActive(true);      // Display the bubble chat
            CancelInvoke("HideBubbleChat");        // Cancel any previous hide calls
            Invoke("HideBubbleChat", duration);    // Hide the bubble chat after 'duration' seconds
        }

        public void HideBubbleChat()
        {
            bubbleChatObject.SetActive(false);     // Hide the bubble chat
        }
    }
}
