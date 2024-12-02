using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Photon
{
    public class LobbyChat : MonoBehaviourPunCallbacks
    {
        public TMP_InputField chatInputField; // Input field for chat messages
        public TMP_Text chatText;             // Text component to show validation message (e.g., "Please enter a message!")
        public Button sendButton;             // Button for sending messages
        public Transform contentTransform;    // The parent transform that holds the chat messages

        public GameObject messagePrefab;      // Prefab to instantiate for each new message

        private void Start()
        {
            if (photonView == null)
            {
                Debug.LogError("PhotonView is not attached to the object!");
            }
            else
            {
                // Initialize the send button's functionality
                sendButton.onClick.AddListener(OnSendMessage);
            }
        }

        // Method to send a chat message
        private void OnSendMessage()
        {
            string message = chatInputField.text;

            if (!string.IsNullOrEmpty(message))
            {
                // Send the message to the Photon network, including the player's nickname
                photonView.RPC("BroadcastChatMessage", RpcTarget.All, message, PhotonNetwork.LocalPlayer.NickName);
                
                // Clear the input field after sending the message
                chatInputField.text = "";
            }
            else
            {
                chatText.text = "Please enter a message!";  // Show a message if the input is empty
            }
        }

        [PunRPC]
        private void BroadcastChatMessage(string message, string senderNickName)
        {
            // Debugging to check if the references are assigned
            Debug.Log("Message Prefab: " + (messagePrefab != null ? "Assigned" : "Not Assigned"));
            Debug.Log("Content Transform: " + (contentTransform != null ? "Assigned" : "Not Assigned"));

            // If prefab or content transform is not assigned, log error
            if (messagePrefab == null)
            {
                Debug.LogError("Message Prefab is not assigned!");
                return;
            }

            if (contentTransform == null)
            {
                Debug.LogError("Content Transform is not assigned!");
                return;
            }

            // Format the message with the player's name (senderNickName)
            string formattedMessage = $"<color=blue>{senderNickName}:</color> {message}";  // Use the passed nickname
            
            // Instantiate the message prefab in the content transform
            GameObject newMessage = Instantiate(messagePrefab, contentTransform);

            if (newMessage == null)
            {
                Debug.LogError("Failed to instantiate the message prefab!");
                return;
            }

            TMP_Text messageText = newMessage.GetComponent<TMP_Text>();

            if (messageText == null)
            {
                Debug.LogError("Message prefab does not contain a TMP_Text component! GameObject: " + newMessage.name);
                return;
            }

            // Set the formatted message text
            messageText.text = formattedMessage;

            // Ensure the Content Transform has a ScrollRect component (for auto-scrolling)
            ScrollRect scrollRect = contentTransform.GetComponentInParent<ScrollRect>();  // Get ScrollRect from the parent
            if (scrollRect == null)
            {
                Debug.LogError("ScrollRect component is not found in parent of Content Transform!");
                return;
            }

            // Optionally, automatically scroll down the chat to the newest message
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;  // Scroll to the bottom
        }
    }
}
