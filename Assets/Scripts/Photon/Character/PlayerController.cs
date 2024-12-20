using Cinemachine;
using Photon.Pun;
using UnityEngine;

namespace Photon.Character
{
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
    {
        [Header("Cinemachine")]
        public CinemachineVirtualCamera playerCamPrefab; // Assign via Inspector

        private CinemachineVirtualCamera playerCam;

        private void Start()
        {
            if (photonView.IsMine)
            {
                // Instantiate the player's virtual camera
                if (playerCamPrefab != null)
                {
                    playerCam = Instantiate(playerCamPrefab, transform.position, Quaternion.identity, transform);
                    playerCam.Follow = this.transform;
                    playerCam.LookAt = this.transform;
                    playerCam.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogError("Player Camera Prefab is not assigned in the PlayerController.");
                }

                // Notify the SoloLobbyManager to add this player to the target group
                if (SoloLobbyManager.Instance != null)
                {
                    SoloLobbyManager.Instance.AddPlayerToTargetGroup(this.transform);
                }
                else
                {
                    Debug.LogError("SoloLobbyManager instance is not found.");
                }
            }
            else
            {
                // Optionally, disable the camera for non-local players
                if (playerCam != null)
                {
                    playerCam.gameObject.SetActive(false);
                }
            }
        }

        private void OnDestroy()
        {
            if (playerCam != null)
            {
                Destroy(playerCam.gameObject);
            }

            if (photonView.IsMine && SoloLobbyManager.Instance != null)
            {
                SoloLobbyManager.Instance.RemovePlayerFromTargetGroup(this.transform);
            }
        }

        // Implement IPunObservable if needed
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Send data to other players
                stream.SendNext(transform.position);
                // Add more variables as needed
            }
            else
            {
                // Receive data from other players
                transform.position = (Vector3)stream.ReceiveNext();
                // Receive more variables as needed
            }
        }
    }
}
