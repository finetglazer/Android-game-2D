using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.SceneManager.Solo
{
    public class WinningController: MonoBehaviourPunCallbacks
    {
        public TMP_Text winnerNameText;
        public TMP_Text loserNameTxt;
        
        public Button leaveButton;
        
        
        private bool isLeaving = false;
        
        private void Start()
        {
            Hashtable playerProperties = PhotonNetwork.CurrentRoom.CustomProperties;

            // Alternatively, if you set properties on players:
            // Hashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;

            // Access the properties
            string winner = playerProperties.ContainsKey("winner") ? (string)playerProperties["winner"] : "Unknown";
            string loser = playerProperties.ContainsKey("loser") ? (string)playerProperties["loser"] : "Unknown";

            // Assign to UI elements
            winnerNameText.text = winner;
            loserNameTxt.text = loser;

            
            // debug to see if null or not
            Debug.Log("Winner: " + winnerNameText.text);
            Debug.Log("Loser: " + loserNameTxt.text);
            
            // color
            winnerNameText.color = Color.blue;
            loserNameTxt.color = Color.red;
            
            // Set up leave button
            if (leaveButton != null)
            {
                leaveButton.onClick.RemoveAllListeners();
                leaveButton.onClick.AddListener(LeaveRoom);
            }
            else
            {
                Debug.LogError("Leave Button is not assigned in the Inspector.");
            }
            
        }
        
        private void LeaveRoom()
        {
            if (isLeaving) return;

            if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
            {
                isLeaving = true;
                if (leaveButton != null)
                {
                    leaveButton.interactable = false; 
                }
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                Debug.LogWarning("Cannot leave room: Client is not connected or not in a room.");
            }
        }
        
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            PhotonNetwork.JoinLobby();
            PhotonNetwork.LoadLevel("Scenes/DashboardScene");
        }

    }
}