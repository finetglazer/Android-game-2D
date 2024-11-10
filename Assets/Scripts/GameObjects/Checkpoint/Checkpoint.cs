using System;
using System.Collections;
using UnityEngine;
using MainCharacter;
using ServerInteraction;
using UnityEngine.Networking;
using UnityEngine.SceneManagement; // Make sure to include the Cinemachine namespace

namespace GameObjects.Checkpoint
{
    public class Checkpoint : MonoBehaviour
    {
        public float offset;
        // public CinemachineVirtualCamera targetCamera; // Reference to the target virtual camera

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if the player touched the checkpoint
            if (!other.CompareTag("Player")) return;

            var player = other.GetComponent<PlayerDie>();

            // Set the player's respawn position relative to the checkpoint
            player.SetCheckpoint(new Vector3(transform.position.x + offset, transform.position.y, transform.position.z));

            print("Checkpoint activated at: " + transform.position);

            StartCoroutine(CreateUpdateCheckpointLocationRequest());
        }

        private IEnumerator CreateUpdateCheckpointLocationRequest()
        {
            const string url = "http://localhost:8080/api/gameplay/update-checkpoint-location";
            var request = new UnityWebRequest(url, "POST");
            var checkpointLocation = gameObject.transform.position.x + ", " + gameObject.transform.position.y + ", " + gameObject.transform.position.z;
            var sceneIndex = Array.FindIndex(SceneNamesAndURLs.SceneNames, sceneName => sceneName == SceneManager.GetActiveScene().name) + 1;
            var userId = PlayerPrefs.GetString("userId");
            var jsonBody = "{\"userId\":\"" + userId + "\",\"checkpointLocation\":\"" + checkpointLocation + "\",\"sceneIndex\":\"" + sceneIndex + "\"}";  
            var jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            print(request.result == UnityWebRequest.Result.Success
                ? "Updated checkpoint location to server successfully!"
                : request.downloadHandler.text);
        }
        
    }
}
