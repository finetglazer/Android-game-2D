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
            
            confirmButton.onClick.AddListener(OnConfirmSelection);

            // Set the default Inactive with the passcode input field
            passcodeInputField.gameObject.SetActive(false);
            joinButton.gameObject.SetActive(false);
            
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
            // First, clear the list of available rooms
            availableRooms.Clear();
    
            // Filter rooms based on the passcode entered by the user
            string enteredPasscode = passcodeInputField.text.Trim();  // Trim the input passcode to remove any extra spaces
            if (string.IsNullOrEmpty(enteredPasscode))
            {
                Debug.LogError("Please enter a passcode to join a room.");
                return;
            }

            // Debugging the passcode you're trying to match
            Debug.Log("Entered Passcode: " + enteredPasscode);

            foreach (var room in roomList)
            {
                // Make sure the room contains the passcode key in custom properties
                if (room.CustomProperties.ContainsKey("passcode"))
                {
                    string roomPasscode = room.CustomProperties["passcode"]?.ToString(); // Get the passcode from the room properties
            
                    // Debugging log for each room's passcode
                    Debug.Log($"Room: {room.Name} | Room Passcode: {roomPasscode}");
            
                    // If the passcode matches the entered passcode, add the room to the availableRooms list
                    if (roomPasscode != null && roomPasscode == enteredPasscode)
                    {
                        availableRooms.Add(room);
                        Debug.Log("Room added to available rooms: " + room.Name);
                    }
                }
                else
                {
                    Debug.Log($"Room {room.Name} does not have a passcode.");
                }
            }

            // Log available rooms after filtering
            Debug.Log("Available Rooms: " + availableRooms.Count);
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
            // Generate a random passcode for the new room
            passcode = new Random().Next(1000, 9999).ToString();

            // Room options (customize these as needed)
            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = 4,    // Max players in the room
                IsVisible = true,   // Room is visible to others
                IsOpen = true       // Room is open for players to join
            };

            // Store the passcode in the room's custom properties
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "passcode", passcode }
            };

            // Create a room with a random name
            string roomName = "Room_" + new Random().Next(1, 1000); // Random room name
            PhotonNetwork.CreateRoom(roomName, roomOptions); // Create the room

            // Debugging log to see the passcode
            Debug.Log("Room created with passcode: " + passcode);
        }



        IEnumerator WaitForRoomListUpdate()
        {
            // Wait for a short period to allow the room list to update
            yield return new WaitForSeconds(1f);  // You can adjust this delay time

            // After the delay, try joining the room again
            Debug.Log("Attempting to join room with passcode: " + passcodeInputField.text);
            JoinRoom(); // Now attempt to join the room
        }



        public override void OnCreatedRoom()
        {
            Debug.Log("Room created successfully with passcode: " + passcode);
            PhotonNetwork.LoadLevel("LobbyScene");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError("Room creation failed: " + message);
        }
        void JoinRoom()
        {
            string enteredPasscode = passcodeInputField.text.Trim(); // Ensure we are using the trimmed input

            // Check if the entered passcode is empty
            if (string.IsNullOrEmpty(enteredPasscode))
            {
                Debug.LogError("Please enter a passcode to join a room.");
                return;
            }

            // Debugging the entered passcode
            Debug.Log("Entered Passcode: " + enteredPasscode);

            // Loop through the available rooms to find one with the correct passcode
            foreach (var room in availableRooms)
            {
                string roomPasscode = room.CustomProperties["passcode"]?.ToString(); // Ensure the passcode is fetched properly
        
                // Debugging log for each room's passcode and whether it's a match
                if (roomPasscode != null)
                {
                    Debug.Log($"Room: {room.Name} | Passcode: {roomPasscode}");

                    // If we find a matching passcode, attempt to join the room
                    if (roomPasscode == enteredPasscode)
                    {
                        PhotonNetwork.JoinRoom(room.Name); // Join the room
                        Debug.Log("Joining room: " + room.Name);
                        return;
                    }
                }
                else
                {
                    Debug.Log("Room " + room.Name + " does not have a passcode.");
                }
            }

            // If no room is found with the passcode, log an error
            Debug.LogError("No room found with the entered passcode: " + enteredPasscode);
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