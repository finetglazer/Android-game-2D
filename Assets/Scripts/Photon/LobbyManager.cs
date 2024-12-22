using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Photon
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        // UI elements
        [Header("Player Slots")]
        public GameObject[] playerSlots;

        [Header("Player Icons and Names")]
        public Image[] playerIcons; // Assign via Inspector
        public TMP_Text[] playerNames; // Assign via Inspector

        [Header("Buttons")]
        public Button startMatchButton;
        public Button leaveRoomButton; // Renamed for clarity

        [Header("UI Texts")]
        public TMP_Text passcodeText;

        [Header("Icons")]
        public Sprite masterIcon;
        public Sprite readyIcon;
        public Sprite waitingIcon;

        // Spawn points (optional)
        public Transform[] spawnPoints;

        // Ready Status Tracking
        private Dictionary<int, bool> playerReadyStatus = new Dictionary<int, bool>();

        private PhotonView _photonView;

        private void Awake()
        {
            // Ensure the PhotonView is correctly assigned.
            _photonView = GetComponent<PhotonView>();
        }

        private void Start()
        {
            // Attach listener to the Start Match button
            if (startMatchButton != null)
                startMatchButton.onClick.AddListener(OnStartMatchButtonClicked);
            else
                Debug.LogError("Start Match Button is not assigned in the Inspector.");

            // Attach listener to the Leave Room button
            if (leaveRoomButton != null)
                leaveRoomButton.onClick.AddListener(OnLeaveRoomButtonClicked);
            else
                Debug.LogError("Leave Room Button is not assigned in the Inspector.");

            // Initialize button based on client role
            InitializeStartMatchButton();

            if (PhotonNetwork.CurrentRoom != null)
            {
                passcodeText.text = $"Passcode: {PhotonNetwork.CurrentRoom.Name}"; // room name
            }
            else
            {
                passcodeText.text = "Not in a room";
            }

            // Update the player slots
            UpdatePlayerSlots();

            // Register to the scene loaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
            
           

            
        }
        
        void AssignSpawnIndicesToAllPlayers()
        {
            
            // Make sure you have the same spawnPoints array or logic available here.
            // For simplicity, we’ll just assign indices in order of PlayerList.
            Player[] players = PhotonNetwork.PlayerList;
            Debug.LogWarning(players.Length);            
            for (int i = 0; i < players.Length; i++)
            {
                int spawnIndex = i % spawnPoints.Length;
                ExitGames.Client.Photon.Hashtable spawnProps = new ExitGames.Client.Photon.Hashtable();
                spawnProps["SpawnIndex"] = spawnIndex;
                players[i].SetCustomProperties(spawnProps);
                Debug.Log("Assignin spawn index to player " + players[i].NickName);
            }
        }

        private void OnDestroy()
        {
            // Unregister from the scene loaded event to avoid memory leaks
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            UpdatePlayerSlots();
            UpdateStartMatchButtonInteractable();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            // Remove the player's ready status
            if (playerReadyStatus.ContainsKey(otherPlayer.ActorNumber))
                playerReadyStatus.Remove(otherPlayer.ActorNumber);
            UpdatePlayerSlots();
            UpdateStartMatchButtonInteractable();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
            UpdatePlayerSlots();
            InitializeStartMatchButton(); // Reinitialize the start button for the new master
            UpdateStartMatchButtonInteractable();
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            PhotonNetwork.JoinLobby();
            // Load the previous scene (Dashboard, for example) after leaving the room
            PhotonNetwork.LoadLevel("Scenes/DashboardScene");
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            UpdatePlayerSlots();
            UpdateStartMatchButtonInteractable();
        }

        void InitializeStartMatchButton()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                startMatchButton.GetComponentInChildren<TMP_Text>().text = "Start Match";
                // Initially disable the button until all players are ready
                startMatchButton.interactable = PhotonNetwork.CurrentRoom.PlayerCount >= 2;
                UpdateStartMatchButtonInteractable();
            }
            else
            {
                // Check if the player is already ready
                bool isReady = playerReadyStatus.ContainsKey(PhotonNetwork.LocalPlayer.ActorNumber) && playerReadyStatus[PhotonNetwork.LocalPlayer.ActorNumber];
                startMatchButton.GetComponentInChildren<TMP_Text>().text = isReady ? "Unready" : "Ready";
                // Non-master clients don't start the match, so ensure button is interactable
                startMatchButton.interactable = true;
            }
        }

        void OnStartMatchButtonClicked()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // Master Client: Start the match if all non-master clients are ready
                if (AreAllNonMasterPlayersReady())
                {
                    _photonView.RPC("StartMatch", RpcTarget.All);
                }
                else
                {
                    Debug.LogWarning("Not all players are ready.");
                }
            }
            else
            {
                // Non-Master Client: Toggle ready status
                ToggleReadyStatus();
            }
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
            UpdateStartMatchButtonInteractable();

            // If the RPC is for the local player and they are not the master, update the button text
            if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber && !PhotonNetwork.IsMasterClient)
            {
                startMatchButton.GetComponentInChildren<TMP_Text>().text = isReady ? "Unready" : "Ready";
            }
        }

        bool AreAllNonMasterPlayersReady()
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (!player.IsMasterClient)
                {
                    if (!playerReadyStatus.ContainsKey(player.ActorNumber) || !playerReadyStatus[player.ActorNumber])
                        return false;
                }
            }
            return PhotonNetwork.CurrentRoom.PlayerCount >= 2;
        }

        void UpdateStartMatchButtonInteractable()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                startMatchButton.interactable = AreAllNonMasterPlayersReady();
            }
            else
            {
                // Non-master clients' button is always interactable for readying/unreadying
                startMatchButton.interactable = true;
            }
        }

        void UpdatePlayerSlots()
        {
            if (playerSlots == null || playerSlots.Length == 0)
            {
                Debug.LogError("Player slots are not assigned in the Inspector.");
                return;
            }

            if (playerIcons == null || playerIcons.Length != playerSlots.Length)
            {
                Debug.LogError("Player icons are not correctly assigned in the Inspector.");
                return;
            }

            if (playerNames == null || playerNames.Length != playerSlots.Length)
            {
                Debug.LogError("Player names are not correctly assigned in the Inspector.");
                return;
            }

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
                        if (player.IsMasterClient)
                        {
                            playerIcons[i].sprite = masterIcon;
                        }
                        else if (playerReadyStatus.ContainsKey(player.ActorNumber) && playerReadyStatus[player.ActorNumber])
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

        [PunRPC]
        void StartMatch()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                AssignSpawnIndicesToAllPlayers(); // Assign here when all players are present and ready
            }
            // Load the SoloScene for all players
            PhotonNetwork.LoadLevel("SoloScene"); // Ensure SoloScene is added to Build Settings
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "SoloScene")
            {
                
            }
        }

        // Method to handle Leave Room button click
        void OnLeaveRoomButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
            // Scene loading is handled in the OnLeftRoom callback
        }
    }
}
