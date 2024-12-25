using Photon.Pun;
using UnityEngine;

namespace Photon.Multiplayer.Characters.Peasant
{
    public class AttackHandlerMultiPlayer : MonoBehaviourPun
    {
        public Transform arrowStartingPoint;
        public float damageDealt = 1f;
        public float arrowSpeed = 1f;
        public GameObject[] arrowArray;
        internal float ArrowDirection;
        
        public void Fire()
        {
            ArrowDirection = transform.localScale.x > 0 ? 1 : -1;
            var arrowFound = null as GameObject;
            foreach (var arrow in arrowArray)
            {
                if (arrow.gameObject.activeInHierarchy) continue;
                arrowFound = arrow;
                break;
            }
            
            if (arrowFound is null) return;
            
            arrowFound.SetActive(true);
            arrowFound.transform.position = arrowStartingPoint.transform.position;
            arrowFound.transform.localScale = new Vector3(arrowFound.transform.localScale.x,
                Mathf.Sign(gameObject.transform.localScale.x) * Mathf.Abs(arrowFound.transform.localScale.y),
                arrowFound.transform.localScale.z);
            ArrowDirection = Mathf.Sign(gameObject.transform.localScale.x);
            PhotonNetwork.Instantiate("Arrow", transform.position, Quaternion.identity);
        }
    }
}