using Photon.Pun;
using UnityEngine;

namespace Photon.SceneManager.Multi
{
    public class MultiSceneTransition : MonoBehaviourPunCallbacks
    {
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            // call RPC to load level
            photonView.RPC("LoadLevel", RpcTarget.All);
            
          // Async operation
        }
        [PunRPC]
        void LoadLevel()
        {
            PhotonNetwork.LoadLevel("MultiScene");
        }

    }
}