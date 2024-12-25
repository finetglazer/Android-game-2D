using Photon.Multiplayer.Characters.Knight;
using Photon.Pun;
using UnityEngine;

namespace Photon.Multiplayer.Characters.Peasant
{ 
    public class ArrowMultiMovement : MonoBehaviour
    {
        private AttackHandlerMultiPlayer _characterMultiAttackHandler;
        private GameObject _arrow;
        private float _damageDealt;
        private float _speed ;
        private float _arrowDirection;
        private void Start()
        {
            _characterMultiAttackHandler = GetComponentInParent<AttackHandlerMultiPlayer>();  
            _damageDealt = _characterMultiAttackHandler.damageDealt;
            _speed = _characterMultiAttackHandler.arrowSpeed;
            _arrowDirection = _characterMultiAttackHandler.ArrowDirection;
        }

        private void Update()
        {
            transform.Translate(new Vector2(0, _speed * Time.deltaTime * _arrowDirection));   
            // Since arrow has been rotated 90 degrees to be horizontal, the translation should be handled carefully! 
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var playerAnimator = other.GetComponent<Animator>();
                if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("die")) return;
                var enemyPhotonView = other.GetComponent<PhotonView>();
                if (enemyPhotonView is not null && !enemyPhotonView.IsMine)
                {
                    Debug.Log("Player" + other.name + " hit by arrow");
                    enemyPhotonView.RPC("ApplyDamage", RpcTarget.All, _damageDealt);
                }
            }
            
            DeActivateArrow();
        }

        private void DeActivateArrow()
        {
            gameObject.SetActive(false);
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(gameObject.transform.position);
                stream.SendNext(gameObject.transform.localScale);
            }
            else
            {
                gameObject.transform.position = (Vector3)stream.ReceiveNext();
                gameObject.transform.localScale = (Vector3)stream.ReceiveNext();
            }
        }
    }
}