using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Photon.Solo.Commons.HealthBar
{
    public class HealthBarManager : MonoBehaviourPunCallbacks
    {
        public GameObject healthBarPrefab;
        public Canvas uiCanvas;
        public Camera mainCamera;

        // Dictionary to keep track of player health bars
        private readonly Dictionary<int, GameObject> _playerHealthBars = new();

        // New dictionary for enemy health bars
        private readonly Dictionary<int, GameObject> _enemyHealthBars = new();
        
        private GameObject _healthBar;
        private GameObject _healthBarEnemy;

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
            if (_playerHealthBars.ContainsKey(otherPlayer.ActorNumber))
            {
                Destroy(_playerHealthBars[otherPlayer.ActorNumber]);
                _playerHealthBars.Remove(otherPlayer.ActorNumber);
            }
        }

        // This method should be called when a player prefab is instantiated
        public void CreateHealthBar(GameObject player)
        {
            if (player == null) return;

            // Get the PhotonView ID of the player
            var pv = player.GetComponent<PhotonView>();
            if (pv == null) return;

            var actorNumber = pv.Owner.ActorNumber;

            if (!_playerHealthBars.ContainsKey(actorNumber))
            {
                // Instantiate the health bar UI
                _healthBar = Instantiate(healthBarPrefab, uiCanvas.transform);
                Debug.Log($"Health bar instantiated for Player {actorNumber}");

                // Initialize the HealthBarUI script
                var healthBar = _healthBar.GetComponent<HealthBarUI>();
                if (healthBar != null)
                {
                    healthBar.SetCharacter(player.transform, mainCamera);
                }

                // Add to the dictionary
                _playerHealthBars.Add(actorNumber, _healthBar);

            }
        }

        // **New Method: CreateEnemyHealthBar**
        public void CreateEnemyHealthBar(GameObject enemy)
        {
            if (enemy == null) return;

            // Get the PhotonView ID of the enemy
            var pv = enemy.GetComponent<PhotonView>();
            if (pv == null) return;

            var viewID = pv.ViewID;

            if (!_enemyHealthBars.ContainsKey(viewID))
            {
                // Instantiate the health bar UI
                _healthBarEnemy = Instantiate(healthBarPrefab, uiCanvas.transform);

                // Initialize the HealthBarUI script
                var healthBar = _healthBarEnemy.GetComponent<HealthBarUI>();
                if (healthBar != null)
                {
                    healthBar.SetCharacterEnemy(enemy.transform, mainCamera, isEnemy: true);
                }

                // Add to the dictionary
                _enemyHealthBars.Add(viewID, _healthBarEnemy);
            }
        }


        private void Update()
        {
            // traverse the dictionary, check the filled health bar and set active false if fill amount <= 0
            Debug.Log("Number of players: " + _playerHealthBars.Count);
            foreach (var playerHealthBar in _playerHealthBars)
            {
                var filledHealthBar = playerHealthBar.Value.transform.Find("FilledHealthBar");
                if (filledHealthBar != null)
                {
                    Debug.Log("player fill health bar found");
                    var filledHealthBarImage = filledHealthBar.GetComponent<UnityEngine.UI.Image>();
                    if (filledHealthBarImage != null)
                    {
                        Debug.Log("player health bar image found");
                        if (filledHealthBarImage.fillAmount <= 0f)
                        {
                            Debug.Log("player health bar fill amount <= 0");
                            playerHealthBar.Value.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("player health bar image not found");
                    }
                }
                else
                {
                    Debug.LogWarning("player fill health bar not found");
                }
            }

            
            // Same for enemy
            Debug.Log("Number of enemies: " + _enemyHealthBars.Count);
            foreach (var enemyHealthBar in _enemyHealthBars)
            {
                var filledHealthBar = enemyHealthBar.Value.transform.Find("FilledHealthBar");
                if (filledHealthBar != null)
                {
                    Debug.Log("enemy fill health bar found");
                    var filledHealthBarImage = filledHealthBar.GetComponent<UnityEngine.UI.Image>();
                    if (filledHealthBarImage != null)
                    {
                        Debug.Log("enemy health bar image found");
                        if (filledHealthBarImage.fillAmount <= 0f)
                        {
                            Debug.Log("enemy health bar fill amount <= 0");
                            enemyHealthBar.Value.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("enemy health bar image not found");
                    }
                }
                else
                {
                    Debug.LogWarning("enemy fill health bar not found");
                }
            }
        }
    }
}
