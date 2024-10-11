using TMPro;
using UnityEngine;

// Make sure to use TextMeshPro namespace if you're using TMP

namespace GameObjects.BubbleChat
{
    public class BubbleChatController : MonoBehaviour
    {
        public TextMeshProUGUI chatText;       // Reference to the Text component
        public GameObject bubbleChatObject;    // Reference to the bubble chat Image

        void Start()
        {
            // Hide the bubble chat initially
            bubbleChatObject.SetActive(false);
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