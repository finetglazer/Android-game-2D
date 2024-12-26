using Photon.Pun;
using Photon.Realtime;
using Photon.Solo.Commons.HealthBar;
using UnityEngine;

namespace Photon.Enemy
{
    [RequireComponent(typeof(PhotonView))]
    public class MultiEnemyInitializer: MonoBehaviourPunCallbacks
    {
        private PhotonView _photonView;
        private MultiEnemyMovement _movementScript;
        
        private void Awake()
        {
            // Cache PhotonView component
            _photonView = GetComponent<PhotonView>();

            // Ensure the Movement script is present
            _movementScript = GetComponent<MultiEnemyMovement>();
            if (_movementScript == null)
            {
                Debug.LogError("Movement script is missing on the enemy prefab.");
            }
        }
        private void Start()
        {
            // Only the Master Client should own the enemy
            if (PhotonNetwork.IsMasterClient)
            {
                AssignOwnership();
            }
            else
            {
                // Disable enemy behavior scripts on non-owner clients
                if (_movementScript != null)
                {
                    _movementScript.enabled = false;
                }
            }
        }
        
        private void AssignOwnership()
        {
            if (!_photonView.IsMine)
            {
                _photonView.RequestOwnership();
            }

            // Enable movement script for the owner
            if (_movementScript != null)
            {
                _movementScript.enabled = true;
            }
            
            // Notify HealthBarManager to create a health bar for this player
            HealthBarManager hbManager = FindObjectOfType<HealthBarManager>();
            if (hbManager != null)
            {
                Debug.Log("Creating health bar for player: " + photonView.Owner.NickName);
                hbManager.CreateHealthBar(gameObject);
            }
            else
            {
                Debug.LogError("HealthBarManager not found in the scene.");
            }
        }
        
        // Optional: Handle Master Client switch
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                AssignOwnership();
            }
        }

    }
}