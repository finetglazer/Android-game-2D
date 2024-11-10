using System.Collections;
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
        public GameObject player;
        public Button newGameButton;
        public Button continueGameButton;
        public Button leaderboardButton;
        private const string RootRequestURL = "http://localhost:8080/api/gameplay/"; 
        private string _userId;

        private void Start()
        {
            // _userId = SignInManager.UserId;
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

        private IEnumerator CreateContinueGameRequest()
        {
            var request = RequestGenerator(RootRequestURL + "/continue", new[] { "userId" }, new[] { _userId }, "POST");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                var gameContinueResponse = JsonUtility.FromJson<GameContinueResponse>(request.downloadHandler.text);
                var currentPositionString = gameContinueResponse.currentPosition;
                var currentPositionNums = currentPositionString.Split(",\\s+").Select(float.Parse).ToArray();
                SceneManager.LoadScene(gameContinueResponse.sceneName);
                player.transform.position = new Vector3(currentPositionNums[0], currentPositionNums[1], currentPositionNums[2]);
            }
            else
            {
                print(request.downloadHandler.text);
            }
        }

        private IEnumerator CreateNewGameRequest()
        {
            var request = RequestGenerator("http://localhost:8080/api/gameplay/new-game", new[] { "userId" }, new []{_userId}, "POST");
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
            var request = RequestGenerator(RootRequestURL + "/rank", new[] { "userId" }, new[] { _userId }, "GET");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                var playerRankingResponse = JsonUtility.FromJson<PlayerRankingResponse>(request.downloadHandler.text);
                SceneManager.LoadScene("Scenes/RankScene");
                // TODO: Add display logic
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
            jsonBody += "\"" + fieldNames[^1] + "\":\"" + values[^1] + "\"}";
            var jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            return request;
        }
    }
}