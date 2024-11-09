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
        private string _userId;

        private void Start()
        {
            _userId = SignInManager.UserId;
        }

        private void OnGameContinueButtonClicked()
        {
            StartCoroutine(CreateContinueGameRequest());
        }

        private IEnumerator CreateContinueGameRequest()
        {
            const string url = "http://localhost:8080/api/gameplay/continue";
            var request = new UnityWebRequest(url, "POST");
            var jsonBody = "{\"userId\":\"" + _userId + "\"}";
            var jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
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
    }
}