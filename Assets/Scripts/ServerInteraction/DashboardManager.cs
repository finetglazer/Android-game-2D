﻿using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using ServerInteraction.Responses;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ServerInteraction
{
    public class DashboardManager : MonoBehaviour
    {
        public Button newGameButton;
        public Button continueGameButton;
        public Button leaderboardButton;
        private const string RootRequestURL = "http://localhost:8080/api/gameplay";
        private Vector3 _playerPosition;
        internal static PlayerRankingResponse PlayerRankingResponse = new();
        
        private void Start()
        {
           
            newGameButton.onClick.AddListener(OnNewGameButtonClicked);
            continueGameButton.onClick.AddListener(OnGameContinueButtonClicked);
            leaderboardButton.onClick.AddListener(OnLeaderBoardButtonClicked);
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
                }
                SceneManager.LoadScene(SceneNamesAndURLs.SceneUrLs[gameContinueResponse.sceneIndex - 1]);
                yield return new WaitForSeconds(1);
                print("adaadad");
                var player = GameObject.FindWithTag("Player");
                player.transform.position = _playerPosition;
            }
            else
            {
                print(request.downloadHandler.text);
            }
        }

        private static IEnumerator CreateNewGameRequest()
        {
            var request = RequestGenerator(RootRequestURL + "/new-game", new[] { "userId" }, new []{ PlayerPrefs.GetString("userId") }, "POST");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                SceneManager.LoadScene("Scenes/1stscene");
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
                SceneManager.LoadScene("TestScene - Hiep/RankScene");
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
    }
}