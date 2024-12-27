using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Solo.Commons.PlayerCore
{
    public class PlayerDieNetworked : MonoBehaviourPun, IPunObservable
    {
        public float deathPoint;
        private Vector3 _respawnPoint;

        private Characters.Knight.MovementSoloPlayer _knightMovementSoloPlayer;
        private Characters.Merchant.MovementSoloPlayer _merchantMovementSoloPlayer;
        private Characters.Peasant.MovementSoloPlayer _peasantMovementSoloPlayer;
        private Characters.Soldier.MovementSoloPlayer _soldierMovementSoloPlayer;
        private Characters.Thief.MovementSoloPlayer _thiefMovementSoloPlayer;

        private bool isDead = false; // Flag to prevent multiple deaths

        private void Start()
        {
            _respawnPoint = transform.position;
            _knightMovementSoloPlayer = GetComponent<Characters.Knight.MovementSoloPlayer>();
            _merchantMovementSoloPlayer = GetComponent<Characters.Merchant.MovementSoloPlayer>();
            _peasantMovementSoloPlayer = GetComponent<Characters.Peasant.MovementSoloPlayer>();
            _soldierMovementSoloPlayer = GetComponent<Characters.Soldier.MovementSoloPlayer>();
            _thiefMovementSoloPlayer = GetComponent<Characters.Thief.MovementSoloPlayer>();

            if (AllMovementSoloPlayerAreNull())
            {
                Debug.LogError("MovementSoloPlayer component not found on the player.");
            }
        }

        private void Update()
        {
            if (AllMovementSoloPlayerAreNull() || isDead) return; // Exit if already dead

            var currentHealth = -1f;
            if (_knightMovementSoloPlayer) currentHealth = _knightMovementSoloPlayer.currentHealth;
            else if (_merchantMovementSoloPlayer) currentHealth = _merchantMovementSoloPlayer.currentHealth;
            else if (_peasantMovementSoloPlayer) currentHealth = _peasantMovementSoloPlayer.currentHealth;
            else if (_soldierMovementSoloPlayer) currentHealth = _soldierMovementSoloPlayer.currentHealth;
            else if (_thiefMovementSoloPlayer) currentHealth = _thiefMovementSoloPlayer.currentHealth;
            
            if (currentHealth <= 0f)
            {
                if (photonView.IsMine)
                {
                    if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("type", out object typeValue))
                    {
                        if (typeValue.ToString().Equals("solo"))
                        {
                            Die();
                        }
                        else
                        {
                            MultiDie();
                        }
                    }
                    
                }
            }

            // Optional: Handle death based on position
            if (transform.position.y < deathPoint)
            {
                if (photonView.IsMine)
                {
                    if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("type", out object typeValue))
                    {
                        if (typeValue.ToString().Equals("solo"))
                        {
                            Die();
                        }
                        else
                        {
                            MultiDie();
                        }
                    }
                }
            }
        }

        public void MultiDie()
        {
            Debug.Log("Player has died. Initiating death sequence.");
            // base on the key position in each local player, respawn the player to the position of the key
            Hashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
            if (playerProperties.TryGetValue("position", out object position))
            {
                transform.position = (Vector3) position;
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

        private bool AllMovementSoloPlayerAreNull()
        {
            return (!_knightMovementSoloPlayer 
                    && !_merchantMovementSoloPlayer 
                    && !_peasantMovementSoloPlayer 
                    && !_soldierMovementSoloPlayer 
                    && !_thiefMovementSoloPlayer);
        }
    }
}