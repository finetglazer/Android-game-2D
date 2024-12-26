using Photon.Pun;
using UnityEngine;

namespace Photon.Solo.Characters.Peasant
{
    public class AttackHandlerSoloPlayer : MonoBehaviourPun
    {
        public Transform arrowStartingPoint; 
        public GameObject[] arrowPrefabs;
        public float damageDealt = 1f;
        public float arrowSpeed = 10f;
        internal float ArrowDirection;

        public bool Fire(int prefabIndex)
        {
            if (!photonView.IsMine) return false; 
            
            if (prefabIndex < 0 || prefabIndex >= arrowPrefabs.Length)
            {
                Debug.LogError("Invalid arrow prefab index!");
                return false;
            }
                
            ArrowDirection = transform.localScale.x > 0 ? 1 : -1;
            
            var arrowPrefab = arrowPrefabs[prefabIndex];
            var arrow = PhotonNetwork.Instantiate(arrowPrefab.name, arrowStartingPoint.position, Quaternion.identity);

            if (arrowPrefab.activeInHierarchy) return false;
            
            arrowPrefab.SetActive(true);
    
            // Adjust scale based on direction
            arrow.transform.localScale = new Vector3(
                arrow.transform.localScale.x,
                Mathf.Sign(gameObject.transform.localScale.x) * Mathf.Abs(arrow.transform.localScale.y),
                arrow.transform.localScale.z
            );
            return true;
        }
    }
}