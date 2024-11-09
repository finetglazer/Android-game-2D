using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace ServerInteraction
{
    public class CreateNewGameContinueRequest : MonoBehaviour
    {
        public Button newGameButton;
        public Button continueGameButton;
        public Button leaderboardButton;
        private string _userId;

        private void Start()
        {
            _userId = SignInManager.UserId;
        }

        private void OnNewGameButtonClicked()
        {
            StartCoroutine(CreateContinueGameRequest());
        }

        private IEnumerator CreateContinueGameRequest()
        {
            const string url = "http://localhost:8080/api/gameplay/new-game";
            var request = new UnityWebRequest(url, "GET");
            var jsonBody = "{\"userId\":\"" + _userId + "\"}";
            var jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                var sceneName = request.result
            }
        }
    }
}