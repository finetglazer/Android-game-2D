using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Photon
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        // UI elements
        public GameObject[] playerSlots;
        public Button leaveRoomButton;
        public Button startMatchButton;
        public TMP_Text passcodeText;

        public GameObject playerPrefab;

        private int currentPlayerCount;
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
            // Attach listeners to the buttons
            if (leaveRoomButton != null)
                leaveRoomButton.onClick.AddListener(OnLeaveRoomButtonClicked);
            else
                Debug.LogError("Leave Room Button is not assigned in the Inspector.");
            
            if (startMatchButton != null)
                startMatchButton.onClick.AddListener(OnStartMatchButtonClicked);
            else
                Debug.LogError("Start Match Button is not assigned in the Inspector.");
            
            // Disable the start match button initially
            startMatchButton.interactable = false;

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
            if (playerSlots == null || playerSlots.Length == 0)
            {
                Debug.LogError("Player slots are not assigned in the Inspector.");
                return;
            }

            currentPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            Debug.Log("Current Player Count: " + currentPlayerCount);
            int playerIndex = 0;

            foreach (var slot in playerSlots)
            {
                slot.SetActive(playerIndex < currentPlayerCount);
                // Update the player name in the slot
                if (playerIndex < PhotonNetwork.PlayerList.Length)
                {
                    slot.GetComponentInChildren<TMP_Text>().text = PhotonNetwork.PlayerList[playerIndex].NickName;
                }
                else
                {
                    slot.GetComponentInChildren<TMP_Text>().text = "Empty";
                }
                playerIndex++;
            }
        }

        void OnLeaveRoomButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
            // Load the previous scene (Dashboard, for example)
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/DashboardScene");
        }

        // This method will be called when the master client presses the start match button
        void OnStartMatchButtonClicked()
        {
            if (currentPlayerCount >= 2)
            {
                // Check if this is the master client before initiating the match
                if (PhotonNetwork.IsMasterClient)
                {
                    // Send RPC to all clients to start the match
                    if (_photonView != null) // Ensure photonView is not null
                    {
                        _photonView.RPC("StartMatch", RpcTarget.All);
                    }
                    else
                    {
                        Debug.LogError("PhotonView is not assigned on the GameObject.");
                    }
                }
                else
                {
                    Debug.LogWarning("Only the master client can start the match.");
                }
            }
            else
            {
                Debug.LogError("Not enough players to start the match.");
            }
        }

        // RPC method that is called on all clients to start the match
        [PunRPC]
        void StartMatch()
        {
            // All clients will execute this method
            Debug.Log("StartMatch RPC called");

            // Load the scene for all players
            PhotonNetwork.LoadLevel("EndingScene"); // Match scene loading
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

        public override void OnJoinedRoom()
        {
            // We don't want to instantiate the player here because the scene is not loaded yet.
        }
    }
}
