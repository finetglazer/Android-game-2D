using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace Photon.Solo.Characters.Peasant
{ 
    public class ArrowSoloMovement : MonoBehaviour
    {
        private AttackHandlerSoloPlayer _characterSoloAttackHandler;
        private GameObject _arrow;
        private float _damageDealt;
        private float _speed ;
        private float _arrowDirection;
        private void Start()
        {
            Debug.Log(transform.parent.name);
            gameObject.SetActive(true);
            var gameObjectList = GameObject.FindGameObjectsWithTag("Player"); 
            var parentPhotonView = transform.parent.GetComponent<PhotonView>();
            var parent = gameObjectList.FirstOrDefault(i => i.GetComponent<PhotonView>() == parentPhotonView);

            if (parent != null) _characterSoloAttackHandler = parent.GetComponent<AttackHandlerSoloPlayer>();
            else print("Parent not found");
            _damageDealt = _characterSoloAttackHandler.damageDealt;
            _speed = _characterSoloAttackHandler.arrowSpeed;
            _arrowDirection = _characterSoloAttackHandler.ArrowDirection;
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
        
        
    }
}