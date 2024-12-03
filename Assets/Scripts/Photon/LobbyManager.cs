using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

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

        [Header("Player Prefab")]
        public GameObject playerPrefab;

        [Header("Icons")]
        public Sprite masterIcon;
        public Sprite readyIcon;
        public Sprite waitingIcon;

        // Ready Status Tracking
        private Dictionary<int, bool> playerReadyStatus = new Dictionary<int, bool>();

        private PhotonView _photonView;

        private void Awake()
        {
            // Ensure the PhotonView is correctly assigned.
            _photonView = GetComponent<PhotonView>();
            if (_photonView == null)
            {
                Debug.LogError("PhotonView is not attached to this GameObject.");
            }
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

            passcodeText.text = $"Passcode: {PhotonNetwork.CurrentRoom.Name}"; // room name

            // Update the player slots
            UpdatePlayerSlots();

            // Register to the scene loaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
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
            // Load the previous scene (Dashboard, for example) after leaving the room
            SceneManager.LoadScene("Scenes/DashboardScene");
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
                startMatchButton.GetComponentInChildren<TMP_Text>().text = "Ready";
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
                // Non-master clients' button is always interactable for readying
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

        // RPC method that is called on all clients to start the match
        [PunRPC]
        void StartMatch()
        {
            // All clients will execute this method
            Debug.Log("StartMatch RPC called");

            // Load the match scene for all players
            PhotonNetwork.LoadLevel("EndingScene"); // Replace with your actual match scene name
        }

        // This method is called when the scene has finished loading on all clients
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "SoloScene")
            {
                // Define the spawn position (can be random or fixed depending on your needs)
                Vector3 spawnPosition = new Vector3(Random.Range(-5f, 5f), 0f, 0f);  // Example spawn position

                // Instantiate the player object for each player in the room
                PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
            }
        }

        // New method to handle Leave Room button click
        void OnLeaveRoomButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
            // Scene loading is handled in the OnLeftRoom callback
        }
    }
}
