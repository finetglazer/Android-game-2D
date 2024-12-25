﻿using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.CharSelectionRoom
{
    public class MultiCharSelectionManager : MonoBehaviourPunCallbacks
    {
        [Header("Player Slots")]
        public GameObject[] playerSlots;

        [Header("Player Icons and Names")]
        public Image[] playerIcons; // Assign via Inspector
        public TMP_Text[] playerNames; // Assign via Inspector

        [Header("Buttons")]
        public Button confirmButton; // Assign via Inspector

        [Header("Dropdowns")]
        public TMP_Dropdown characterDropdowns; // Assign via Inspector

        [Header("Icons")]
        public Sprite readyIcon;
        public Sprite waitingIcon;

        [Header("Timer")]
        public TMP_Text timeRecord; // Assign via Inspector

        private PhotonView _photonView;

        // Constants
        private const string TimerEndTimeKey = "TimerEndTime";
        private const int CountdownDuration = 5; // 30 seconds

        // Timer variables
        private double timerEndTime;
        private bool isTimerRunning;
        
        // Ready Status Tracking
        private Dictionary<int, bool> playerReadyStatus = new Dictionary<int, bool>();


        private void Awake()
        {
            // Ensure the PhotonView is correctly assigned.
            _photonView = GetComponent<PhotonView>();
        }

        private void Start()
        {
            StartCountdown();
            
            // Check if the TimerEndTime is already set in room properties
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(TimerEndTimeKey))
            {
                timerEndTime = (double)PhotonNetwork.CurrentRoom.CustomProperties[TimerEndTimeKey];
                isTimerRunning = true;
            }
            
            
            // Attach listener to the confirm button
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            
            // Update the player slots
            UpdatePlayerSlots();
            
            
            

        }

        private void Update()
        {
            if (isTimerRunning)
            {
                double remainingTime = timerEndTime - PhotonNetwork.Time;
                if (remainingTime > 0)
                {
                    timeRecord.text = "Time remains: " + Mathf.Ceil((float)remainingTime).ToString() + "s";
                }
                else
                {
                    timeRecord.text = "0s";
                    isTimerRunning = false;
                    OnTimerEnded();
                }
            }
        }

        /// <summary>
        /// Method to start the countdown timer.
        /// Only the Master Client should call this method to initiate the timer.
        /// </summary>
        public void StartCountdown()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            double currentTime = PhotonNetwork.Time;
            timerEndTime = currentTime + CountdownDuration;

            // Create a Hashtable to update the room properties
            Hashtable props = new Hashtable
            {
                { TimerEndTimeKey, timerEndTime }
            };

            // Set the custom room properties
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }

        /// <summary>
        /// Callback when room properties are updated.
        /// This ensures that all clients update their timer based on the shared TimerEndTime.
        /// </summary>
        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey(TimerEndTimeKey))
            {
                timerEndTime = (double)propertiesThatChanged[TimerEndTimeKey];
                isTimerRunning = true;
            }
        }

        /// <summary>
        /// Action to perform when the timer ends.
        /// </summary>
        private void OnTimerEnded()
        {
            Debug.Log("Timer has ended!");
            // Example action: Load the game scene
            string typeOfRoom = PhotonNetwork.CurrentRoom.CustomProperties["type"].ToString();
            if (typeOfRoom == "easy")
            {
                PhotonNetwork.LoadLevel("MultiEasyScene");
            }
            else if (typeOfRoom == "medium")
            {
                PhotonNetwork.LoadLevel("MultiMediumScene");
            }
            else if (typeOfRoom == "nightmare")
            {
                PhotonNetwork.LoadLevel("MultiNightMareScene");
            }
            else if (typeOfRoom == "solo")
            {
                PhotonNetwork.LoadLevel("SoloScene");
            }
            
        }


        private void OnConfirmButtonClicked()
        {
            ToggleReadyStatus();
            confirmButton.interactable = false;
            UpdatePlayerSlots();
            OnConfirmSelection();
        }
        
        void OnConfirmSelection()
        {
            Debug.Log("Character selected: " + characterDropdowns.options[characterDropdowns.value].text);
            Hashtable props = PhotonNetwork.LocalPlayer.CustomProperties;
            props["Character"] = characterDropdowns.options[characterDropdowns.value].text;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            
            
        }

        void ToggleReadyStatus()
        {
            bool isReady = playerReadyStatus.ContainsKey(PhotonNetwork.LocalPlayer.ActorNumber) && playerReadyStatus[PhotonNetwork.LocalPlayer.ActorNumber];
            _photonView.RPC("SetPlayerReady", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, !isReady);
        }
        
        [PunRPC]
        void SetPlayerReady(int actorNumber, bool isReady)
        {
            if (isReady)
                playerReadyStatus[actorNumber] = true;
            else
                playerReadyStatus.Remove(actorNumber);

            UpdatePlayerSlots();
            // UpdateStartMatchButtonInteractable();
            
            Debug.Log("Player " + PhotonNetwork.LocalPlayer.NickName + " is " + (isReady ? "ready" : "not ready"));
            
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

                    if (playerIcons[i] != null)
                    {
                        if (playerReadyStatus.ContainsKey(player.ActorNumber) && playerReadyStatus[player.ActorNumber])
                        {
                            playerIcons[i].sprite = readyIcon;
                        }
                        else
                        {
                            playerIcons[i].sprite = waitingIcon;
                        }
                    }
                    else
                    {
                        Debug.LogError($"Icon Image component not assigned for player slot {i}.");
                    }
                }
                else
                {
                    // Hide unused slots
                    playerSlots[i].SetActive(false);
                }
            }
        }
        
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            PhotonNetwork.JoinLobby();
            // Load the previous scene (Dashboard, for example) after leaving the room
            PhotonNetwork.LoadLevel("Scenes/DashboardScene");
        }
        
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            // Remove the player's ready status
            if (playerReadyStatus.ContainsKey(otherPlayer.ActorNumber))
                playerReadyStatus.Remove(otherPlayer.ActorNumber);
            UpdatePlayerSlots();
        }
    }
    
    
}
