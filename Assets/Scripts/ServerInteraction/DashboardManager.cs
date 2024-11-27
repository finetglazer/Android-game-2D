using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        private string passcode = "";
        public Button joinButton;
        
        public Button newGameButton;
        public Button continueGameButton;
        public Button leaderboardButton;
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
            leaderboardButton.onClick.AddListener(OnLeaderBoardButtonClicked);
            passwordChangeButton.onClick.AddListener(OnPasswordChangeButtonClicked);
            signOutButton.onClick.AddListener(OnSignOutButtonClicked);
            

            // Ensure Photon is connected before displaying options
            // Listen for the confirm button click
            confirmButton.onClick.AddListener(OnConfirmSelection);

            // Set the default Inactive with the passcode input field
            passcodeInputField.gameObject.SetActive(false);
            joinButton.gameObject.SetActive(false);
            
            // Connect to Photon if not already connected
            if (PhotonNetwork.IsConnectedAndReady)
            {
                informText.fontStyle = FontStyles.Normal;
                informText.text = "Already connected, ready to create or join rooms.";
                informText.color = Color.green;
                // Debug.Log("Already connected, ready to create or join rooms.");
            }
            else
            {
                informText.text = "Connecting to Photon...";
                informText.color = Color.yellow;
                informText.fontStyle = FontStyles.Italic;
                // Debug.Log("Not connected, attempting to connect...");
                PhotonNetwork.ConnectUsingSettings();  // Connect to Photon server
            }
        }
        
        // This callback is triggered once connected to the Photon Master Server
        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Photon Master Server.");
            
            // Now that we're connected, we can try to create or join rooms
            if (PhotonNetwork.IsConnectedAndReady)
            {
                informText.fontStyle = FontStyles.Normal;
                informText.text = "Photon is ready, you can now create/join rooms.";
                informText.color = Color.green;
                // Debug.Log("Photon is ready, you can now create/join rooms.");
            }
            else
            {
                informText.text = "Photon is not ready yet.";
                informText.color = Color.yellow;
                // Debug.LogWarning("Photon is not ready yet.");
            }
        }
        
        
        // Callback for when the room list is updated (if using custom room lists)
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            availableRooms = roomList.Where(room =>
                room.CustomProperties.ContainsKey("passcode") &&
                room.CustomProperties["passcode"].ToString() == passcodeInputField.text).ToList();
            Debug.Log("Room list updated");
        }
        
        void OnConfirmSelection()
        {
            // First check if we are connected to Photon and ready
            if (PhotonNetwork.IsConnectedAndReady)
            {
                int selectedOption = roomOptionsDropdown.value;

                if (selectedOption == 0)
                {
                    CreateRoom();  // Create room if option is selected
                    passcodeInputField.gameObject.SetActive(false);  // Show the passcode input field
                    joinButton.gameObject.SetActive(false);  // Show the join button
                }
                else if (selectedOption == 1)
                {
                    passcodeInputField.gameObject.SetActive(true);  // Show the passcode input field
                    joinButton.gameObject.SetActive(true);  // Show the join button
                    
                    joinButton.onClick.AddListener(JoinRoom);  // Join room if option is selected
                    // JoinRoom();  // Join room if option is selected
                }
            }
            else
            {
                // Optionally, show a loading screen or message telling the user they need to wait for the connection
                Debug.LogWarning("Photon is not connected yet. Please wait...");
            }
        }

        
        // Function to create a room with a passcode
        void CreateRoom()
        {
            // Generate a random passcode
            passcode = new Random().Next(1000, 9999).ToString();

            // Room options (you can customize these)
            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = 4,  // Max players in the room
                IsVisible = true, // Room is visible to others
                IsOpen = true     // Room is open for players to join
            };

            // Store the passcode in the room properties
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "passcode", passcode }  // Store passcode in room properties
            };

            // Create the room with a random name
            string roomName = "Room_" + new Random().Next(1, 1000);  // You can use a more descriptive room name
            PhotonNetwork.CreateRoom(roomName, roomOptions);  // Create the room

            Debug.Log("Creating room with passcode: " + passcode);
            PlayerPrefs.SetString("roomPasscode", passcode);
            PlayerPrefs.Save();
        }
        
        // Callback for when room creation succeeds
        public override void OnCreatedRoom()
        {
            Debug.Log("Room created successfully.");

            // After room creation, load the LobbyScene
            PhotonNetwork.LoadLevel("LobbyScene");
        }
        
        // Callback for when room creation fails
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError("Room creation failed: " + message);
        }

        
        void JoinRoom()
        {
            
            string enteredPasscode = passcodeInputField.text;  // Get the passcode from the input field

            if (string.IsNullOrEmpty(enteredPasscode))
            {
                Debug.LogError("Please enter a passcode.");
                return;
            }

            // Search for a room that has a matching passcode
            foreach (var room in availableRooms)
            {
                if (room.CustomProperties.ContainsKey("passcode") &&
                    room.CustomProperties["passcode"].ToString() == enteredPasscode)
                {
                    PhotonNetwork.JoinRoom(room.Name);  // Join the room with the matching passcode
                    Debug.Log("Joining room: " + room.Name);
                    return;
                }
            }

            // If no room matches, show an error
            Debug.LogError("No room found with that passcode.");
        }

        // Callback for when the player successfully joins a room
        public override void OnJoinedRoom()
        {
            Debug.Log("Successfully joined the room.");
            // Load the lobby scene (replace with your actual scene name)
            PhotonNetwork.LoadLevel("LobbyScene");
        }

       

        // Callback for when joining a room fails
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

        private void OnLeaderBoardButtonClicked()
        {
            StartCoroutine(CreatePlayerRankingRequest());
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