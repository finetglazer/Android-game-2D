using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ServerInteraction
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        // UI elements
        public GameObject[] playerSlots;
        public Button leaveRoomButton;
        public Button startMatchButton;
        public TMP_Text passcodeText;

        private int currentPlayerCount = 0;
        
        private void Start()
        {
            leaveRoomButton.onClick.AddListener(OnLeaveRoomButtonClicked);
            startMatchButton.onClick.AddListener(OnStartMatchButtonClicked);
            
            // Disable start match button initially
            startMatchButton.interactable = false;
            
            
            // Get passcode from PlayerPrefs (set earlier in the Dashboard scene)
            string passcode = PlayerPrefs.GetString("roomPasscode", "No Passcode");
            passcodeText.text = $"Passcode: {passcode}";

            // Update the player slots
            UpdatePlayerSlots();
            
        }
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            currentPlayerCount++;
            UpdatePlayerSlots();
            
            // Enable the start match button when there are 2 or more players
            if (currentPlayerCount >= 2)
            {
                startMatchButton.interactable = true;
            }
        }
        
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            currentPlayerCount--;
            UpdatePlayerSlots();
            
            // Disable the start match button when there are less than 2 players
            if (currentPlayerCount < 2)
            {
                startMatchButton.interactable = false;
            }
        }

        void UpdatePlayerSlots()
        {
            int playerIndex = 0;

            foreach (var slot in playerSlots) 
            {
                slot.SetActive(playerIndex < currentPlayerCount);
                playerIndex++;
            }
        }
        
        void OnLeaveRoomButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
            // Load the previous scene (Dashboard, for example)
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/DashboardScene");
        }
        
        void OnStartMatchButtonClicked()
        {
            if (currentPlayerCount >= 2)
            {
                // Load the game scene
                PhotonNetwork.LoadLevel("EndingScene");
            }
        }

    }
}