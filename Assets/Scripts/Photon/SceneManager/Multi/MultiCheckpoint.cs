using ExitGames.Client.Photon;
using MainCharacter;
using Photon.Pun;
using UnityEngine;

namespace Photon.SceneManager.Multi
{
    public class MultiCheckpoint : MonoBehaviourPunCallbacks
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if the player touched the checkpoint
            if (!other.CompareTag("Player")) return;
        
            // set "position" property to the current position of the player
            Hashtable playerProperties = new Hashtable();
            playerProperties["position"] = transform.position;
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
            Debug.Log("Somthing");
        }
    }
}