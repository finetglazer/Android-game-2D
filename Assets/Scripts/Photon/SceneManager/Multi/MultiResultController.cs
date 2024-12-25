using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using Photon.Realtime;


namespace Photon.SceneManager.Multi
{
    public class MultiResultController: MonoBehaviourPunCallbacks
    {
        [Header("Player Slots")]
        public GameObject[] playerSlots;
        
        [Header("Player Names")]
        public TMP_Text[] playerNames; // Assign via Inspector
        
        [Header("Button")]
        public Button leaveButton; // Assign via Inspector
        
        private bool isLeaving = false;
        
        private PhotonView _photonView;
        
        
        private void Awake()
        {
            // Ensure the PhotonView is correctly assigned.
            _photonView = GetComponent<PhotonView>();
        }
        
        private void Start()
        {
            // Update the player slots
            UpdatePlayerSlots();
            
            if (leaveButton != null)
            {
                leaveButton.onClick.RemoveAllListeners();
                leaveButton.onClick.AddListener(LeaveRoom);
            }
            else
            {
                Debug.LogError("Leave Button is not assigned in the Inspector.");
            }

        }
        
        private void LeaveRoom()
        {
            if (isLeaving) return;

            if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
            {
                isLeaving = true;
                if (leaveButton != null)
                {
                    leaveButton.interactable = false; 
                }
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                Debug.LogWarning("Cannot leave room: Client is not connected or not in a room.");
            }
        }
        
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            PhotonNetwork.JoinLobby();
            PhotonNetwork.LoadLevel("Scenes/DashboardScene");
        }
        
        void UpdatePlayerSlots()
        {
            int currentPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;

            for (int i = 0; i < playerSlots.Length; i++)
            {
                if (i < currentPlayerCount)
                {
                    Player player = PhotonNetwork.PlayerList[i];
                    playerSlots[i].SetActive(true);

                    if (playerNames[i] != null)
                        playerNames[i].text = player.NickName;
                    else
                        Debug.LogError($"NameText component not assigned for player slot {i}.");
                }
                else
                {
                    // Hide unused slots
                    playerSlots[i].SetActive(false);
                }
            }
        }

    }
}