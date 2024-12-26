using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Text;
using Photon.Realtime;
using ServerInteraction.Payload;
using UnityEngine.Networking;


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
            
            SendResult();

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

        void SendResult()
        {
            double endingTime = PhotonNetwork.Time;
            double startingTime = (double)PhotonNetwork.CurrentRoom.CustomProperties["startTime"];
            double timeElapsed = endingTime - startingTime;

            string dateStartTime = (string)PhotonNetwork.CurrentRoom.CustomProperties["dateStartTime"];

            // Get all names of players
            object[] playerNamesArray = (object[])PhotonNetwork.CurrentRoom.CustomProperties["playerNames"];
            List<string> playerNames = new List<string>();
            foreach (var name in playerNamesArray)
            {
                playerNames.Add(name.ToString());
            }

            // Create the MultiHistoryMatch object
            MultiHistoryMatch match = new MultiHistoryMatch
            {
                dateStartTime = dateStartTime,
                timeElapsed = timeElapsed,
                playerNames = playerNames,
                deathCount = Random.Range(1, 20)
            };

            // Debug logs to print all results first
            Debug.Log("dateStartTime: " + match.dateStartTime);
            Debug.Log("timeElapsed: " + match.timeElapsed);
            Debug.Log("playerNames: " + string.Join(", ", match.playerNames));
            Debug.Log("deathCount: " + match.deathCount);

            // Serialize the object to JSON
            string jsonString = JsonUtility.ToJson(match);
            Debug.Log("Serialized JSON: " + jsonString); // Add this to verify the JSON

            // Create a UnityWebRequest with POST method
            UnityWebRequest request = new UnityWebRequest("http://localhost:8080/api/gameplay/update-multi-match", "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Start the coroutine to send the request
            StartCoroutine(SendRequestCoroutine(request));
        }

        IEnumerator SendRequestCoroutine(UnityWebRequest request)
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }
}