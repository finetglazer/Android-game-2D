using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Photon
{
    public class LobbyChat : MonoBehaviourPunCallbacks
    {
        public TMP_InputField chatInputField;
        public TMP_Text chatText;
        public Button sendButton;
        public Transform contentTransform;

        public GameObject messagePrefab;
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
                // send the message to the Photon network
                photonView.RPC("BroadcastChatMessage", RpcTarget.All, message);
                
                // Clear the input field
                chatInputField.text = "";
            }
            
            else
            {
                chatText.text = "Please enter a message!";
            }
        }
        
        [PunRPC]
        private void BroadcastChatMessage(string message)
        {
            // Debugging to check if the references are assigned
            Debug.Log("Message Prefab: " + (messagePrefab != null ? "Assigned" : "Not Assigned"));
            Debug.Log("Content Transform: " + (contentTransform != null ? "Assigned" : "Not Assigned"));

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

            // Get the player's name from Photon
            string playerName = PhotonNetwork.NickName;

            // Format the message with the player's name
            string formattedMessage = $"{playerName}: {message}";

            // Check if the prefab contains the TMP_Text component
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

            messageText.text = formattedMessage;  // Set the player's name and message

            // Ensure the Content Transform has a ScrollRect
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