using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Photon.Character
{
    public class HealthBarManager : MonoBehaviourPunCallbacks
    {
        public GameObject healthBarPrefab; // Prefab for the health bar UI
        public Canvas uiCanvas; // Reference to the UI Canvas
        public UnityEngine.Camera mainCamera; // Reference to the main camera

        // Dictionary to keep track of player health bars
        private Dictionary<int, GameObject> playerHealthBars = new Dictionary<int, GameObject>();

        private void Start()
        {
            // Optionally, if HealthBarManager itself is a networked object
            // PhotonNetwork.Instantiate("HealthBarManagerPrefab", Vector3.zero, Quaternion.identity);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            // Optionally handle any setup when a new player joins
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            // Remove the health bar when a player leaves
            if (playerHealthBars.ContainsKey(otherPlayer.ActorNumber))
            {
                Destroy(playerHealthBars[otherPlayer.ActorNumber]);
                playerHealthBars.Remove(otherPlayer.ActorNumber);
            }
        }

        public void CreateHealthBar(GameObject player)
        {
            if (player == null) return;

            // Get the PhotonView ID of the player
            PhotonView pv = player.GetComponent<PhotonView>();
            if (pv == null) return;

            int actorNumber = pv.Owner.ActorNumber;

            if (!playerHealthBars.ContainsKey(actorNumber))
            {
                // Instantiate the health bar UI
                GameObject healthBarUI = Instantiate(healthBarPrefab, uiCanvas.transform);
                
                // Initialize the HealthBarUI script
                HealthBarUI healthBar = healthBarUI.GetComponent<HealthBarUI>();
                if (healthBar != null)
                {

                    healthBar.SetCharacter(player.transform, mainCamera);
                }

                // Add to the dictionary
                playerHealthBars.Add(actorNumber, healthBarUI);
            }
          
        }
    }
}
