using Photon.Pun;
using UnityEngine;

namespace Photon.Solo.Characters.Peasant
{
    public class NetworkedInstantiation : MonoBehaviourPunCallbacks
    {
        [Tooltip("Name of the prefab to instantiate. Ensure it's registered in Photon.")]
        public string prefabName;

        [Tooltip("Parent Transform under which the instantiated object will be placed.")]
        public Transform parentTransform;

        [Tooltip("Position offset relative to the parent.")]
        public Vector3 positionOffset = Vector3.zero;

        [Tooltip("Rotation relative to the parent.")]
        public Quaternion rotationOffset = Quaternion.identity;

        /// <summary>
        /// Call this method to instantiate the networked object.
        /// </summary>
        public void InstantiateNetworkedObject()
        {
            if (string.IsNullOrEmpty(prefabName))
            {
                Debug.LogError("Prefab name is not set.");
                return;
            }

            if (parentTransform == null)
            {
                Debug.LogError("Parent Transform is not set.");
                return;
            }

            // Define the spawn position relative to the parent
            Vector3 spawnPosition = parentTransform.position + positionOffset;

            // Instantiate the object via Photon
            GameObject networkedObject = PhotonNetwork.Instantiate(prefabName, spawnPosition, rotationOffset, 0);

            if (networkedObject != null)
            {
                // Set the parent
                networkedObject.transform.SetParent(parentTransform);

                // Optionally, adjust local position and rotation if needed
                networkedObject.transform.localPosition = positionOffset;
                networkedObject.transform.localRotation = rotationOffset;

                Debug.Log($"Instantiated and parented {prefabName} to {parentTransform.name}");
            }
            else
            {
                Debug.LogError($"Failed to instantiate prefab: {prefabName}");
            }
        }

        // Example usage: Instantiate the object when the player presses the "I" key
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                InstantiateNetworkedObject();
            }
        }
    }
}
