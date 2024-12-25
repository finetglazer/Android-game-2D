using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.SceneManager.Solo
{
    public class SoloSceneManager : MonoBehaviourPunCallbacks
    {
        [Header("Player Settings")]
        public GameObject playerPrefab;
        public Transform[] spawnPoints;

        [Header("UI Elements")]
        public Button leaveButton;

        private bool hasSpawned = false;
        private bool isLeaving = false;

        private void Awake()
        {
            if (FindObjectsOfType<SoloSceneManager>().Length > 1)
            {
                Destroy(this.gameObject);
            }
            else
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }

        private void Start()
        {
            // Attempt to spawn the player immediately if SpawnIndex is already available.
            if (!hasSpawned && PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("SpawnIndex", out object indexObj))
            {
                int spawnIndex = (int)indexObj;
                SpawnLocalPlayer(spawnIndex);
                hasSpawned = true;
            }

            // Set up leave button
            if (leaveButton != null)
            {
                leaveButton.onClick.RemoveAllListeners();
                leaveButton.onClick.AddListener(LeaveRoom);
            }
            else
            {
                Debug.LogError("Leave Button is not assigned in the Inspector.");
            }

            // Debug check for non-master client
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Non-master client detected. Waiting for SpawnIndex to be set if not already.");
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

            // If the local player's SpawnIndex is set later, spawn now.
            if (targetPlayer == PhotonNetwork.LocalPlayer && changedProps.ContainsKey("SpawnIndex") && !hasSpawned)
            {
                int spawnIndex = (int)changedProps["SpawnIndex"];
                SpawnLocalPlayer(spawnIndex);
                hasSpawned = true;
            }
        }

        private void SpawnLocalPlayer(int spawnIndex)
        {
            Debug.Log("The call");
            if (playerPrefab == null)
            {
                Debug.LogError("Player Prefab is not assigned in SoloSceneManager.");
                return;
            }

            Vector3 spawnPosition;
            Quaternion spawnRotation;

            if (spawnIndex >= 0 && spawnPoints != null && spawnIndex < spawnPoints.Length)
            {
                spawnPosition = spawnPoints[spawnIndex].position;
                spawnRotation = spawnPoints[spawnIndex].rotation;
            }
            else
            {
                spawnPosition = new Vector3(Random.Range(-5f, 5f), 0f, 0f);
                spawnRotation = Quaternion.identity;
                Debug.LogWarning("No valid spawn point found. Spawning at a random position.");
            }

            Debug.Log($"Spawning player {PhotonNetwork.LocalPlayer.NickName} at {spawnPosition} with SpawnIndex {spawnIndex}");
            GameObject instantiatedPlayer = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, spawnRotation);
            if (instantiatedPlayer == null)
            {
                Debug.LogError("Failed to instantiate player prefab in SoloScene.");
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
    }
}
