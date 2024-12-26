using Photon.Pun;
using UnityEngine;

namespace Photon.Solo.Characters.Peasant
{
    public class AttackHandlerSoloPlayer : MonoBehaviourPun
    {
        public Transform arrowStartingPoint;
        public float damageDealt = 1f;
        public float arrowSpeed = 5f;
        public GameObject[] arrowArray; // Pool of arrows
        internal float ArrowDirection;

        public void Fire()
        {
            ArrowDirection = transform.localScale.x > 0 ? 1 : -1;

            var arrowIndex = -1;
            for (var i = 0; i < arrowArray.Length; i++)
            {
                if (arrowArray[i].gameObject.activeInHierarchy) continue;
                arrowIndex = i;
                break;
            }

            if (arrowIndex == -1) return; 

            
            photonView.RPC("RPC_FireArrow", RpcTarget.Others, arrowStartingPoint.transform.position, ArrowDirection, arrowIndex);
        }

        [PunRPC]
        private void RPC_FireArrow(Vector3 position, float direction, int arrowIndex)
        {
            if (arrowIndex < 0 || arrowIndex >= arrowArray.Length) return;
            var arrowFound = arrowArray[arrowIndex];
            
            arrowFound.SetActive(true);
            arrowFound.transform.position = position;
            arrowFound.transform.localScale = new Vector3(
                arrowFound.transform.localScale.x,
                Mathf.Sign(direction) * Mathf.Abs(arrowFound.transform.localScale.y),
                arrowFound.transform.localScale.z
            );
        }
    }
}
