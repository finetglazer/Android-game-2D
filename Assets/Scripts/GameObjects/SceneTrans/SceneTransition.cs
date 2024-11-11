using System;
using System.Collections;
using System.Globalization;
using Recorder;
using ServerInteraction;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace GameObjects.SceneTrans
{
    public class SceneTransition : MonoBehaviour
    {
        public string sceneName;
        private bool _sceneFinishTimeUpdated;
        private bool _scenePointUpdated;

        private void Update()
        {
            if (!_scenePointUpdated || !_sceneFinishTimeUpdated) return;
            LoadSubsequentScene();
        }
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            
            DeathNote.ClearLists();
            
            //TODO: Add updateFinishTime and updateScenePoint APIs
            var sceneTime = MainCharacter.Movement.SceneTime;
            StartCoroutine(CreateUpdateSceneFinishTimeRequest());   // Async operation
            StartCoroutine(CreateUpdateScenePointRequest());    // Async operation
        }

        private IEnumerator CreateUpdateSceneFinishTimeRequest()
        {
            const string url = "http://localhost:8080/api/gameplay/update-scene-finish-time";
            var request = new UnityWebRequest(url, "POST");
            var sceneIndex = Array.FindIndex(SceneNamesAndURLs.SceneNames, sceneName => sceneName == SceneManager.GetActiveScene().name) + 1;
            var userId = PlayerPrefs.GetString("userId");
            var jsonBody = "{\"userId\":\"" + userId + "\",\"sceneIndex\":\"" + sceneIndex + "\",\"sceneFinishTime\":\"" + MainCharacter.Movement.SceneTime + "\"}";  
            var jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                _sceneFinishTimeUpdated = true;
                MainCharacter.Movement.SceneTime = 0;
            }
            else
            {
                print(request.downloadHandler.text);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator CreateUpdateScenePointRequest()
        {
            const string url = "http://localhost:8080/api/gameplay/update-scene-points";
            var request = new UnityWebRequest(url, "POST");
            var sceneIndex = Array.FindIndex(SceneNamesAndURLs.SceneNames, sceneName => sceneName == SceneManager.GetActiveScene().name) + 1;
            var userId = PlayerPrefs.GetString("userId");
            var deathCount = GameObject.FindWithTag("Player").GetComponent<MainCharacter.Movement>().deathCount;
            var scenePoint = Mathf.Max(0f, 100f - deathCount * 2 - Mathf.Max((MainCharacter.Movement.SceneTime - 120), 0f));
            var jsonBody = "{\"userId\":\"" + userId + "\",\"sceneIndex\":\"" + sceneIndex + "\",\"scenePoint\":\"" + scenePoint.ToString(CultureInfo.InvariantCulture) + "\"}";   
            var jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                _scenePointUpdated = true;
            }
            else
            {
                print(request.downloadHandler.text);
            }
        }

        private void LoadSubsequentScene()
        {
            MainCharacter.Movement.SceneTime = 0;
            _sceneFinishTimeUpdated = false;
            _scenePointUpdated = false;
            SceneManager.LoadScene(sceneName);
        }
    }
}