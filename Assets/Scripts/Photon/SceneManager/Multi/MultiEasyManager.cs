using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.SceneManager.Multi
{
    public class MultiEasyManager : MonoBehaviourPunCallbacks
    {
        [Header("Player Settings")]
        public Transform spawnPoint;

        [Header("UI Elements")]
        public Button leaveButton;

        private bool hasSpawned;
        private bool isLeaving;

        private void Start()
        {
            if (!hasSpawned)
            {
                hasSpawned = true;
                SpawnPlayers();
            }
            
            // Set up leave button
            
            leaveButton.onClick.RemoveAllListeners();
            leaveButton.onClick.AddListener(LeaveRoom);
        }
        
        
        private void SpawnPlayers()
        {
            Vector3 spawnPosition = spawnPoint.position;
            Quaternion spawnRotation = spawnPoint.rotation;
            
            Debug.Log(spawnPosition.x + " " + spawnPosition.y + " " + spawnPosition.z);
            
            // get the name of the player option from the dropdown
            string playerPrefab = PhotonNetwork.LocalPlayer.CustomProperties["Character"] as string;
            

            Debug.Log($"Spawning player {PhotonNetwork.LocalPlayer.NickName} at {spawnPosition} with SpawnIndex");
            GameObject instantiatedPlayer = PhotonNetwork.Instantiate(playerPrefab, spawnPosition, spawnRotation);
            
            // 
            Hashtable playerProperties = new Hashtable();
            playerProperties["position"] = transform.position; // Example: Assign current position

            // Save the hashtable to the local player
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
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