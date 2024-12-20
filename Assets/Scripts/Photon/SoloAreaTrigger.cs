using UnityEngine;

namespace Photon
{
    public class SoloAreaTrigger: MonoBehaviour
    {
        private SoloLobbyManager lobbyManager;

        private void Start()
        {
            lobbyManager = SoloLobbyManager.Instance;
            if (lobbyManager == null)
            {
                Debug.LogError("SoloLobbyManager instance not found.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                lobbyManager.PlayerEnteredLowestArea();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                lobbyManager.PlayerExitedLowestArea();
            }
        }
    }
}