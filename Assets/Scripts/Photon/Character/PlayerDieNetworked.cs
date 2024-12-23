using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Photon.Character
{
    public class PlayerDieNetworked : MonoBehaviourPun, IPunObservable
    {
        public float deathPoint;
        private Vector3 _respawnPoint;

        private MovementMultiplayer _movementMultiplayer;

        private bool isDead = false; // Flag to prevent multiple deaths

        private void Start()
        {
            _respawnPoint = transform.position;
            _movementMultiplayer = GetComponent<MovementMultiplayer>();
            if (_movementMultiplayer == null)
            {
                Debug.LogError("MovementMultiplayer component not found on the player.");
            }
        }

        private void Update()
        {
            if (!_movementMultiplayer || isDead) return; // Exit if already dead

            if (_movementMultiplayer.currentHealth <= 0f)
            {
                if (photonView.IsMine)
                {
                    Die();
                }
            }

            // Optional: Handle death based on position
            if (transform.position.y < deathPoint)
            {
                if (photonView.IsMine)
                {
                    Die();
                }
            }
        }

        public void Die()
        {
            if (isDead) return; // Prevent multiple executions
            isDead = true; // Set the flag

            Debug.Log("Player has died. Initiating death sequence.");

            // Before loading the next scene, send the API for winner and loser
            const string url = "http://localhost:8080/api/gameplay/update-solo-stats"; // Update with your server's URL
            var request = new UnityWebRequest(url, "POST");

            // Get the name of the other player in the room
            var otherPlayer = PhotonNetwork.PlayerListOthers.Length > 0 ? PhotonNetwork.PlayerListOthers[0].NickName : "Unknown";

            var jsonBody = "{\"winner\":\"" + otherPlayer + "\",\"loser\":\"" + PhotonNetwork.LocalPlayer.NickName + "\"}";
            var jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);

            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Send the request asynchronously
            StartCoroutine(SendRequest(request));

            if (photonView.IsMine)
            {
                // Notify all clients to load the "EndingScene"
                photonView.RPC("LoadScene", RpcTarget.All);
            }
        }

        [PunRPC]
        private void LoadScene()
        {
            Debug.Log("RPC LoadScene called. Loading EndingScene.");
            // Optionally, add delay or effects before loading the scene
            PhotonNetwork.LoadLevel("EndingScene");
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Send respawn position if needed
                stream.SendNext(_respawnPoint);
            }
            else
            {
                // Receive respawn position if needed
                _respawnPoint = (Vector3)stream.ReceiveNext();
            }
        }

        private IEnumerator SendRequest(UnityWebRequest request)
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error sending request: " + request.error);
            }
            else
            {
                Debug.Log("Request sent successfully: " + request.downloadHandler.text);
            }
        }
    }
}
