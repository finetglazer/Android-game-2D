using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit.Forms;
using Photon.Realtime;

using ServerInteraction.Responses;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

namespace ServerInteraction
{
    public class DashboardManager : MonoBehaviourPunCallbacks
    {
        public TMP_Text informText;
        public TMP_Dropdown roomOptionsDropdown;
        public Button confirmButton;
        public TMP_InputField passcodeInputField;
        public Button joinButton;
        
        public Button newGameButton;
        public Button continueGameButton;
        public Button leaderboardButton;
        public Button matchHistoryButton;
        private const string RootRequestURL = "http://localhost:8080/api/gameplay";
        private Vector3 _playerPosition;
        public Button passwordChangeButton;
        public Button signOutButton;
        private bool _isNewGame = true;
        internal static PlayerRankingResponse PlayerRankingResponse = new();
        
        
        // The list of rooms in the lobby
        private List<RoomInfo> availableRooms = new List<RoomInfo>();

        private void Start()
        {
            newGameButton.onClick.AddListener(OnNewGameButtonClicked);
            continueGameButton.onClick.AddListener(OnGameContinueButtonClicked);
            leaderboardButton.onClick.AddListener(OnLeaderboardButtonClicked);
            matchHistoryButton.onClick.AddListener(OnMatchHistoryButtonClicked);
            passwordChangeButton.onClick.AddListener(OnPasswordChangeButtonClicked);
            signOutButton.onClick.AddListener(OnSignOutButtonClicked);
            
            confirmButton.onClick.AddListener(OnConfirmSelection);

            // Set the default Inactive with the passcode input field
            passcodeInputField.gameObject.SetActive(false);
            joinButton.gameObject.SetActive(false);
            
            PhotonNetwork.NickName = PlayerPrefs.GetString("alias", "Player");
            
            if (PhotonNetwork.IsConnectedAndReady)
            {
                informText.fontStyle = FontStyles.Normal;
                informText.text = "Already connected, ready to create or join rooms.";
                informText.color = Color.green;
            }
            else
            {
                informText.text = "Connecting to Photon...";
                informText.color = Color.yellow;
                informText.fontStyle = FontStyles.Italic;
                PhotonNetwork.ConnectUsingSettings();
                // PhotonNetwork.AutomaticallySyncScene = true;

            }
            
        }
        
        // This callback is triggered once connected to the Photon Master Server
        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Photon Master Server.");
            if (PhotonNetwork.IsConnectedAndReady)
            {
                informText.fontStyle = FontStyles.Normal;
                informText.text = "Photon is ready, you can now create/join rooms.";
                informText.color = Color.green;

                // Request the room list after successfully connecting to Photon
                PhotonNetwork.JoinLobby();  // This joins the default lobby, which should update the room list.
            }
            else
            {
                informText.text = "Photon is not ready yet.";
                informText.color = Color.yellow;
            }
        }
        
        // This callback is triggered once room list is updated
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (var room in roomList)
            {
                availableRooms.Add(room);
                Debug.Log("Room available: " + room.Name);
            }

