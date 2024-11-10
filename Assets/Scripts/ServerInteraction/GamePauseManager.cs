using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ServerInteraction
{
    public class GamePauseManager : MonoBehaviour
    {
        public Button pauseButton;
        private void Start()
        {
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }

        private void OnPauseButtonClicked()
        {
            StartCoroutine(CreateUpdatePlayerPositionRequest());
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator CreateUpdatePlayerPositionRequest()
        {
            const string url = "http://localhost:8080/api/gameplay/update-player-position";
            var request = new UnityWebRequest(url, "POST");
            var player = GameObject.FindWithTag("Player");
            var playerPosition = player.transform.position.x + ", " + player.transform.position.y + ", " + player.transform.position.z;
            var sceneIndex = Array.FindIndex(SceneNamesAndURLs.SceneNames, sceneName => sceneName == SceneManager.GetActiveScene().name) + 1;
            var userId = PlayerPrefs.GetString("userId");
            var jsonBody = "{\"userId\":\"" + userId + "\",\"playerPosition\":\"" + playerPosition + "\",\"sceneIndex\":\"" + sceneIndex + "\"}";  
            var jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                SceneManager.LoadScene("TestScene - Hiep/DashboardScene");
            }
            else
            {
                print(request.downloadHandler.text);
            }
        }
    }
}