using Photon.Pun;
using UnityEngine;

namespace Photon.Character
{
    public class PlayerInstantiation : MonoBehaviourPunCallbacks
    {
        private void Start()
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                // Notify HealthBarManager to create a health bar for this player
                HealthBarManager hbManager = FindObjectOfType<HealthBarManager>();
                if (hbManager != null)
                {
                    hbManager.CreateHealthBar(this.gameObject);
                }
                else
                {
                    Debug.LogError("HealthBarManager not found in the scene.");
                }
            }
            

        }
    }
}