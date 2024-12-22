using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Cinemachine;

namespace Photon
{
    public class SoloLobbyManager : MonoBehaviourPunCallbacks
    {
        // UI elements
        [Header("Player Slots")]
        public GameObject[] playerSlots; // Should have length 2

        [Header("Player Icons and Names")]
        public Image[] playerIcons; // Should have length 2
        public TMP_Text[] playerNames; // Should have length 2

        [Header("Buttons")]
        public Button startMatchButton;
        public Button leaveRoomButton;

        [Header("UI Texts")]
        public TMP_Text passcodeText;

        [Header("Icons")]
        public Sprite masterIcon;
        public Sprite readyIcon;
        public Sprite waitingIcon;

        // Spawn points
        public Transform[] spawnPoints;

        // Ready Status Tracking
        private Dictionary<int, bool> playerReadyStatus = new Dictionary<int, bool>();

        private PhotonView _photonView;
        

        // Singleton pattern for easy access
        public static SoloLobbyManager Instance;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Attach listeners
            if (startMatchButton != null)
                startMatchButton.onClick.AddListener(OnStartMatchButtonClicked);
            else
                Debug.LogError("Start Match Button is not assigned in the Inspector.");

            if (leaveRoomButton != null)
                leaveRoomButton.onClick.AddListener(OnLeaveRoomButtonClicked);
            else
                Debug.LogError("Leave Room Button is not assigned in the Inspector.");

            InitializeStartMatchButton();

            if (PhotonNetwork.CurrentRoom != null)
            {
                passcodeText.text = $"Passcode: {PhotonNetwork.CurrentRoom.Name}";
            }
            else
            {
                passcodeText.text = "Not in a room";
            }

            UpdatePlayerSlots();

            // Register to the scene loaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
            
        }

        private void OnDestroy()
        {
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
                startMatchButton.interactable = PhotonNetwork.CurrentRoom.PlayerCount == 2;
                UpdateStartMatchButtonInteractable();
            }
            else
            {
                // Check if the player is already ready
                bool isReady = playerReadyStatus.ContainsKey(PhotonNetwork.LocalPlayer.ActorNumber) && playerReadyStatus[PhotonNetwork.LocalPlayer.ActorNumber];
                startMatchButton.GetComponentInChildren<TMP_Text>().text = isReady ? "Unready" : "Ready";
                startMatchButton.interactable = true;
            }
        }

        void OnStartMatchButtonClicked()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // With 2 players, ensure the non-master is ready
                if (AreAllNonMasterPlayersReady())
                {
                    _photonView.RPC("StartMatch", RpcTarget.All);
                }
                else
                {
                    Debug.LogWarning("The other player is not ready.");
                }
            }
            else
            {
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

            // Update the button text if this RPC is for the local player
            if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber && !PhotonNetwork.IsMasterClient)
            {
                startMatchButton.GetComponentInChildren<TMP_Text>().text = isReady ? "Unready" : "Ready";
            }
        }

        bool AreAllNonMasterPlayersReady()
        {
            // In a 2-player scenario, there's only one non-master player
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (!player.IsMasterClient)
                {
                    if (!playerReadyStatus.ContainsKey(player.ActorNumber) || !playerReadyStatus[player.ActorNumber])
                        return false;
                }
            }
            return PhotonNetwork.CurrentRoom.PlayerCount == 2;
        }

        void UpdateStartMatchButtonInteractable()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                startMatchButton.interactable = AreAllNonMasterPlayersReady();
            }
            else
            {
                startMatchButton.interactable = true;
            }
        }

        void UpdatePlayerSlots()
        {
            if (playerSlots == null || playerSlots.Length < 2)
            {
                Debug.LogError("Player slots are not assigned correctly.");
                return;
            }

            if (playerIcons == null || playerIcons.Length < 2)
            {
                Debug.LogError("Player icons are not assigned correctly.");
                return;
            }

            if (playerNames == null || playerNames.Length < 2)
            {
                Debug.LogError("Player names are not assigned correctly.");
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
                AssignSpawnIndicesToAllPlayers();
            }
            PhotonNetwork.LoadLevel("SoloScene"); // Ensure SoloScene is added to Build Settings
        }

        void AssignSpawnIndicesToAllPlayers()
        {
            Player[] players = PhotonNetwork.PlayerList;
            for (int i = 0; i < players.Length; i++)
            {
                int spawnIndex = i % spawnPoints.Length;
                ExitGames.Client.Photon.Hashtable spawnProps = new ExitGames.Client.Photon.Hashtable();
                spawnProps["SpawnIndex"] = spawnIndex;
                players[i].SetCustomProperties(spawnProps);
                Debug.Log($"Assigning spawn index {spawnIndex} to player {players[i].NickName} with ActorNumber {players[i].ActorNumber}");
            }
        }


        private PhotonView GetPhotonViewFromPlayer(Player player)
        {
            PhotonView[] allPhotonViews = FindObjectsOfType<PhotonView>();
            foreach (PhotonView pv in allPhotonViews)
            {
                if (pv.Owner == player)
                {
                    return pv;
                }
            }
            return null;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "SoloScene")
            {
                // Additional logic if needed
            }
        }

        void OnLeaveRoomButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}
