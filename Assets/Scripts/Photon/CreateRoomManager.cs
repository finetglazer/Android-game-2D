using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Photon
{
    public class CreateRoomManager : MonoBehaviourPunCallbacks
    {
        [Header("Buttons")]
        public Button soloModeButton;
        public Button multiModeButton;
        public Button backButton;   
     

        private bool isSceneLoading = false;

        private void Start()
        {
            soloModeButton.onClick.AddListener(OnSoloModeClicked);
            multiModeButton.onClick.AddListener(OnMultiModeClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
        
        void OnBackButtonClicked()
        {
            
            PhotonNetwork.LoadLevel("Scenes/DashboardScene");
         
        }

      

        void OnSoloModeClicked()
        {
            Debug.Log("CreateRoomManager started.");
            // Create a room for solo mode
            // For solo mode, let’s say MaxPlayers = 2. Adjust if needed.
            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = 2,
                IsVisible = true,
                IsOpen = true
            };

            string roomName = Random.Range(1000, 9999).ToString();
            PhotonNetwork.CreateRoom(roomName, roomOptions);
            Debug.Log("Creating a solo mode room...");
        }

        void OnMultiModeClicked()
        {
            // Future implementation for multi mode
            // For now, just a placeholder
            Debug.Log("Multi mode not implemented yet.");
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            Debug.Log("Solo mode room created successfully! Loading SoloLobbyScene...");
            if (!isSceneLoading)
            {
                isSceneLoading = true;
                PhotonNetwork.LoadLevel("SoloLobbyScene");
            }
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            Debug.LogError("Solo mode room creation failed: " + message);
            // Handle errors (e.g., show a UI message)
        }
    }
}