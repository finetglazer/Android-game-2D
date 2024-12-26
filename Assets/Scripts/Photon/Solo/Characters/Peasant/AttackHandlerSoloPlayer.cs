using Photon.Pun;
using UnityEngine;

namespace Photon.Solo.Characters.Peasant
{
    public class AttackHandlerSoloPlayer : MonoBehaviourPun
    {
        public Transform parentTransform;
        public Transform arrowStartingPoint; 
        public GameObject[] arrowPrefabs;
        public float damageDealt = 1f;
        public float arrowSpeed = 10f;

        public bool Fire(int prefabIndex)
        {
            if (!photonView.IsMine) return false;

            if (prefabIndex < 0 || prefabIndex >= arrowPrefabs.Length)
            {
                Debug.LogError("Invalid arrow prefab index!");
                return false;
            }

            var arrowPrefab = arrowPrefabs[prefabIndex];

            // Determine direction based on the local scale (1 for right, -1 for left)
            float arrowDirection = Mathf.Sign(transform.localScale.x);

            // Debug log for direction
            Debug.Log($"Firing arrow. Direction: {arrowDirection}");

            // Set rotation: -90 degrees for right, 90 degrees for left around Z-axis
            Quaternion spawnRotation = arrowDirection > 0 
                ? Quaternion.Euler(0, 0, 90) 
                : Quaternion.Euler(0, 0, -90);

            // Instantiate the arrow with the correct rotation
            var arrow = PhotonNetwork.Instantiate(arrowPrefab.name, arrowStartingPoint.position, spawnRotation);

            // Adjust the scale to ensure it's horizontal and correctly oriented
            arrow.transform.localScale = new Vector3(
                Mathf.Abs(arrow.transform.localScale.x) * arrowDirection,
                arrow.transform.localScale.y,
                arrow.transform.localScale.z
            );

            // Debug log for arrow scale
            Debug.Log($"Arrow Scale: {arrow.transform.localScale}");

            // Set direction and speed in the movement script
            var movement = arrow.GetComponent<ArrowSoloMovement>();
            if (movement != null)
            {
                movement.SetDirection(arrowDirection, arrowSpeed);
            }

            return true;
        }
    }
}
