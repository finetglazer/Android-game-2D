using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Photon
{
    public class SoloSceneManager : MonoBehaviourPunCallbacks
    {
        [Header("Player Settings")]
        [Tooltip("Player prefab must be placed inside a Resources folder.")]
        public GameObject playerPrefab;

        [Tooltip("Assign spawn points in the Inspector.")]
        public Transform[] spawnPoints;

        [Header("UI Elements")]
        [Tooltip("Button to leave the room.")]
        public Button leaveButton;

        // Flag to prevent multiple LeaveRoom calls
        private bool isLeaving = false;

        private void Awake()
        {
            // Ensure PhotonView is attached to this GameObject
            if (photonView == null)
            {
                Debug.LogError("PhotonView is not attached to SoloSceneManager.");
            }

            // Implement Singleton pattern to ensure only one instance exists
            if (FindObjectsOfType<SoloSceneManager>().Length > 1)
            {
                Debug.LogWarning("Multiple SoloSceneManager instances detected. Destroying duplicate.");
                Destroy(this.gameObject);
            }
            else
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }

        private void Start()
        {
            // Spawn the local player
            SpawnLocalPlayer();

            // Setup Leave Room button
            if (leaveButton != null)
            {
                leaveButton.onClick.RemoveAllListeners(); // Prevent multiple listeners
                leaveButton.onClick.AddListener(LeaveRoom);
            }
            else
            {
                Debug.LogError("Leave Button is not assigned in the Inspector.");
            }
        }

        /// <summary>
        /// Spawns the local player at the assigned spawn point.
        /// </summary>
        void SpawnLocalPlayer()
        {
            if (playerPrefab == null)
            {
                Debug.LogError("Player Prefab is not assigned in SoloSceneManager.");
                return;
            }

            Vector3 spawnPosition = Vector3.zero;
            Quaternion spawnRotation = Quaternion.identity;
            int spawnIndex = 0;

            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnIndex = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPoints.Length;
                spawnPosition = spawnPoints[spawnIndex].position;
                spawnRotation = spawnPoints[spawnIndex].rotation;
            }
            else
            {
                // Default spawn position if no spawn points are assigned
                spawnPosition = new Vector3(Random.Range(-5f, 5f), 0f, 0f);
                Debug.LogWarning("No spawn points assigned. Spawning at a random position.");
            }

            Debug.Log($"Spawning player {PhotonNetwork.LocalPlayer.NickName} at index {spawnIndex} position {spawnPosition}");

            // Instantiate the player prefab across the network
            GameObject instantiatedPlayer = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, spawnRotation);
            if (instantiatedPlayer != null)
            {
                Debug.Log($"Player instantiated for ActorNumber: {PhotonNetwork.LocalPlayer.ActorNumber} at {spawnPosition}");
            }
            else
            {
                Debug.LogError("Failed to instantiate player prefab in SoloScene.");
            }
        }

        /// <summary>
        /// Handles the Leave Room button click.
        /// </summary>
        private void LeaveRoom()
        {
            if (isLeaving)
            {
                Debug.LogWarning("Already leaving the room. Please wait.");
                return;
            }

            if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
            {
                isLeaving = true;
                if (leaveButton != null)
                {
                    leaveButton.interactable = false; // Disable the button to prevent multiple clicks
                }
                Debug.Log("Leaving the room...");
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                Debug.LogWarning("Cannot leave room: Client is not connected or not in a room.");
            }
        }

        /// <summary>
        /// Callback when the client has successfully left the room.
        /// </summary>
        public override void OnLeftRoom()
        {
            Debug.Log("Successfully left the room. Joining lobby and loading DashboardScene.");
            base.OnLeftRoom();
            PhotonNetwork.JoinLobby();

            // Load the DashboardScene, ensure it's added to Build Settings
            PhotonNetwork.LoadLevel("Scenes/DashboardScene");
        }

        /// <summary>
        /// Callback when the client has joined the lobby.
        /// </summary>
        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            Debug.Log("Joined the lobby.");
        }

        /// <summary>
        /// Callback when another player joins the room.
        /// </summary>
        /// <param name="newPlayer">The player who joined.</param>
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            Debug.Log($"Player {newPlayer.NickName} entered the room.");
        }

        /// <summary>
        /// Callback when a player leaves the room.
        /// </summary>
        /// <param name="otherPlayer">The player who left.</param>
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            Debug.Log($"Player {otherPlayer.NickName} left the room.");
        }
    }
}