            // Log the available rooms count after filtering
            Debug.Log("Filtered Available Rooms: " + availableRooms.Count);
        }

        void OnConfirmSelection()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                int selectedOption = roomOptionsDropdown.value;

                // If the user wants to create a new room
                if (selectedOption == 0)
                {
                    CreateRoom();
                    passcodeInputField.gameObject.SetActive(false); // Hide the passcode input field
                    joinButton.gameObject.SetActive(false); // Hide the join button
                }
                else if (selectedOption == 1) // If the user wants to join an existing room
                {
                    passcodeInputField.gameObject.SetActive(true); // Show passcode input field
                    joinButton.gameObject.SetActive(true); // Show join button
                    joinButton.onClick.AddListener(JoinRoom); // Attach the JoinRoom function to the join button
                }
            }
            else
            {
                Debug.LogWarning("Photon is not connected yet. Please wait...");
            }
        }
        
        void CreateRoom()
        {
            // Room options (customize these as needed)
            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = 4,    // Max players in the room
                IsVisible = true,   // Room is visible to others
                IsOpen = true       // Room is open for players to join
            };

            // Create a room with a random name
            string roomName = new Random().Next(1000, 9999).ToString(); // Random room name
            PhotonNetwork.CreateRoom(roomName, roomOptions); // Create the room
            PlayerPrefs.SetString("roomPasscode", roomName); // Save the room passcode in PlayerPrefs
        }
        
        public override void OnCreatedRoom()
        {
            // Accessing the current room properties
            Room room = PhotonNetwork.CurrentRoom;

            // Accessing basic room information
            Debug.Log("Room Name: " + room.Name);
            Debug.Log("Max Players: " + room.MaxPlayers);
            Debug.Log("Player Count: " + room.PlayerCount);
            
            PhotonNetwork.LoadLevel("LobbyScene");
            
            
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError("Room creation failed: " + message);
        }
        
        void JoinRoom()
        {
            string roomName = passcodeInputField.text;
            Debug.Log("Joining room: " + roomName);
            PhotonNetwork.JoinRoom(roomName);
            
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Successfully joined the room.");
            PhotonNetwork.LoadLevel("LobbyScene");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogError("Join room failed: " + message);
        }

        private void OnSignOutButtonClicked()
        {
            SceneManager.LoadScene("Scenes/SignOutScene");
        }
        
        private void OnPasswordChangeButtonClicked()
        {
            SceneManager.LoadScene("PasswordChangeScene");
        }
        
        private void OnGameContinueButtonClicked()
        {
            StartCoroutine(CreateContinueGameRequest());
        }

        private void OnNewGameButtonClicked()
        {
            StartCoroutine(CreateNewGameRequest());
        }

        private async void OnLeaderboardButtonClicked()
        {
            try
            {
                await CreateGetAllSoloStatsRequest();
            }
            catch (Exception e)
            {
                print(e.ToString());
            }
        }

        private async void OnMatchHistoryButtonClicked()
        {
            try
            {
               await CreateGetMultiplayerMatchHistoryRequest();
            }
            catch (Exception e)
            {
                print(e.ToString());
            }
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator CreateContinueGameRequest()
        {
            var request = RequestGenerator(RootRequestURL + "/continue", new[] { "userId" }, new[] { PlayerPrefs.GetString("userId") }, "POST");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                var gameContinueResponse = JsonUtility.FromJson<GameContinueResponse>(request.downloadHandler.text);
                var currentPositionString = gameContinueResponse.currentPosition;
                
                if (gameContinueResponse.currentPosition != "")
                {
                    var currentPositionNums = currentPositionString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => float.Parse(s.Trim(), CultureInfo.InvariantCulture)).ToArray();
                    _playerPosition = new Vector3(currentPositionNums[0], currentPositionNums[1], currentPositionNums[2]);
                    _isNewGame = false;
                }
                
                SceneManager.sceneLoaded += OnSceneLoaded;
                LoadSceneWithLoadingScreen(gameContinueResponse.currentPosition != "" ? SceneNamesAndURLs.SceneUrLs[gameContinueResponse.sceneIndex - 1] : SceneNamesAndURLs.SceneUrLs[0]);
                // SceneManager.LoadScene(gameContinueResponse.currentPosition != "" ? SceneNamesAndURLs.SceneUrLs[gameContinueResponse.sceneIndex - 1] : SceneNamesAndURLs.SceneUrLs[0]);
            }
            else
            {
                print(request.downloadHandler.text);
            }
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe after the scene is loaded to prevent multiple triggers.
            var player = GameObject.FindWithTag("Player");
            if (player is null) return;
            if (!_isNewGame)
            {
                player.transform.position = _playerPosition;
            }
            
            print("Player position set after scene load.");
        }

        private IEnumerator CreateNewGameRequest()
        {
            var request = RequestGenerator(RootRequestURL + "/new-game", new[] { "userId" }, new []{ PlayerPrefs.GetString("userId") }, "POST");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                LoadSceneWithLoadingScreen("Scenes/1stscene");
                // SceneManager.LoadScene("Scenes/1stscene");
            }
            else
            {
                print(request.downloadHandler.text);
            }
        }
        
        private IEnumerator CreatePlayerRankingRequest()
        {
            var request = RequestGenerator(RootRequestURL + "/rank", new string[] {}, new string[] {}, "GET");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                PlayerRankingResponse = JsonUtility.FromJson<PlayerRankingResponse>(request.downloadHandler.text);
                LoadSceneWithLoadingScreen("Scenes/LeaderboardScene");   
                // SceneManager.LoadScene("Scenes/LeaderboardScene");
            }
            else
            {
                print(request.downloadHandler.text);
            }
        }
        
        private async Task CreateGetAllSoloStatsRequest()
        {
            try
            {
                var client = new HttpClient();
                var response = await client.GetStringAsync(RootRequestURL + "/get-all-solo-stats");
                var statsResponseList = JsonConvert.DeserializeObject<GetAllSoloStatsResponse>(response).soloStatsList;
                LeaderboardUIScript.SoloStatsList = statsResponseList;
                SceneManager.LoadScene("TestScene - Hiep/LeaderboardScene");
            }
            catch (Exception e)
            {
                print(e.ToString());
            }
        }

        private async Task CreateGetMultiplayerMatchHistoryRequest()
        {
            try
            {
                var client = new HttpClient();
                var response = await client.GetStringAsync(RootRequestURL + "/get-multi-player-match-history");
                var multiPlayerMatchHistoryList = JsonConvert.DeserializeObject<GetMultiPlayerMatchHistoryResponse>(response).multiPlayerMatchHistoryList;
                SceneManager.LoadScene("TestScene - Hiep/LeaderboardScene");
            }
            catch (Exception e)
            {
                print(e.ToString());
            }

        }

        private static UnityWebRequest RequestGenerator(string url, string[] fieldNames, string[] values, string method)
        {
            var request = new UnityWebRequest(url, method);
            var jsonBody = "{";
            for (var i = 0; i < fieldNames.Length - 1; ++i)
            {
                jsonBody += "\"" + fieldNames[i] + "\":\"" + values[i] + "\",";
            }
            jsonBody += "\"" + (fieldNames.Length != 0 ? fieldNames[^1] : "") + "\":\"" + (values.Length != 0 ? values[^1] : "") + "\"}";
            var jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            return request;
        }
        
        public void LoadSceneWithLoadingScreen(string sceneToLoad)
        {
            // Set the next scene name in the SceneLoader static class
            SceneLoader.nextSceneName = sceneToLoad;

            // Load the loading scene
            SceneManager.LoadScene("Scenes/FastLoadingScene");
        }
    }
}