using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Photon;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit.Forms;
using Photon.Realtime;

using ServerInteraction.Responses;
using SingleLeaderboard;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
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
        // public Button soloLeaderboardButton;
        // public Button singleLeaderboardButton;
        // public Button matchHistoryButton;
        private const string RootRequestURL = "http://localhost:8080/api/gameplay";
        private Vector3 _playerPosition;
        public Button passwordChangeButton;
        public Button signOutButton;
        private bool _isNewGame = true;
        internal static GetAllSingleStatsResponse GetAllSingleStatsResponse = new();

        private bool isInLobby = false;
        private bool isSceneLoading = false;

              private void Start()
        {
            // Attach listeners to buttons
            newGameButton.onClick.AddListener(OnNewGameButtonClicked);
            continueGameButton.onClick.AddListener(OnGameContinueButtonClicked);
            // future leaderboard or match history handlers go here...

            passwordChangeButton.onClick.AddListener(OnPasswordChangeButtonClicked);
            signOutButton.onClick.AddListener(OnSignOutButtonClicked);

            confirmButton.onClick.AddListener(OnConfirmSelection);

            // Initially hide passcode input and join button
            passcodeInputField.gameObject.SetActive(false);
            joinButton.gameObject.SetActive(false);
            joinButton.onClick.RemoveAllListeners();

            // Disable confirm button until lobby is joined
            confirmButton.interactable = false;

            PhotonNetwork.NickName = PlayerPrefs.GetString("alias", "Player");
            PhotonNetwork.AutomaticallySyncScene = true;

            // Check Photon connection state
            if (PhotonNetwork.IsConnectedAndReady)
            {
                if (PhotonNetwork.InRoom)
                {
                    Debug.Log("Leaving current room before proceeding to dashboard logic.");
                    PhotonNetwork.LeaveRoom();
                }
                else if (!PhotonNetwork.InLobby)
                {
                    Debug.Log("Joining the lobby, please wait...");
                    informText.text = "Joining the lobby, please wait...";
                    informText.color = Color.yellow;
                    informText.fontStyle = FontStyles.Italic;
                    PhotonNetwork.JoinLobby();
                }
                else
                {
                    // Already in the lobby
                    isInLobby = true;
                    confirmButton.interactable = true;
                    informText.text = "Already connected, ready to create or join rooms.";
                    informText.color = Color.green;
                    informText.fontStyle = FontStyles.Normal;
                }
            }
            else
            {
                // Not connected yet, connect to Photon
                Debug.Log("Connecting to Photon...");
                informText.text = "Connecting to Photon...";
                informText.color = Color.yellow;
                informText.fontStyle = FontStyles.Italic;
                PhotonNetwork.ConnectUsingSettings();
            }

            // Subscribe to sceneLoaded for position setting after new game/continue
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            // Unregister from the scene loaded event to avoid memory leaks
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

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
                Debug.Log("Joining Lobby...");
            }
            else
            {
                informText.text = "Photon is not ready yet.";
                informText.color = Color.yellow;
            }
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            isInLobby = true;
            Debug.Log("Successfully joined the Lobby.");
            informText.fontStyle = FontStyles.Normal;
            informText.text = "Connected, you can create or join rooms.";
            informText.color = Color.green;

            // Now allow the user to proceed
            confirmButton.interactable = true;
        }

        public override void OnLeftLobby()
        {
            base.OnLeftLobby();
            isInLobby = false;
            Debug.Log("Left the Lobby.");
            informText.text = "Left the lobby. Please wait...";
            informText.color = Color.yellow;
            confirmButton.interactable = false;
        }
        
        
        void OnConfirmSelection()
        {
            if (!PhotonNetwork.InLobby)
            {
                informText.text = "Unable to proceed. Waiting to join the lobby...";
                informText.color = Color.red;
                return;
            }

            int selectedOption = roomOptionsDropdown.value;
            if (selectedOption == 0)
            {
                // Load CreateRoomScene where user chooses Solo or Multi mode
                PhotonNetwork.LoadLevel("CreateRoomScene");
            }
            else if (selectedOption == 1)
            {
                // Join an existing room
                passcodeInputField.gameObject.SetActive(true);
                joinButton.gameObject.SetActive(true);
                joinButton.onClick.AddListener(JoinRoom);
            }
        }
        

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            Debug.Log("Room successfully created. Loading LobbyScene.");
            if (!isSceneLoading)
            {
                isSceneLoading = true;
                PhotonNetwork.LoadLevel("LobbyScene");
            }
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError("Room creation failed: " + message);
            informText.text = "Room creation failed: " + message;
            informText.color = Color.red;
        }

        void JoinRoom()
        {
            string roomName = passcodeInputField.text;
            Debug.Log("Joining room: " + roomName);
            PhotonNetwork.JoinRoom(roomName);
        }
        
        
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("Successfully joined the room. Loading LobbyScene.");
            if (!isSceneLoading)
            {
                isSceneLoading = true;
                PhotonNetwork.LoadLevel("LobbyScene");
            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogError("Join room failed: " + message);
            informText.text = "Join room failed: " + message;
            informText.color = Color.red;
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

        private async void OnSoloLeaderboardButtonClicked()
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

        private async void OnSingleLeaderboardButtonClicked()
        {
            try
            {
                await CreateGetAllSingleStatsRequest();
            }
            catch (Exception e)
            {
                print(e.ToString());
            }
        }

        // private async void OnMatchHistoryButtonClicked()
        // {
        //     try
        //     {
        //        await CreateGetMultiplayerMatchHistoryRequest();
        //     }
        //     catch (Exception e)
        //     {
        //         print(e.ToString());
        //     }
        // }

        private void OnMatchHistoryButtonClicked()
        {
            SceneManager.LoadScene("TestScene - Hiep/MatchHistoryScene");
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
            }
            else
            {
                print(request.downloadHandler.text);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            isSceneLoading = false;
            SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe after the scene is loaded to prevent multiple triggers.
            var player = GameObject.FindWithTag("Player");
            if (player == null) return;
            if (!_isNewGame)
            {
                player.transform.position = _playerPosition;
            }

            Debug.Log("Player position set after scene load.");
        }

        private IEnumerator CreateNewGameRequest()
        {
            var request = RequestGenerator(RootRequestURL + "/new-game", new[] { "userId" }, new[] { PlayerPrefs.GetString("userId") }, "POST");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                LoadSceneWithLoadingScreen("Scenes/1stscene");
            }
            else
            {
                print(request.downloadHandler.text);
            }
        }

        private IEnumerator CreatePlayerRankingRequest()
        {
            var request = RequestGenerator(RootRequestURL + "/rank", new string[] { }, new string[] { }, "GET");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                GetAllSingleStatsResponse = JsonUtility.FromJson<GetAllSingleStatsResponse>(request.downloadHandler.text);
                LoadSceneWithLoadingScreen("Scenes/LeaderboardScene");
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
                SceneManager.LoadScene("TestScene - Hiep/SoloLeaderboardScene");
            }
            catch (Exception e)
            {
                print(e.ToString());
            }
        }

        private async Task CreateGetAllSingleStatsRequest()
        {
            try
            {
                var client = new HttpClient();
                var response = await client.GetStringAsync(RootRequestURL + "/get-all-single-stats");
                var statsResponseLists = JsonConvert.DeserializeObject<GetAllSingleStatsResponse>(response);
                StaticSingleLeaderboardLists.allSingleStatsLists = statsResponseLists;
                SceneManager.LoadScene("TestScene - Hiep/SingleLeaderboardScene");
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