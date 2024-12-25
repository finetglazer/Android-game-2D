using System.Collections;
using Photon.Pun;
using Photon.Solo.Characters.Soldier;
using UnityEngine;
using UnityEngine.Networking;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Solo.Commons.PlayerCore
{
    public class PlayerDieNetworked : MonoBehaviourPun, IPunObservable
    {
        public float deathPoint;
        private Vector3 _respawnPoint;

        private MovementSoloPlayer _movementSoloPlayer;

        private bool isDead = false; // Flag to prevent multiple deaths

        private void Start()
        {
            _respawnPoint = transform.position;
            _movementSoloPlayer = GetComponent<MovementSoloPlayer>();
            if (_movementSoloPlayer == null)
            {
                Debug.LogError("MovementMultiplayer component not found on the player.");
            }
        }

        private void Update()
        {
            if (!_movementSoloPlayer || isDead) return; // Exit if already dead

            if (_movementSoloPlayer.currentHealth <= 0f)
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

            // Set the winner and loser in the GameManager
           
            string winnerName = otherPlayer;
            string loserName = PhotonNetwork.LocalPlayer.NickName;
            
            // Set up the properties of Photon
            Hashtable result = PhotonNetwork.CurrentRoom.CustomProperties;
            result["loser"] = loserName;
            result["winner"] = winnerName;
            PhotonNetwork.CurrentRoom.SetCustomProperties(result);
            
            
            if (photonView.IsMine)
            {
                // Notify all clients to load the appropriate scene, passing winner and loser names
                photonView.RPC("LoadScene", RpcTarget.All);
            }
        }

        [PunRPC]
        private void LoadScene()
        {
            // Load the appropriate scene
            PhotonNetwork.LoadLevel("ResultSoloScene");
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