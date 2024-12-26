using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Photon.Solo.Commons.HealthBar
{
    public class HealthBarManager : MonoBehaviourPunCallbacks
    {
        public GameObject healthBarPrefab; // Prefab for the health bar UI
        public Canvas uiCanvas; // Reference to the UI Canvas
        public UnityEngine.Camera mainCamera; // Reference to the main camera

        // Dictionary to keep track of player health bars
        private Dictionary<int, GameObject> playerHealthBars = new Dictionary<int, GameObject>();

        private GameObject _healthBar;

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

        // This method should be called when a player prefab is instantiated
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
                _healthBar = Instantiate(healthBarPrefab, uiCanvas.transform);

                // Initialize the HealthBarUI script
                HealthBarUI healthBar = _healthBar.GetComponent<HealthBarUI>();
                if (healthBar != null)
                {
                    healthBar.SetCharacter(player.transform, mainCamera);
                }

                // Add to the dictionary
                playerHealthBars.Add(actorNumber, _healthBar);
                
                // Find the object name filledHealthBar in the healthbar
               
                
            }
        }

        private void Update()
        {
            Transform filledHealthBar = _healthBar.transform.Find("FilledHealthBar");
            // Get component image filled health bar and check if fill amount = 0 -> set active false
            if (filledHealthBar != null)
            {
                    
                UnityEngine.UI.Image filledHealthBarImage = filledHealthBar.GetComponent<UnityEngine.UI.Image>();
                if (filledHealthBarImage != null)
                {
                    Debug.Log("haha");
                    if (filledHealthBarImage.fillAmount <= 0f)
                    {
                        _healthBar.gameObject.SetActive(false);
                        Debug.Log("hihihihiihi");
                    }
                    else
                    {
                        Debug.Log("keke");
                    }
                }
            }
          
        }
    }
}
